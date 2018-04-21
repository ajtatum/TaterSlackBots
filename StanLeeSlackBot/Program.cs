using System;
using System.IO;
using Autofac;
using AutofacSerilogIntegration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.AzureWebApps;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace StanLeeSlackBot
{
	public class Program
	{
		public static IConfiguration Configuration
		{
			get
			{
				var configurationBuilder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
						optional: true, reloadOnChange: true)
					.AddApplicationInsightsSettings()
					.AddEnvironmentVariables();

				if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Development)
				{
					configurationBuilder.AddUserSecrets<Program>(false);
				}

				return configurationBuilder.Build();
			}
		}

		public static void Main(string[] args)
		{


			//Log.Logger = new LoggerConfiguration()
			//	.MinimumLevel.Verbose()
			//	.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			//	.MinimumLevel.Override("System", LogEventLevel.Information)
			//	.Enrich.FromLogContext()
			//	.Enrich.WithThreadId()
			//	.Enrich.WithProcessName()
			//	.Enrich.WithProcessId()
			//	.Enrich.WithExceptionDetails()
			//	.Enrich.With<AzureWebAppsNameEnricher>()
			//	.Enrich.WithProperty("Application", "StanLeeSlackBot")
			//	.WriteTo.Console()
			//	//.WriteTo.RollingFile(
			//	//	new CompactJsonFormatter(), 
			//	//	basedir + "/Logs/StanLeeLog-{Date}.txt", 
			//	//	retainedFileCountLimit: 5)
			//	.WriteTo.File(
			//		@"D:\home\LogFiles\Application\StanLeeLog.txt",
			//		fileSizeLimitBytes: 1_000_000,
			//		rollOnFileSizeLimit: true,
			//		shared: true,
			//		flushToDiskInterval: TimeSpan.FromSeconds(1))
			//	.WriteTo.ApplicationInsightsEvents(telemetryConfiguration)
			//	.CreateLogger();

			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			//string basedir = AppDomain.CurrentDomain.BaseDirectory;
			var appInsight = Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");

			return WebHost.CreateDefaultBuilder(args)
				.UseConfiguration(Configuration)
				.UseStartup<Startup>()
				.UseApplicationInsights(appInsight)
				.UseAzureAppServices()
				.UseSerilog((hostingContext, loggerConfiguration) =>
				{
					var logger = loggerConfiguration
						.MinimumLevel.Verbose()
						.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
						.MinimumLevel.Override("System", LogEventLevel.Information)
						.Enrich.FromLogContext()
						.Enrich.WithThreadId()
						.Enrich.WithProcessName()
						.Enrich.WithProcessId()
						.Enrich.WithExceptionDetails()
						.Enrich.With<AzureWebAppsNameEnricher>()
						.Enrich.WithProperty("Application", "StanLeeSlackBot")
						.WriteTo.Console()
						//.WriteTo.RollingFile(
						//	new CompactJsonFormatter(), 
						//	basedir + "/Logs/StanLeeLog-{Date}.txt", 
						//	retainedFileCountLimit: 5)
						.WriteTo.File(
							@"D:\home\LogFiles\Application\StanLeeLog.txt",
							fileSizeLimitBytes: 1_000_000,
							rollOnFileSizeLimit: true,
							shared: true,
							flushToDiskInterval: TimeSpan.FromSeconds(1))
						.WriteTo.ApplicationInsightsEvents(appInsight)
						.CreateLogger();

					var builder = new ContainerBuilder();
					builder.RegisterLogger(logger);
				})
				.Build();
		}
	}
}