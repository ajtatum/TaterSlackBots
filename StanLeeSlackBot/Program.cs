using System;
using System.IO;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.AzureWebApps;
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

		public static int Main(string[] args)
		{
			string basedir = AppDomain.CurrentDomain.BaseDirectory;

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(Configuration)
				.Enrich.FromLogContext()
				.Enrich.WithThreadId()
				.Enrich.WithProcessName()
				.Enrich.WithProcessId()
				.Enrich.WithExceptionDetails()
				.Enrich.With<AzureWebAppsNameEnricher>()
				.Enrich.WithProperty("Application", "StanLeeSlackBot")
				.WriteTo.Console()
				.WriteTo.RollingFile(new CompactJsonFormatter(), basedir + "/Logs/StanLeeLog-{Date}.txt", retainedFileCountLimit: 5)
				.WriteTo.ApplicationInsightsEvents(new TelemetryClient())
				.CreateLogger();

			try
			{
				Log.Information("Getting the motors running...");

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

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging(builder =>
				{
					builder.ClearProviders();
				})
				.UseConfiguration(Configuration)
				.UseStartup<Startup>()
				.UseApplicationInsights()
				.UseSerilog()
				.Build();
	}
}