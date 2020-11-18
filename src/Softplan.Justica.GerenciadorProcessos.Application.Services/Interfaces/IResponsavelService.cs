using System.Collections.Generic;
using System.Threading.Tasks;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;

namespace Softplan.Justica.GerenciadorProcessos.Application.Services.Interfaces
{
    public interface IResponsavelService
    {
        Task<int?> CriarResponsavelAsync(CriarResponsavelCommand command);

        Task AtualizarResponsavelAsync(AtualizarResponsavelCommand command);

        Task RemoverResponsavelAsync(RemoverResponsavelCommand command);

        Task<PagedList<ResponsavelDto>> ObterResponsaveisAsync(ObterResponsaveisQuery query);

        Task<ResponsavelDto> ObterResponsavelAsync(ObterResponsavelQuery query);
    }
}