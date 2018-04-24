using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PeterKottas.DotNetCore.WindowsService;
using Serilog;
using Serilog.Extensions.Logging;

namespace SB.StanLee
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ServiceRunner<StanLeeWinService>.Run(config =>
			{
				var name = config.GetDefaultName();
				config.Service(serviceConfig =>
				{
					serviceConfig.ServiceFactory((extraArguments, controller) => new StanLeeWinService(controller));

					serviceConfig.OnStart((service, extraParams) =>
					{
						Console.WriteLine("Configuring Logger and LoggerFactory");

						Log.Logger = IoC.ConfigureLogger();
						var loggerFactory = new LoggerFactory(new List<ILoggerProvider>()
						{
							new SerilogLoggerProvider(logger: Log.Logger, dispose: true)
						});

						Console.WriteLine("Configuring IoC");

						var ioc = new IoC(Log.Logger);
						var configuration = ioc.ConfigureApplication();
						var services = ioc.ConfigureServices(configuration, loggerFactory,  Log.Logger);
						var di = ioc.ConfigureAutofac(services);

						Console.WriteLine("Service {0} started", name);
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
	}
}
