using System.Threading.Tasks;
using MarvelousApi.Model;

namespace TaterSlackBots.Common.Services
{
    public interface IMarvelService
    {
	    Task<Character> GetCharacter(string name);
    }
}
