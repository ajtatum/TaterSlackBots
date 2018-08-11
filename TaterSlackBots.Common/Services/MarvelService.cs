using System.Linq;
using System.Threading.Tasks;
using MarvelousApi;
using MarvelousApi.Criteria;
using MarvelousApi.Model;
using Serilog;
using TaterSlackBots.Common.Settings;

namespace TaterSlackBots.Common.Services
{
	public class MarvelService : IMarvelService
	{
		private readonly ILogger _log;
		private readonly IAppSettings _appSettings;

		private static ApiService ApiService { get; set; }

		public MarvelService(IAppSettings appSettings, ILogger log)
		{
			_appSettings = appSettings;
			_log = log.ForContext<MarvelService>();

			ApiService = new ApiService(_appSettings.Marvel.PublicKey, _appSettings.Marvel.PrivateKey);

			_log.Information("Marvel Service Connected");
		}

		public async Task<Character> GetCharacter(string name)
		{
			_log.Information($"Searching for {name}.");

			var nameSearch = new CharacterCriteria()
			{
				Name = name
			};

			var response = await ApiService.GetAllCharactersAsync(5, 0, nameSearch);

			if (response.Success)
				return response.Data.Result.FirstOrDefault();

			_log.Error(response.Code);
			return null;
		}
	}
}
