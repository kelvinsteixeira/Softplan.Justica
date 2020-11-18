using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Softplan.Justica.GerenciadorProcessos.Application.Services.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Handlers.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Parameters;

namespace Softplan.Justica.GerenciadorProcessos.Controllers
{
    public class ProcessoController : ApiControllerBase
    {
        public const string ObterProcessoRouteName = "ObterProcesso";
        public const string ObterProcessosRouteName = "ObterProcessos";

        private readonly IProcessoService processoService;

        public ProcessoController(IApiResultHandler apiResultHandler, IProcessoService processoService, ILogger<ProcessoController> logger) : base(apiResultHandler, logger)
        {
            this.processoService = processoService;
        }

        [HttpPost]
        public Task<IActionResult> CriarProcessoAsync([FromBody] CriarProcessoCommand command)
        {
            return this.ExecAndHandleAsync(
                action: () => this.processoService.CriarProcessoAsync(command),
                onSuccessActionResultHandler: (result) => this.CreatedAtRoute(ObterProcessoRouteName, new { id = result.Value }, result));
        }

        [HttpPut("{id}")]
        public Task<IActionResult> AtualizarProcessoAsync(int? id, [FromBody] AtualizarProcessoCommand command)
        {
            command.Id = id;
            return this.ExecAndHandleAsync(action: () => this.processoService.AtualizarProcessoAsync(command));
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> RemoverProcessoAsync(int? id)
        {
            return this.ExecAndHandleAsync(action: () => this.processoService.RemoverProcessoAsync(new RemoverProcessoCommand { Id = id }));
        }

        [HttpGet("{id}", Name = ObterProcessoRouteName)]
        public Task<IActionResult> ObterProcessoAsync(int? id)
        {
            return this.ExecAndHandleAsync(action: () => this.processoService.ObterProcessoAsync(new ObterProcessoQuery { Id = id }));
        }

        [HttpGet(Name = ObterProcessosRouteName)]
        public Task<IActionResult> ObterProcessosAsync([FromBody] ObterProcessosQuery query, [FromQuery] ObterProcessosParameter parameters)
        {
            query.ConfigurarPaginacao(parameters.PageNumber, parameters.PageSize);

            return this.ExecAndHandleAsync(
                action: () => this.processoService.ObterProcessosAsync(query),
                onSuccessActionResultHandler: (result) =>
                {
                    this.EnsurePaginationMetadata(result, ObterProcessosRouteName);
                    return this.Ok(result);
                });
        }
    }
}