using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DownloadService.Api.Authentication;
using SFA.DAS.DownloadService.Api.Authorization;
using SFA.DAS.DownloadService.Api.Infrastructure;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Services;
using SFA.DAS.DownloadService.Settings;
using SFA.DAS.DownloadService.Api.Client;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Client.Clients;

namespace SFA.DAS.DownloadService.Api
{
    public class Startup
    {
        private const string ServiceName = "SFA.DAS.DownloadService";
        private const string Version = "1.0";

        private readonly IConfiguration Configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        private IWebConfiguration ApplicationConfiguration { get; set; }
        
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new Info { Title = $"Download Service API {Configuration["InstanceName"]}", Version = "v2" });
                c.EnableAnnotations();
                c.OperationFilter<ExamplesOperationFilter>();
            });

            ApplicationConfiguration = ConfigurationService.GetConfig(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            services.AddApiAuthorization(_hostingEnvironment);
            services.AddApiAuthentication(ApplicationConfiguration.ApiAuthentication);

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

            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });
            services.AddHealthChecks();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDataProtection(ApplicationConfiguration, _hostingEnvironment);

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Download Service API v2");
                });

            app.UseMvc();
        }
    }
}
