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
        private const string ServiceName = "SFA.DAS.RoatpRegister";
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





            // MFCMFC See AdminServices for this detail
            //        ApplicationConfiguration = new WebConfiguration
            //        {
            //            RoatpApiClientBaseUrl = "",
            //            RoatpApiAuthentication = new ClientApiAuthentication()

            //};

            //ApplicationConfiguration = ConfigurationService.GetConfig(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });

            //if (!_env.IsDevelopment())
            //{
            //    services.AddDistributedRedisCache(options =>
            //    {
            //        options.Configuration = ApplicationConfiguration.SessionRedisConnectionString;
            //    });
            //}

            //services.AddAntiforgery(options => options.Cookie = new CookieBuilder() { Name = ".Assessors.Staff.AntiForgery", HttpOnly = false });

            //MFCMFC this looks redundant MappingStartup.AddMappings();
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
                config.For<IRoatpServiceApiClient>().Use<RoatpServiceApiClient>();

                
                //MFCMFC not sure if this will be used or not.  LEave in till APR-526 (wiring in API calls) is done
                //config.For<ILog>().Use(x => new NLogLogger(
                //    x.ParentType,
                //    x.GetInstance<IRequestContext>(),
                //    GetProperties())).AlwaysUnique();


                //config.For<IApplyTokenService>().Use<ApplyTokenService>();
                //config.For<IWebConfiguration>().Use(ApplicationConfiguration);
                //config.For<ISessionService>().Use<SessionService>().Ctor<string>().Is(_env.EnvironmentName);
             
                //config.For<IApiClient>().Use<ApiClient>().Ctor<string>().Is(ApplicationConfiguration.ClientApiAuthentication.ApiBaseAddress);

            
                //config.For<IValidationService>().Use<ValidationService>();
             
                //config.For<ISpecialCharacterCleanserService>().Use<SpecialCharacterCleanserService>();


                //config.For<CacheService>().Use<CacheService>();
                //config.For<ISessionService>().Use<SessionService>().Ctor<string>("environment")
                //    .Is(Configuration["EnvironmentName"]);
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

            //if (UseSandbox)
            //{
            //    app.UseMiddleware<SandboxHeadersMiddleware>();
            //}

            //app.UseMiddleware<GetHeadersMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            // app.UseAuthentication();
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
