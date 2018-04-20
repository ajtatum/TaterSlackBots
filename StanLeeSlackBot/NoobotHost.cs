using System;
using Common.Logging;
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

		public NoobotHost(IConfigReader configReader)
		{
			_configReader = configReader;
			_configuration = new ExampleConfiguration();
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
				Log.Information(e.Message);
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