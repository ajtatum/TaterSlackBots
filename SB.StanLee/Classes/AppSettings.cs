namespace SB.StanLee.Classes
{
	public class AppSettings
	{
		public AppSettings() {}

		public MarvelSettings Marvel { get; set; }
		public SlackSettings Slack { get; set; }
		public ApplicationInsightsSettings ApplicationInsights { get; set; }
	}

	public class MarvelSettings
	{
		public string PublicKey { get; set; }
		public string PrivateKey { get; set; }
	}
	public class SlackSettings
	{
		public string ApiToken { get; set; }
	}
	public class ApplicationInsightsSettings
	{
		public string InstrumentationKey { get; set; }
	}
}
