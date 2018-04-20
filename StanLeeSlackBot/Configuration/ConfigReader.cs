using Microsoft.Extensions.Configuration;
using Noobot.Core.Configuration;

namespace StanLeeSlackBot.Configuration
{
    public class ConfigReader : IConfigReader
    {
        private readonly IConfigurationSection _configurationSection;
        private const string SlackApiConfigValue = "slack:apiToken";

        public ConfigReader(IConfigurationSection configSection)
        {
            _configurationSection = configSection;
        }

        public T GetConfigEntry<T>(string entryName)
        {
            return _configurationSection.GetValue<T>(entryName);
        }

        public string SlackApiKey => GetConfigEntry<string>(SlackApiConfigValue);
        public bool HelpEnabled { get; set; } = true;
        public bool StatsEnabled { get; set; } = true;
        public bool AboutEnabled { get; set; } = true;

	    public string MarvelPublicKey => GetConfigEntry<string>("Marvel:publicKey");
	    public string MarvelPrivateKey => GetConfigEntry<string>("Marvel:privateKey");
	}
}