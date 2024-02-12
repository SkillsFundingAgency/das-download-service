namespace SFA.DAS.DownloadService.Api
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using global::NLog;
    using global::NLog.Web;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    [ExcludeFromCodeCoverage]
    public class Program
    {

        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var logger = LogManager.Setup().LoadConfigurationFromXml(environment == "Development" ? "nlog.Development.config" : "nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("Starting up host");

                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseNLog();
                });
    }
}
