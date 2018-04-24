using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using AutofacSerilogIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SB.StanLee.Bots;
using SB.StanLee.Classes;
using SB.StanLee.Services;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;
using ILogger = Serilog.ILogger;

namespace SB.StanLee
{
	public class IoC
	{
		private readonly ILogger _logger;

		public IConfiguration  Configuration { get; private set; }
		public AutofacServiceProvider AutofacServiceProvider { get; private set; }

		public IoC(ILogger logger)
		{
			_logger = logger;
		}

		public IConfiguration ConfigureApplication()
		{
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			if (string.IsNullOrWhiteSpace(environment))
				throw new NullReferenceException("Environment not found in ASPNETCORE_ENVIRONMENT");

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();
            
			Configuration = builder.Build();
			return Configuration;
		}

		public ServiceCollection ConfigureServices(IConfiguration configuration, ILoggerFactory loggerFactory, ILogger logger)
		{
			var serviceCollection = new ServiceCollection();

			serviceCollection.AddOptions();
			serviceCollection.AddSingleton(Configuration);
			serviceCollection.Configure<AppSettings>(Configuration);

			serviceCollection.AddLogging(loggingBuilder =>
			{
				loggingBuilder.AddSerilog(dispose: true, logger: logger);
			});

			return serviceCollection;
		}

		public AutofacServiceProvider ConfigureAutofac(ServiceCollection serviceCollection)
		{
			var containerBuilder = new ContainerBuilder();
			containerBuilder.Populate(serviceCollection);
			containerBuilder.RegisterModule(new AutofacModule());
			var container = containerBuilder.Build();

			using (var scope = container.BeginLifetimeScope())
			{
				var stanLeeBot = scope.Resolve<IStanLeeBot>();
			}

			AutofacServiceProvider = new AutofacServiceProvider(container);
			return AutofacServiceProvider;
		}

		public static ILogger ConfigureLogger()
		{
			return new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.MinimumLevel.Override("System", LogEventLevel.Information)
				.Enrich.FromLogContext()
				.Enrich.WithThreadId()
				.Enrich.WithProcessName()
				.Enrich.WithProcessId()
				.Enrich.WithExceptionDetails()
				.Enrich.WithDemystifiedStackTraces()
				.Enrich.WithProperty("Application", "StanLeeSlackBot")
				.WriteTo.Console()
				.WriteTo.Debug()
				.WriteTo.Seq("http://localhost:5341")
				.WriteTo.File(
					new CompactJsonFormatter(),
					@"D:\home\LogFiles\Application\Console.StanLeeSlackBot.txt",
					fileSizeLimitBytes: 1_000_000,
					rollOnFileSizeLimit: true,
					shared: true,
					rollingInterval: RollingInterval.Day,
					flushToDiskInterval: TimeSpan.FromSeconds(1))
				.CreateLogger();
		}
	}
}
