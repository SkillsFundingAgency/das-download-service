using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DownloadService.Api.Client.Clients;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Services;
using SFA.DAS.DownloadService.Settings;
using SFA.DAS.DownloadService.Web.Infrastructure;

namespace SFA.DAS.DownloadService.Web
{
    public class Startup
    {
        private const string ServiceName = "SFA.DAS.DownloadService";
        private const string Version = "1.0";

        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration Configuration;

        private IWebConfiguration ApplicationConfiguration { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration);

            config.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["Environment"];
                options.PreFixConfigurationKeys = false;
            });

            Configuration = config.Build();
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

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            services.AddHttpClient<IDownloadServiceApiClient, DownloadServiceApiClient>("DownloadServiceApiClient", config =>
            {
                config.BaseAddress = new Uri(ApplicationConfiguration.DownloadServiceApiAuthentication.ApiBaseAddress);
            });

            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });
            services.AddHealthChecks();
            services.AddMvc(options => options.EnableEndpointRouting = false);
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
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseHealthChecks("/ping");
            app.UseRequestLocalization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
