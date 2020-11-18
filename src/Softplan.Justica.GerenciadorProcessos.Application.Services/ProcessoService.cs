using System.Threading.Tasks;
using MediatR;
using Softplan.Justica.GerenciadorProcessos.Application.Services.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;
using Softplan.Justica.GerenciadorProcessos.Domain.Events;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;

namespace Softplan.Justica.GerenciadorProcessos.Application.Services
{
    public class ProcessoService : IProcessoService
    {
        private readonly IMediator mediator;

        public ProcessoService(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task AtualizarProcessoAsync(AtualizarProcessoCommand command)
        {
            var response = await this.mediator.Send(command, default);
            if (response.Success && response.Value != null)
            {
                await this.mediator.Send(new ProcessoAtualizadoEvent(response.Value.ProcessoId, response.Value.ResponsaveisAdicionadosIds));
            }
        }

        public async Task<int?> CriarProcessoAsync(CriarProcessoCommand command)
        {
            var response = await this.mediator.Send(command, default);

            if (response.Success && response.Value.HasValue)
            {
                await this.mediator.Send(new ProcessoCriadoEvent(response.Value.Value));
            }

            return response.Value;
        }

        public async Task<ProcessoDto> ObterProcessoAsync(ObterProcessoQuery query)
        {
            var response = await this.mediator.Send(query, default);
            return response.Value;
        }

        public async Task<PagedList<ProcessoDto>> ObterProcessosAsync(ObterProcessosQuery query)
        {
            var result = await this.mediator.Send(query, default);
            return result.Value;
        }

        public async Task RemoverProcessoAsync(RemoverProcessoCommand command)
        {
            var response = await this.mediator.Send(command, default);
            if (response.Success && response.Value != null)
            {
                await this.mediator.Send(new ProcessoRemovidoEvent(response.Value.NumeroProcesso, response.Value.ResponsaveisIds));
            }
        }
    }
}