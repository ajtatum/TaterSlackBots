using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using SlackBotNet;
using StanLeeSlackBot.Classes;
using static SlackBotNet.MatchFactory;

namespace StanLeeSlackBot.Controllers
{
    [Produces("application/json")]
    [Route("api/StanLee")]
    public class StanLeeController : Controller
    {
	    private readonly AppSettings _appSettings;
	    private readonly ILoggerFactory _loggerFactory;
	    private readonly Serilog.ILogger _log;

		public StanLeeController(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory, Serilog.ILogger log)
		{
			_appSettings = appSettings.Value;
			_loggerFactory = loggerFactory;
			_log = log;
		}

		public async void Get()
		{
			_log.Information("StanLee getting connected!");

			var bot = await SlackBot.InitializeAsync(_appSettings.Slack.ApiToken,
				cfg =>
				{
					cfg.LoggerFactory = _loggerFactory;
				});

			bot.When(Matches.Text("hello"), async conv =>
			{
				await conv.PostMessage("Hi!");
			})
			.OnException((msg, ex) =>
			{
				_log.Error(ex, "Stan Lee had an error! It's {exception}");
			});
		}
    }
}