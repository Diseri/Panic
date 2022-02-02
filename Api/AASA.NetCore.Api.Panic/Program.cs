using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AASA.NetCore.Lib.Helper;
using AASA.NetCore.Lib.Helper.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace AASA.NetCore.Api.Panic
{
    public class Program
    {
        //private static string environment { get; set; }
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            //environment = GeneralHelpers.GetCurrentEnvironment().GetEnvironmentForAppSetting();

            //configure logging first
            // ConfigureLogging();

            //then create the host, so that if the host fails we can log errors
            CreateHost(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    //configuration.AddJsonFile(
                    //    $"appsettings.json",
                    //    optional: true);
                })
                .UseSerilog();

        private static void CreateHost(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", ex);
                throw;
            }
        }

        private static void ConfigureLogging()
        {
            //var environment = GeneralHelpers.GetCurrentEnvironment().GetEnvironmentForAppSetting(); ;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.json",
                    optional: true)
                .Build();

            //Log.Logger = new LoggerConfiguration()
            //    .Enrich.FromLogContext()
            //    .Enrich.WithMachineName()
            //    .WriteTo.Debug()
            //    .WriteTo.Console()
            //    .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
            //    .Enrich.WithProperty("Environment", environment)
            //    .ReadFrom.Configuration(configuration)
            //    .CreateLogger();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }

    }
}
