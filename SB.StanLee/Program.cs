using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeterKottas.DotNetCore.WindowsService;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;
using TaterSlackBots.Common.Settings;
using ILogger = Serilog.ILogger;

namespace SB.StanLee
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Configuring Logger and LoggerFactory");
			var (log, loggerFactory) = ConfigureLogger<Program>();

			Console.WriteLine("Configuring IoC");

			var ioc = new IoC(Log.Logger);
			var configuration = ioc.ConfigureApplication();
			var services = ioc.ConfigureServices(configuration, loggerFactory, Log.Logger);
			var di = ioc.ConfigureAutofac(services);
			
			ServiceRunner<StanLeeWinService>.Run(config =>
			{
				var appSettings = di.GetRequiredService<IAppSettings>();
				config.SetName(appSettings.ServiceConfig.StanLeeConfig.Name);
				config.SetDisplayName(appSettings.ServiceConfig.StanLeeConfig.DisplayName);
				config.SetDescription(appSettings.ServiceConfig.StanLeeConfig.Description);

				var name = config.GetDefaultName();

				config.Service(serviceConfig =>
				{
					serviceConfig.ServiceFactory((extraArguments, controller) => new StanLeeWinService(controller));

					serviceConfig.OnStart((service, extraParams) =>
					{
						log.Information($"Using the AppSettingsType: {configuration.GetValue<string>("AppSettingType")}");

						log.Information($"Service {name} starting. Extra params: {{extraParams}}", extraParams);
						Console.WriteLine("Service {0} starting", name);
						service.Start();
					});

					serviceConfig.OnStop(service =>
					{
						Console.WriteLine("Service {0} stopped", name);
						service.Stop();
					});

					serviceConfig.OnError(e =>
					{
						Log.Error(e, $"Service {name} error");
					});
				});
			});
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
