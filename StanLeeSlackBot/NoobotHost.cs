using System;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using Serilog;
using StanLeeSlackBot.Configuration;

namespace StanLeeSlackBot
{
	public class NoobotHost
	{
		private readonly IConfigReader _configReader;
		private INoobotCore _noobotCore;
		private readonly IConfiguration _configuration;

		private readonly Serilog.ILogger _logger;

		public NoobotHost(IConfigReader configReader, Serilog.ILogger logger)
		{
			_configReader = configReader;
			_configuration = new ExampleConfiguration();

			_logger = logger;
		}

		public void Start()
		{
			IContainerFactory containerFactory = new ContainerFactory(_configuration, _configReader);
			INoobotContainer container = containerFactory.CreateContainer();
			_noobotCore = container.GetNoobotCore();


			Log.Information("Connecting...");
			try
			{
				_noobotCore
					.Connect()
					.ContinueWith(task =>
					{

						if (!task.IsCompleted || task.IsFaulted)
						{
							Log.Information($"Error connecting to Slack: {task.Exception}");
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
			Log.Information("Disconnecting...");
			_noobotCore?.Disconnect();
		}
	}
}