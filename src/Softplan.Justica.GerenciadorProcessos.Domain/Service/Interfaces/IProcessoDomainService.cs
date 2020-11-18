using Softplan.Justica.GerenciadorProcessos.Domain.Models;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Service.Interfaces
{
    public interface IProcessoDomainService
    {
        bool ValidarHierarquiaQuantidade(Processo processo);

        bool ValidarNaoExistenteNaHierarquia(Processo processo);
    }
}