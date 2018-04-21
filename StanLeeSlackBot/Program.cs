using System;
using System.IO;
using Autofac;
using AutofacSerilogIntegration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Enrichers.AzureWebApps;
using Serilog.Enrichers.HttpContextData;
using Serilog.Events;
using Serilog.Exceptions;

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

		public static int Main(string[] args)
		{
			var appInsight = Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.MinimumLevel.Override("System", LogEventLevel.Information)
				.Enrich.FromLogContext()
				.Enrich.WithThreadId()
				.Enrich.WithProcessName()
				.Enrich.WithProcessId()
				.Enrich.WithExceptionDetails()
				.Enrich.WithHttpContextData()
				.Enrich.With<AzureWebAppsNameEnricher>()
				.Enrich.WithProperty("Application", "StanLeeSlackBot")
				.WriteTo.Console()
				.WriteTo.ApplicationInsightsEvents(appInsight)
				//.WriteTo.RollingFile(
				//	new CompactJsonFormatter(), 
				//	basedir + "/Logs/StanLeeLog-{Date}.txt", 
				//	retainedFileCountLimit: 5)
				.WriteTo.File(
					@"D:\home\LogFiles\Application\StanLeeLog-{Date}.txt",
					fileSizeLimitBytes: 1_000_000,
					rollOnFileSizeLimit: true,
					shared: true,
					rollingInterval: RollingInterval.Day,
					flushToDiskInterval: TimeSpan.FromSeconds(1))
				.CreateLogger();

			var builder = new ContainerBuilder();
			builder.RegisterLogger();

			try
			{
				Log.Information("Starting web host");
				BuildWebHost(args).Run();
				return 0;
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Host terminated unexpectedly");
				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			var appInsight = Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");

			return WebHost.CreateDefaultBuilder(args)
				.UseConfiguration(Configuration)
				.UseStartup<Startup>()
				.UseSerilog()
				.UseApplicationInsights(appInsight)
				.UseAzureAppServices()
				.Build();
		}
	}
}