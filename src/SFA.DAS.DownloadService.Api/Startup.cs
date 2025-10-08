using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Refit;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.DownloadService.Api.Client;
using SFA.DAS.DownloadService.Api.Client.Clients;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Infrastructure;
using SFA.DAS.DownloadService.Api.SwaggerHelpers.Examples;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Services;
using SFA.DAS.DownloadService.Settings;
using Swashbuckle.AspNetCore.Filters;

namespace SFA.DAS.DownloadService.Api
{
    public class Startup
    {
        private const string ServiceName = "SFA.DAS.DownloadService";
        private const string Version = "1.0";

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
                options.SwaggerDoc("v1", new OpenApiInfo { Title = $"Download Service API {Configuration["InstanceName"]}", Version = "v1" });
                options.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });
                options.CustomSchemaIds(x => x.GetCustomAttributes(false).OfType<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ?? x.Name);
                options.DocInclusionPredicate((name, api) => true);
                options.EnableAnnotations();
                options.ExampleFilters();
            });
            services.AddSwaggerExamplesFromAssemblyOf<AparExample>();
            services.AddSwaggerExamplesFromAssemblyOf<UkpnrAparExample>();

            ApplicationConfiguration = ConfigurationService.GetConfig(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            services.AddRefitClient<IRoatpApiClient>(new RefitSettings(new NewtonsoftJsonContentSerializer()))
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(ApplicationConfiguration.RoatpApiAuthentication.ApiBaseAddress))
                .AddHttpMessageHandler(() => new InnerApiAuthenticationHeaderHandler(new AzureClientCredentialHelper(), ApplicationConfiguration.RoatpApiAuthentication.Identifier));

            services.AddDistributedMemoryCache();
            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });
            services.AddHealthChecks();
            services.AddControllers();
            services.AddDataProtection(ApplicationConfiguration, _hostingEnvironment);

            services.AddLogging(builder =>
            {
                builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
                builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
            });

            services.AddApplicationInsightsTelemetry();

            ConfigureDependencyInjection(services);
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IAparMapper, AparMapper>();
            services.AddTransient(x => ApplicationConfiguration);

            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseHealthChecks("/ping");
            app.UseRequestLocalization();

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "api";
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Download Service API v1");
                });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
