using System;
using System.Collections.Generic;
using System.Linq;
//using MarvelAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;
using StanLeeSlackBot.Classes;

namespace StanLeeSlackBot.Middleware
{
	public class MarvelMiddleware : MiddlewareBase
	{
		private readonly StatsPlugin _statsPlugin;
		private IConfiguration _configuration;
		//private Marvel _Marvel { get; set; }

		public MarvelMiddleware(IMiddleware next, StatsPlugin statsPlugin, IConfiguration configuration) : base(next)
		{
			_statsPlugin = statsPlugin;
			_configuration = configuration;
			//_Marvel = new Marvel(_configuration["Marvel:publicKey"], _configuration["Marvel:privateKey"]);

			HandlerMappings = new[]
			{
				new HandlerMapping
				{
					ValidHandles = StartsWithHandle.For("whois"),
					Description = "Marvel Character lookup",
					EvaluatorFunc = MarvelHandler,
					MessageShouldTargetBot = true,
					ShouldContinueProcessing = false,
					VisibleInHelp = true
				}
			};
		}

		private IEnumerable<ResponseMessage> MarvelHandler(IncomingMessage message, IValidHandle matchedHandle)
		{
			yield return message.IndicateTypingOnChannel();

			_statsPlugin.IncrementState("WhoIs:Lookups");

			var marvelCharacterStr = message.TargetedText.Replace("whois", string.Empty).TrimPunctuation().Trim();

			//var characters = _Marvel.GetCharacters(Name: marvelCharacterStr);

			//var marvelCharacter = characters.FirstOrDefault();

			//var response = string.Empty;

			//if (marvelCharacter != null)
			//{
			//	response = $"Ah, {marvelCharacter.Name}! {marvelCharacter.Description}.{Environment.NewLine}{Environment.NewLine}";
			//	response += $"{Environment.NewLine}{Environment.NewLine}";
			//	response += $"{marvelCharacter.Name} has appeared in {marvelCharacter.Comics.Available} comics!";
			//	response += $"{Environment.NewLine}{Environment.NewLine}";
			//	response += $"Learn more here: {marvelCharacter.Urls.FirstOrDefault(x => x.Type == "detail")?.Url ?? "... or not."}";
			//}
			//else
			//{
			//	response = $"I wasn't able to find anything about {marvelCharacterStr}...";
			//}

			yield return message.ReplyToChannel("Check back...");
		}
	}
}
