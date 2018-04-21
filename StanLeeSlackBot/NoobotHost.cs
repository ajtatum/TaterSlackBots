using System;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using Serilog;
using StanLeeSlackBot.Configuration;
using ILogger = Serilog.ILogger;

namespace StanLeeSlackBot
{
	public class NoobotHost
	{
		private readonly IConfigReader _configReader;
		private INoobotCore _noobotCore;
		private readonly IConfiguration _configuration;

		private readonly ILogger _log;

		public NoobotHost(IConfigReader configReader, ILogger logger)
		{
			_configReader = configReader;
			_configuration = new ExampleConfiguration();

			_log = logger;
		}

		public void Start()
		{
			IContainerFactory containerFactory = new ContainerFactory(_configuration, _configReader);
			INoobotContainer container = containerFactory.CreateContainer();
			_noobotCore = container.GetNoobotCore();

			_log.Information("Connecting...");
			try
			{
				_noobotCore
					.Connect()
					.ContinueWith(task =>
					{
						if (!task.IsCompleted || task.IsFaulted)
						{
							_log.Information($"Error connecting to Slack: {task.Exception}. {(task.Exception.InnerExceptions.Any() ? string.Join(". ", task.Exception.InnerExceptions.Select(x=>x.Message)) : string.Empty) }");
						}
					})
					.GetAwaiter()
					.GetResult();
			}
			catch (Exception e)
			{
				var ai = new TelemetryClient();
				ai.TrackException(e);
				ai.Flush();
				throw;
			}
		}

		public void Stop()
		{
			_log.Information("Disconnecting...");
			_noobotCore?.Disconnect();
		}
	}
}