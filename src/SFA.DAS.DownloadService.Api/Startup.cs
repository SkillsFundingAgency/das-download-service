using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Refit;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DownloadService.Api.Client;
using SFA.DAS.DownloadService.Api.Client.Clients;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Infrastructure;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Services;
using SFA.DAS.DownloadService.Settings;
using Swashbuckle.AspNetCore.Filters;

namespace SFA.DAS.DownloadService.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration);

            config.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });

            _configuration = config.Build();
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
                options.SwaggerDoc("v1", new OpenApiInfo { Title = $"Download Service API {_configuration["InstanceName"]}", Version = "v1" });
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
            });
            
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            var roatpApiAuthentication = _configuration.GetSection("RoatpApiAuthentication").Get<ManagedIdentityApiAuthentication>();
            
            services.AddRefitClient<IRoatpApiClient>(new RefitSettings(new NewtonsoftJsonContentSerializer()))
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(roatpApiAuthentication.ApiBaseAddress))
                .AddHttpMessageHandler(() => new InnerApiAuthenticationHeaderHandler(new AzureClientCredentialHelper(_configuration), roatpApiAuthentication.Identifier));

            services.AddDistributedMemoryCache();
            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });
            services.AddHealthChecks();
            services.AddControllers();
            services.AddDataProtection(_configuration, _hostingEnvironment);

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

            var rewriteOptions = new RewriteOptions()
                .AddRedirect("^$", "api"); 

            app.UseRewriter(rewriteOptions);
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
