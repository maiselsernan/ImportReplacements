using System.Threading.Tasks;

namespace ImportReplacement.Api.Repositories.Interfaces
{
    public interface IAuthenticationRepository
    {
        bool Authenticate(string password);
    }
}
