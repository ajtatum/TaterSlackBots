namespace SB.StanLee.Classes
{
	public interface IAppSettings
	{
		ServiceConfig ServiceConfig { get; set; }
		Marvel Marvel { get; set; }
		Slack Slack { get; set; }
		ApplicationInsights ApplicationInsights { get; set; }
		string AppSettingType { get; set; }
	}
}