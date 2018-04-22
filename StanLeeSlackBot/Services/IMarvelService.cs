using System.Threading.Tasks;
using MarvelSharp.Model;

namespace StanLeeSlackBot.Services
{
    public interface IMarvelService
    {
	    Task<Character> GetCharacter(string name);
    }
}
