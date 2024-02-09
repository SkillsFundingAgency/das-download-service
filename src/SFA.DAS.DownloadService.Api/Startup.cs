using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SFA.DAS.DownloadService.Api.Client;
using SFA.DAS.DownloadService.Api.Client.Clients;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Infrastructure;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Services;
using SFA.DAS.DownloadService.Settings;

namespace SFA.DAS.DownloadService.Api
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private IWebConfiguration ApplicationConfiguration { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = $"Download Service API {Configuration["InstanceName"]}"
                    });
                options.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });
                options.CustomSchemaIds(x => x.GetCustomAttributes(false).OfType<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ?? x.Name);
                options.DocInclusionPredicate((name, api) => true);
                options.EnableAnnotations();
            });

            var config = Configuration.AddStorageConfiguration();
            ApplicationConfiguration = config.GetSection(nameof(WebConfiguration)).Get<WebConfiguration>();

            // The authentication of the API has been added but disabled as this is a public API which only
            // exposes data which is already in the public domain, this does not follow the standard APIM
            // pattern by design - following the standard pattern would be tech debt - when that is addressed
            // the authentication below could be re-enabled to make this API a secured internal API
            //services.AddApiAuthorization(_hostingEnvironment);
            //services.AddApiAuthentication(ApplicationConfiguration.ApiAuthentication);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            services.AddHttpClient<IAssessorApiClient, AssessorApiClient>("AssessorApiClient", config =>
            {
                config.BaseAddress = new Uri(ApplicationConfiguration.AssessorApiAuthentication.ApiBaseAddress);
            });

            services.AddHttpClient<IRoatpApiClient, RoatpApiClient>("RoatpApiClient", config =>
            {
                config.BaseAddress = new Uri(ApplicationConfiguration.RoatpApiAuthentication.ApiBaseAddress);
            });

            services.AddHealthChecks();
            services.AddControllers();
            services.AddDataProtection(ApplicationConfiguration, _hostingEnvironment);
            services.AddApplicationInsightsTelemetry();

            ConfigureDependencyInjection(services);
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IAparMapper, AparMapper>();
            services.AddTransient(x => ApplicationConfiguration);

            services.AddTransient<IAssessorTokenService, TokenService>(serviceProvider =>
                new TokenService(ApplicationConfiguration.AssessorApiAuthentication));

            services.AddTransient<IRoatpTokenService, TokenService>(serviceProvider =>
                new TokenService(ApplicationConfiguration.RoatpApiAuthentication));

            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseHealthChecks("/ping");
            app.UseRequestLocalization();

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.RoutePrefix = string.Empty;
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Download Service API v1");
                });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
