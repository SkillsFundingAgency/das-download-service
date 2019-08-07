using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Settings;
using SFA.DAS.Roatp.Api.Client;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using SFA.DAS.Roatp.ApplicationServices.Interfaces;
using SFA.DAS.Roatp.ApplicationServices.Services;
using StructureMap;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;

namespace SFA.DAS.DownloadService.Web
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;
        private const string ServiceName = "SFA.DAS.DownloadService";
        private const string Version = "1.0";
        public IConfiguration Configuration { get; }
        public IWebConfiguration ApplicationConfiguration { get; set; }
        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = $"Download Service API {Configuration["InstanceName"]}", Version = "v1" });
                c.EnableAnnotations();               
                c.OperationFilter<ExamplesOperationFilter>();     
            });

            ApplicationConfiguration = new WebConfiguration
            {
                RoatpApiClientBaseUrl = "",
                RoatpApiAuthentication = new ClientApiAuthentication()

            };

            ApplicationConfiguration = ConfigurationService.GetConfig(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var configResult = ConfigureIoC(services);

            return configResult;
        }

        private IServiceProvider ConfigureIoC(IServiceCollection services)
        {
            var container = new Container();
            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.WithDefaultConventions();
                });
                config.For<IRoatpMapper>().Use<RoatpMapper>();
                config.For<IRoatpApiClient>().Use<RoatpApiClient>();
                config.For<ITokenService>().Use<TokenService>();
                config.For<IWebConfiguration>().Use(ApplicationConfiguration);
                config.Populate(services);
            });
            return container.GetInstance<IServiceProvider>();
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
            app.UseRequestLocalization();


            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Download Service API v1");
                });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Download", action = "Index" });
            });
        }


        private IDictionary<string, object> GetProperties()
        {
            var properties = new Dictionary<string, object>();
            properties.Add("Version", GetVersion());
            return properties;
        }


        private string GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
}
