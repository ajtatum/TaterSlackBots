using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Enrichers.AzureWebApps;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace StanLeeSlackBot
{
	public class Program
	{
		public static int Main(string[] args)
		{
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
			return new WebHostBuilder()
				.UseKestrel()
				.ConfigureServices(services => services.AddAutofac())
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
					.MinimumLevel.Information()
					.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
					.MinimumLevel.Override("System", LogEventLevel.Information)
					.Enrich.FromLogContext()
					.Enrich.WithThreadId()
					.Enrich.WithProcessName()
					.Enrich.WithProcessId()
					.Enrich.WithExceptionDetails()
					.Enrich.With<AzureWebAppsNameEnricher>()
					.Enrich.WithProperty("Application", "StanLeeSlackBot")
					.WriteTo.Async(a =>
						{
							a.Console();

							a.File(
								new CompactJsonFormatter(),
								@"D:\home\LogFiles\Application\StanLeeLog.txt",
								fileSizeLimitBytes: 1_000_000,
								rollOnFileSizeLimit: true,
								shared: true,
								rollingInterval: RollingInterval.Day,
								flushToDiskInterval: TimeSpan.FromSeconds(1));
						},
						bufferSize: 500))
				.UseApplicationInsights()
				.UseAzureAppServices()
				.Build();
		}
	}
}