using Microsoft.Extensions.Options;

namespace SB.StanLee.Classes
{
	public class AppSettings : IAppSettings
	{
		public AppSettings() {}

		//public AppSettings(IOptions<AppSettings> options)
		//{
		//	Marvel = options.Value.Marvel;
		//	Slack = options.Value.Slack;
		//	ApplicationInsights = options.Value.ApplicationInsights;
		//}

		public Marvel Marvel { get; set; }
		public Slack Slack { get; set; }
		public ApplicationInsights ApplicationInsights { get; set; }
	}

	public class Marvel : IMarvel
	{
		public string PublicKey { get; set; }
		public string PrivateKey { get; set; }
		
		public void Configure(Marvel options)
		{
			PublicKey = options.PublicKey;
			PrivateKey = options.PrivateKey;
		}
	}

	public class Slack : ISlack
	{
		public string ApiToken { get; set; }
		public void Configure(Slack options)
		{
			ApiToken = options.ApiToken;
		}
	}

	public class ApplicationInsights : IApplicationInsights
	{
		public string InstrumentationKey { get; set; }
		public void Configure(ApplicationInsights options)
		{
			InstrumentationKey = options.InstrumentationKey;
		}
	}
}
