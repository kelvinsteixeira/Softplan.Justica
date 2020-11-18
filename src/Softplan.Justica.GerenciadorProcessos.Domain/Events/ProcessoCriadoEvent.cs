using MediatR;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Events
{
    public class ProcessoCriadoEvent : IRequest
    {
        public ProcessoCriadoEvent(int processoId)
        {
            this.ProcessoId = processoId;
        }

        public int ProcessoId { get; }
    }
}