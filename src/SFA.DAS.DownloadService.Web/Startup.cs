using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.DownloadService.Api.Client;
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
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfigurationRoot _configuration;

        private IWebConfiguration ApplicationConfiguration { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddStorageConfiguration(configuration);

            _configuration = config.Build();
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

            ApplicationConfiguration = _configuration.GetSection(nameof(WebConfiguration)).Get<WebConfiguration>();

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
            services.AddMvc();
            services.AddDataProtection(ApplicationConfiguration, _hostingEnvironment);

            ConfigureDependencyInjection(services);
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IAparMapper, AparMapper>();
            services.AddTransient(x => ApplicationConfiguration);
            services.AddTransient<IDownloadServiceTokenService, TokenService>(serviceProvider =>
                new TokenService(ApplicationConfiguration.DownloadServiceApiAuthentication));
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
            app.UseRouting();
            app.UseSession();
            app.UseHealthChecks("/ping");
            app.UseRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
