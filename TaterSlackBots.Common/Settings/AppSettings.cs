namespace TaterSlackBots.Common.Settings
{
	public class AppSettings : IAppSettings
	{
		public string AppSettingType { get; set; }

		public ServiceConfig ServiceConfig { get; set; }
		public Marvel Marvel { get; set; }
		public Slack Slack { get; set; }
		public ApplicationInsights ApplicationInsights { get; set; }
	}

	public class ServiceConfig : IServiceConfig
	{
		public StanLeeConfig StanLeeConfig { get; set; }
	}

	public class StanLeeConfig :  IServiceBotConfig
	{
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
	}

	public interface IMarvel
	{
		string PublicKey { get; set; }
		string PrivateKey { get; set; }
	}

	public class Marvel : IMarvel
	{
		public string PublicKey { get; set; }
		public string PrivateKey { get; set; }
	}

	public interface ISlack
	{
		string ApiToken { get; set; }
	}

	public class Slack : ISlack
	{
		public string ApiToken { get; set; }
	}

	public class ApplicationInsights : IApplicationInsights
	{
		public string InstrumentationKey { get; set; }
	}
}
