namespace SB.StanLee.Classes
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
		public ServiceConfig()
		{
			Name = "StanLeeSlackBot";
			DisplayName = "Stan Lee SlackBot";
			Description = "An unofficial Stan Lee Slackbot to help you find anything related to Marvel";
		}

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
