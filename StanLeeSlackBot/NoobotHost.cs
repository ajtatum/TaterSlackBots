using System;
using Common.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using StanLeeSlackBot.Configuration;

namespace StanLeeSlackBot
{
	public class NoobotHost
	{
		private readonly IConfigReader _configReader;
		private INoobotCore _noobotCore;
		private readonly IConfiguration _configuration;

		private readonly ILog _log;

		public NoobotHost(IConfigReader configReader, ILog log)
		{
			_configReader = configReader;
			_configuration = new ExampleConfiguration();
			_log = log;
		}

		public void Start()
		{
			IContainerFactory containerFactory = new ContainerFactory(_configuration, _configReader, _log);
			INoobotContainer container = containerFactory.CreateContainer();
			_noobotCore = container.GetNoobotCore();


			_log.Trace("Connecting...");
			try
			{
				_noobotCore
					.Connect()
					.ContinueWith(task =>
					{

						if (!task.IsCompleted || task.IsFaulted)
						{
							_log.Debug($"Error connecting to Slack: {task.Exception}");
						}
					})
					.GetAwaiter()
					.GetResult();
			}
			catch (Exception e)
			{
				_log.Error(e.Message);
				throw;
			}
		}

		public void Stop()
		{
			_log.Trace("Disconnecting...");
			_noobotCore?.Disconnect();
		}
	}
}