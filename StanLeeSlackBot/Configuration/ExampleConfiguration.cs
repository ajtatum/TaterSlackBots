using Noobot.Core.Configuration;
using Noobot.Toolbox.Middleware;
using Noobot.Toolbox.Plugins;
using StanLeeSlackBot.Middleware;

namespace StanLeeSlackBot.Configuration
{
    public class ExampleConfiguration : ConfigurationBase
    {
        public ExampleConfiguration()
        {
			UseMiddleware<WelcomeMiddleware>();
			UseMiddleware<AdminMiddleware>();
			UseMiddleware<ScheduleMiddleware>();
			UseMiddleware<JokeMiddleware>();
			UseMiddleware<YieldTestMiddleware>();
			UseMiddleware<PingMiddleware>();
			UseMiddleware<MarvelMiddleware>();

			UsePlugin<JsonStoragePlugin>();
			UsePlugin<SchedulePlugin>();
			UsePlugin<AdminPlugin>();
			UsePlugin<PingPlugin>();
		}
    }
}