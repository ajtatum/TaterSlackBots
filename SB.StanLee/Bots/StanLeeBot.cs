using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SlackBotNet;
using TaterSlackBots.Common.Extensions;
using TaterSlackBots.Common.Services;
using TaterSlackBots.Common.Settings;

namespace SB.StanLee.Bots
{
	public class StanLeeBot : ITaterSlackBot, IHostedService, IDisposable
	{
		private readonly ILogger _logger;
		private readonly IApplicationLifetime _appLifetime;
		private readonly ILoggerFactory _loggerFactory;
		private readonly IAppSettings _appSettings;
		private readonly IMarvelService _marvelService;

		private bool _stopping;
		private Task _backgroundTask;

		public StanLeeBot(ILogger<StanLeeBot> logger, IApplicationLifetime appLifetime, ILoggerFactory loggerFactory, IAppSettings appSettings, IMarvelService marvelService)
		{
			_logger = logger;
			_appLifetime = appLifetime;
			_loggerFactory = loggerFactory;
			_appSettings = appSettings;
			_marvelService = marvelService;
		}

		//public async Task Start()
		//{
		//	_log.Information("StanLee getting connected!");

		//	var bot = await SlackBot.InitializeAsync(_appSettings.Slack.ApiToken,
		//		cfg =>
		//		{
		//			cfg.LoggerFactory = _loggerFactory;
		//			cfg.WhenHandlerMatchMode = WhenHandlerMatchMode.FirstMatch;

		//			cfg.OnSendMessageFailure = async (queue, msg, logger, e) =>
		//			{
		//				if (msg.SendAttempts <= 5)
		//				{
		//					logger?.LogWarning($"Failed to send message {msg.Text}. Tried {msg.SendAttempts} times");
		//					await Task.Delay(1000 * msg.SendAttempts);
		//					queue.Enqueue(msg);
		//					return;
		//				}

		//				logger?.LogError($"Gave up trying to send message {msg.Text}");
		//			};
		//		});


		//	bot.When(MatchFactory.Matches.Text("hello"), async conv =>
		//	{
		//		await conv.PostMessage("Hi!");
		//	})
		//		.OnException((msg, ex) =>
		//		{
		//			_log.Error(ex, "Stan Lee had an error! It's {exception}");
		//		});

		//	bot.When(MatchFactory.Matches.Text("whois"), async conv =>
		//	{
		//		const string toBeSearched = "whois";
		//		var marvelCharacterStr = conv.Text.StripAscii()
		//			.Substring(conv.Text.IndexOf(toBeSearched, StringComparison.Ordinal) + toBeSearched.Length)
		//			.TrimPunctuation()
		//			.Trim();

		//		var marvelCharacter = await _marvelService.GetCharacter(marvelCharacterStr);

		//		var response = string.Empty;

		//		if (marvelCharacter != null)
		//		{
		//			response = $"Excelsior! I found {marvelCharacter.Name} :star-struck:! {marvelCharacter.Description}.{Environment.NewLine}{Environment.NewLine}";
		//			response += $"{Environment.NewLine}{Environment.NewLine}";
		//			response += $"{marvelCharacter.Name} has appeared in {marvelCharacter.Comics.Available} comics!";
		//			response += $"{Environment.NewLine}{Environment.NewLine}";
		//			response += $"Learn more here: {marvelCharacter.Urls.FirstOrDefault(x => x.Type == "detail")?.Value ?? "... or not."}";
		//		}
		//		else
		//		{
		//			response = $"I wasn't able to find anything about {marvelCharacterStr}...";
		//		}

		//		await conv.PostMessage(response);
		//	})
		//		.OnException((msg, ex) =>
		//		{
		//			_log.Error(ex, "Stan Lee had an error! It's {exception}");
		//		});
		//}



		public Task StartAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("StanLeeBot is starting.");
			_backgroundTask = StanLeeBotWork();
			return Task.CompletedTask;
		}

		private async Task StanLeeBotWork()
		{
			while (!_stopping)
			{
				_logger.LogInformation("StanLee getting connected!");

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
							_stopping = true;
						});

				bot.When(MatchFactory.Matches.Text("hello"), async conv =>
				{
					
					await conv.PostMessage("Hi!");
				})
					.OnException((msg, ex) =>
					{
						_logger.LogError(ex, $"Stan Lee had an error! It's {ex}");
					});

				bot.When(MatchFactory.Matches.Text("whois"), async conv =>
				{
					const string toBeSearched = "whois";
					var marvelCharacterStr = conv.Text.StripAscii()
						.Substring(conv.Text.IndexOf(toBeSearched, StringComparison.Ordinal) + toBeSearched.Length)
						.TrimPunctuation()
						.Trim();

					_logger.LogInformation($"Request by  {conv.From.Username}");

					var marvelCharacter = await _marvelService.GetCharacter(marvelCharacterStr);

					var response = string.Empty;

					if (marvelCharacter != null)
					{
						_logger.LogInformation($"Found information for {marvelCharacterStr}. Marvel Character ID: {marvelCharacter.Id}");

						response = $"Excelsior! I found {marvelCharacter.Name} :star-struck:! {marvelCharacter.Description}.{Environment.NewLine}{Environment.NewLine}";
						response += $"{Environment.NewLine}{Environment.NewLine}";
						response += $"{marvelCharacter.Name} has appeared in {marvelCharacter.Comics.Available} comics!";
						response += $"{Environment.NewLine}{Environment.NewLine}";
						response += $"Learn more here: {marvelCharacter.Urls.FirstOrDefault(x => x.Type == "detail")?.Value ?? "... or not."}";
					}
					else
					{
						_logger.LogInformation($"Character not found: {marvelCharacterStr}.");

						response = $"I wasn't able to find anything about {marvelCharacterStr}...";
					}

					_logger.LogInformation($"Sending information to Slack for {marvelCharacterStr} to {conv.From.Username}.");

					await conv.PostMessage(response);
				})
				.OnException((msg, ex) =>
				{
					_logger.LogError(ex, $"Stan Lee had an error! It's {ex}");
				});
			}

			_logger.LogInformation("StanLeeBot is stopping");
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("StanLeeBot is stopping.");
			_stopping = true;
			if (_backgroundTask != null)
			{
				// TODO: cancellation
				await _backgroundTask;
			}
		}

		public void Dispose()
		{
			Console.WriteLine("StanLeeBot is disposing.");
		}
	}
}
