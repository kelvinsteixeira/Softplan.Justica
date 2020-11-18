using System.Threading.Tasks;
using MediatR;
using Softplan.Justica.GerenciadorProcessos.Application.Services.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;

namespace Softplan.Justica.GerenciadorProcessos.Application.Services
{
    public class ResponsavelService : IResponsavelService
    {
        private readonly IMediator mediator;

        public ResponsavelService(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task AtualizarResponsavelAsync(AtualizarResponsavelCommand command)
        {
            await this.mediator.Send(command);
        }

        public async Task<int?> CriarResponsavelAsync(CriarResponsavelCommand command)
        {
            var response = await this.mediator.Send(command);
            return response.Value;
        }

        public async Task<PagedList<ResponsavelDto>> ObterResponsaveisAsync(ObterResponsaveisQuery query)
        {
            var result = await this.mediator.Send(query);
            return result.Value;
        }

        public async Task<ResponsavelDto> ObterResponsavelAsync(ObterResponsavelQuery query)
        {
            var result = await this.mediator.Send(query);
            return result.Value;
        }

        public async Task RemoverResponsavelAsync(RemoverResponsavelCommand command)
        {
            await this.mediator.Send(command);
        }
    }
}