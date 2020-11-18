using Microsoft.AspNetCore.Mvc;

namespace Softplan.Justica.GerenciadorProcessos.Handlers.Interfaces
{
    public interface IApiResultHandler
    {
        IActionResult Handle(ControllerBase controller, object result = default);
    }
}