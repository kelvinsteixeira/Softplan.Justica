using System.Collections.Generic;
using System.Threading.Tasks;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;

namespace Softplan.Justica.GerenciadorProcessos.Application.Services.Interfaces
{
    public interface IProcessoService
    {
        Task<int?> CriarProcessoAsync(CriarProcessoCommand command);

        Task AtualizarProcessoAsync(AtualizarProcessoCommand atualizarProcessoCommand);

        Task RemoverProcessoAsync(RemoverProcessoCommand removerProcessoCommand);

        Task<PagedList<ProcessoDto>> ObterProcessosAsync(ObterProcessosQuery obterProcessosQuery);

        Task<ProcessoDto> ObterProcessoAsync(ObterProcessoQuery obterProcessoQuery);
    }
}