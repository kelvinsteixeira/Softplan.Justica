using Softplan.Justica.GerenciadorProcessos.Domain.Models;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Repository
{
    public interface ISituacaoProcessoRepository
    {
        SituacaoProcesso ObterPorId(int id);
    }
}