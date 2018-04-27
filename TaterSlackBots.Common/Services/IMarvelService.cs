using System.Threading.Tasks;
using MarvelSharp.Model;

namespace TaterSlackBots.Common.Services
{
    public interface IMarvelService
    {
	    Task<Character> GetCharacter(string name);
    }
}
