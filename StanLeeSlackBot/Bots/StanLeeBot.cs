using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackBotNet;
using StanLeeSlackBot.Classes;
using StanLeeSlackBot.Services;

namespace StanLeeSlackBot.Bots
{
    public class StanLeeBot : IStanLeeBot
	{
	    private readonly AppSettings _appSettings;
	    private readonly ILoggerFactory _loggerFactory;
	    private readonly IMarvelService _marvelService;
	    private readonly Serilog.ILogger _log;

		public StanLeeBot(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory, Serilog.ILogger log, IMarvelService marvelService)
		{
			_appSettings = appSettings.Value;
			_loggerFactory = loggerFactory;
			_log = log;
			_marvelService = marvelService;
		}

		public async Task Start()
		{
			_log.Information("StanLee getting connected!");

			var bot = await SlackBot.InitializeAsync(_appSettings.Slack.ApiToken,
				cfg =>
				{
					cfg.LoggerFactory = _loggerFactory;
					cfg.WhenHandlerMatchMode = WhenHandlerMatchMode.FirstMatch;
				});

			bot.When(MatchFactory.Matches.Text("hello"), async conv =>
				{
					await conv.PostMessage("Hi!");
				})
				.OnException((msg, ex) =>
				{
					_log.Error(ex, "Stan Lee had an error! It's {exception}");
				});

			bot.When(MatchFactory.Matches.Text("whois"), async conv =>
				{
					const string toBeSearched = "whois";
					var marvelCharacterStr = conv.Text.StripAscii()
						.Substring(conv.Text.IndexOf(toBeSearched, StringComparison.Ordinal) + toBeSearched.Length)
						.TrimPunctuation()
						.Trim();

					var marvelCharacter = await _marvelService.GetCharacter(marvelCharacterStr);

					var response = string.Empty;

					if (marvelCharacter != null)
					{
						response = $"Excelsior! I found {marvelCharacter.Name} :star-struck:! {marvelCharacter.Description}.{Environment.NewLine}{Environment.NewLine}";
						response += $"{Environment.NewLine}{Environment.NewLine}";
						response += $"{marvelCharacter.Name} has appeared in {marvelCharacter.Comics.Available} comics!";
						response += $"{Environment.NewLine}{Environment.NewLine}";
						response += $"Learn more here: {marvelCharacter.Urls.FirstOrDefault(x => x.Type == "detail")?.Value ?? "... or not."}";
					}
					else
					{
						response = $"I wasn't able to find anything about {marvelCharacterStr}...";
					}

					await conv.PostMessage(response);
				})
				.OnException((msg, ex) =>
				{
					_log.Error(ex, "Stan Lee had an error! It's {exception}");
				});
		}
	}
}