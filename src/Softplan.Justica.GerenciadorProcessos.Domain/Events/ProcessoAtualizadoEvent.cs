using System.Collections.Generic;
using MediatR;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Events
{
    public class ProcessoAtualizadoEvent : IRequest
    {
        public ProcessoAtualizadoEvent(int processId, IReadOnlyCollection<int> responsaveisAdicionadosIds)
        {
            this.ProcessoId = processId;
            this.ResponsaveisAdicionadosIds = responsaveisAdicionadosIds;
        }

        public int ProcessoId { get; set; }

        public IReadOnlyCollection<int> ResponsaveisAdicionadosIds { get; }
    }
}