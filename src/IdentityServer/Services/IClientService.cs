using System.Threading.Tasks;
using IdentityServer.Models.Client;

namespace IdentityServer.Services
{
    public interface IClientService
    {
        Task UpsertClient(ClientInputModel inputModel);
    }
}