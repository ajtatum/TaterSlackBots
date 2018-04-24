namespace SB.StanLee.Classes
{
	public interface IAppSettings
	{
		Marvel Marvel { get; set; }
		Slack Slack { get; set; }
		ApplicationInsights ApplicationInsights { get; set; }
	}

	public interface IMarvel
	{
		string PublicKey { get; set; }
		string PrivateKey { get; set; }
		void Configure(Marvel options);
	}

	public interface ISlack
	{
		string ApiToken { get; set; }
		void Configure(Slack options);
	}

	public interface IApplicationInsights
	{
		string InstrumentationKey { get; set; }
		void Configure(ApplicationInsights options);
	}
}