namespace StanLeeSlackBot.Classes
{
	public class AppSettings
	{
		public AppSettings() {}

		public Marvel Marvel { get; set; }
		public Slack Slack { get; set; }
		public ApplicationInsights ApplicationInsights { get; set; }
	}

	public class Marvel
	{
		public string PublicKey { get; set; }
		public string PrivateKey { get; set; }
	}
	public class Slack
	{
		public string ApiToken { get; set; }
	}
	public class ApplicationInsights
	{
		public string InstrumentationKey { get; set; }
	}
}
