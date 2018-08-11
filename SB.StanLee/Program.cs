using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SB.StanLee.Bots;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;
using TaterSlackBots.Common.Extensions;
using TaterSlackBots.Common.Services;
using TaterSlackBots.Common.Settings;
using ILogger = Serilog.ILogger;

namespace SB.StanLee
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			Console.WriteLine("Configuring Logger and LoggerFactory");
			var (seriLogger, loggerFactory) = ConfigureLogger<Program>();

			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			if (string.IsNullOrWhiteSpace(environment))
				throw new NullReferenceException("Environment not found in ASPNETCORE_ENVIRONMENT");

			var host = new HostBuilder()
				 .ConfigureHostConfiguration(configHost =>
				 {
					 configHost.SetBasePath(Directory.GetCurrentDirectory());
					 configHost.AddJsonFile("hostsettings.json", optional: true);
					 configHost.AddEnvironmentVariables(prefix: "ASPNETCORE_");
					 configHost.AddCommandLine(args);
				 })
				 .ConfigureAppConfiguration((hostContext, configApp) =>
				 {
					 configApp.AddJsonFile("appsettings.json", optional: true);
					 configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
					 configApp.AddEnvironmentVariables(prefix: "ASPNETCORE_");
					 configApp.AddCommandLine(args);
				 })
				.ConfigureServices((hostContext, services) =>
				{
					services.AddOptions();
					services.AddSingleton(hostContext.Configuration);

					var appSettings = new AppSettings();
					services.ConfigurePOCO<IAppSettings>(hostContext.Configuration, appSettings);

					services.AddTransient<IMarvelService>(ms =>
					{
						var appService = ms.GetService<IAppSettings>();
						return new MarvelService(appService, seriLogger);
					});

					services.AddHostedService<StanLeeBot>();
				})
				.UseSerilog();

			await host.RunAsServiceAsync();
		}

		public static (ILogger, ILoggerFactory) ConfigureLogger<T>()
		{
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			if (string.IsNullOrWhiteSpace(environment))
				throw new NullReferenceException("Environment not found in ASPNETCORE_ENVIRONMENT");

			var logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("System", LogEventLevel.Information)
				.Enrich.FromLogContext()
				.Enrich.WithThreadId()
				.Enrich.WithProcessName()
				.Enrich.WithProcessId()
				.Enrich.WithExceptionDetails()
				.Enrich.WithDemystifiedStackTraces()
				.Enrich.WithProperty("Application", "StanLeeSlackBot")
				.Enrich.WithProperty("ASPNETCORE_ENVIRONMENT", environment)
				.WriteTo.Console()
				.WriteTo.Debug()
				//.WriteTo.Seq("http://localhost:5341", apiKey: "tz0UsDNNCKMQF1dYRX38", compact: true)
				.WriteTo.File(
					new CompactJsonFormatter(),
					@"D:\home\LogFiles\Application\Console.StanLeeSlackBot.txt",
					fileSizeLimitBytes: 1_000_000,
					rollOnFileSizeLimit: true,
					shared: true,
					rollingInterval: RollingInterval.Day,
					flushToDiskInterval: TimeSpan.FromSeconds(1))
				.CreateLogger();

			Log.Logger = logger;

			var loggerFactory = new LoggerFactory(new List<ILoggerProvider>()
			{
				new SerilogLoggerProvider(logger: Log.Logger, dispose: true)
			});

			return (logger.ForContext<T>(), loggerFactory);
		}
	}
}
