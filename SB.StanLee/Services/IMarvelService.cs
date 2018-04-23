using System.Threading.Tasks;
using MarvelSharp.Model;

namespace SB.StanLee.Services
{
    public interface IMarvelService
    {
	    Task<Character> GetCharacter(string name);
    }
}
