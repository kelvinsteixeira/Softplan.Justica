using System.Collections.Generic;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Command
{
    public class AtualizarProcessoResponse
    {
        public AtualizarProcessoResponse(int processoId, List<int> responsaveisAdicionadosIds)
        {
            this.ProcessoId = processoId;
            this.ResponsaveisAdicionadosIds = responsaveisAdicionadosIds;
        }

        public int ProcessoId { get; }

        public IReadOnlyCollection<int> ResponsaveisAdicionadosIds { get; }
    }
}