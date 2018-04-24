using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackBotNet;
using SB.StanLee.Classes;
using SB.StanLee.Services;

namespace SB.StanLee.Bots
{
	public class StanLeeBot : IStanLeeBot
	{
		private readonly IAppSettings _appSettings;
		private readonly ILoggerFactory _loggerFactory;
		private readonly IMarvelService _marvelService;
		private readonly Serilog.ILogger _log;

		public StanLeeBot(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory, Serilog.ILogger log, IMarvelService marvelService)
		{
			_appSettings = appSettings.Value;
			_loggerFactory = loggerFactory;
			_log = log.ForContext<StanLeeBot>();
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

					cfg.OnSendMessageFailure = async (queue, msg, logger, e) =>
					{
						if (msg.SendAttempts <= 5)
						{
							logger?.LogWarning($"Failed to send message {msg.Text}. Tried {msg.SendAttempts} times");
							await Task.Delay(1000 * msg.SendAttempts);
							queue.Enqueue(msg);
							return;
						}

						logger?.LogError($"Gave up trying to send message {msg.Text}");
					};
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