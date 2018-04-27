namespace TaterSlackBots.Common.Settings
{
	public interface IAppSettings
	{
		string AppSettingType { get; set; }

		ServiceConfig ServiceConfig { get; set; }
		Marvel Marvel { get; set; }
		Slack Slack { get; set; }
		ApplicationInsights ApplicationInsights { get; set; }
	}
}
