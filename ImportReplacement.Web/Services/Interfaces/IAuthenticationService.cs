using System.Threading.Tasks;

namespace ImportReplacement.Web.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> Authenticate(string password);
    }
}
