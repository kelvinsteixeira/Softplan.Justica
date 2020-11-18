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
    public class ResponsavelController : ApiControllerBase
    {
        public const string ObterResponsavelRouteName = "ObterResponsavel";

        private readonly IResponsavelService responsavelService;

        public ResponsavelController(IApiResultHandler apiResultHandler, IResponsavelService responsavelService, ILogger<ResponsavelController> logger) : base(apiResultHandler, logger)
        {
            this.responsavelService = responsavelService;
        }

        [HttpPut("{id}")]
        public Task<IActionResult> AtualizarResponsavelAsync(int? id, [FromBody] AtualizarResponsavelCommand command)
        {
            command.Id = id;
            return this.ExecAndHandleAsync(action: () => this.responsavelService.AtualizarResponsavelAsync(command));
        }

        [HttpPost]
        public Task<IActionResult> CriarResponsavelAsync([FromBody] CriarResponsavelCommand command)
        {
            return this.ExecAndHandleAsync(
                action: () => this.responsavelService.CriarResponsavelAsync(command),
                onSuccessActionResultHandler: (result) => this.CreatedAtRoute(ObterResponsavelRouteName, new { id = result.Value }, result));
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> RemoverResponsavelAsync(int? id)
        {
            return this.ExecAndHandleAsync(action: () => this.responsavelService.RemoverResponsavelAsync(new RemoverResponsavelCommand { Id = id }));
        }

        [HttpGet(Name = "ObterResponsaveis")]
        public Task<IActionResult> ObterResponsaveisAsync([FromBody] ObterResponsaveisQuery query, [FromQuery] ObterResponsaveisParameter parameters)
        {
            query.ConfigurarPaginacao(parameters.PageNumber, parameters.PageSize);

            return this.ExecAndHandleAsync(
                action: () => this.responsavelService.ObterResponsaveisAsync(query),
                onSuccessActionResultHandler: (result) =>
                {
                    this.EnsurePaginationMetadata(result, "ObterResponsaveis");
                    return this.Ok(result);
                });
        }

        [HttpGet("{id}", Name = ObterResponsavelRouteName)]
        public Task<IActionResult> ObterResponsavelAsync(int? id)
        {
            return this.ExecAndHandleAsync(action: () => this.responsavelService.ObterResponsavelAsync(new ObterResponsavelQuery { Id = id }));
        }
    }
}