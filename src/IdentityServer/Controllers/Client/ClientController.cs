using System.Threading.Tasks;
using IdentityServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Client
{
    public class ClientController : Controller
    {
        private readonly IClientService ClientService;

        public ClientController(IClientService clientService)
        {
            ClientService = clientService ?? throw new System.ArgumentNullException(nameof(clientService));
        }

        public async Task<IActionResult> Index()
        {
            var models = await ClientService.GetAllClientsAsync();
            return View(models);
        }
    }
}