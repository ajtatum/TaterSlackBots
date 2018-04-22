using System.Linq;
using System.Threading.Tasks;
using MarvelSharp;
using MarvelSharp.Criteria;
using MarvelSharp.Model;
using Microsoft.Extensions.Options;
using Serilog;
using StanLeeSlackBot.Classes;

namespace StanLeeSlackBot.Services
{
	public class MarvelService : IMarvelService
	{
		private readonly AppSettings _appSettings;
		private readonly Serilog.ILogger _log;

		private static ApiService ApiService { get; set; }

		public MarvelService(IOptions<AppSettings> appSettings, Serilog.ILogger log)
		{
			_appSettings = appSettings.Value;
			_log = log;

			ApiService = new ApiService(_appSettings.Marvel.PublicKey, _appSettings.Marvel.PrivateKey);

			Log.Information("Marvel Service Connected");
		}

		public async Task<Character> GetCharacter(string name)
		{
			Log.Information($"Searching for {name}.");

			var nameSearch = new CharacterCriteria()
			{
				Name = name
			};

			var response = await ApiService.GetAllCharactersAsync(5, 0, nameSearch);

			if (response.Success)
				return response.Data.Result.FirstOrDefault();

			Log.Error(response.Code);
			return null;
		}
	}
}
