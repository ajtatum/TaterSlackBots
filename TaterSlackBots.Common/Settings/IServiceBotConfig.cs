namespace TaterSlackBots.Common.Settings
{
	public interface IServiceBotConfig
	{
		string Name { get; set; }
		string DisplayName { get; set; }
		string Description { get; set; }
	}
}
