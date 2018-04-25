using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SB.StanLee.Bots;
using SB.StanLee.Classes;
using SB.StanLee.Extensions;
using Serilog;
using ILogger = Serilog.ILogger;

namespace SB.StanLee
{
	public class IoC
	{
		private readonly ILogger _logger;

		public IConfiguration Configuration { get; private set; }
		public AutofacServiceProvider AutofacServiceProvider { get; private set; }

		public IoC(ILogger logger)
		{
			_logger = logger.ForContext<IoC>();
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

			var appSettings = new AppSettings();
			serviceCollection.ConfigurePOCO<IAppSettings>(Configuration, appSettings);

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
		
	}
}
