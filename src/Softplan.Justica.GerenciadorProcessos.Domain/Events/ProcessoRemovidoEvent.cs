using MediatR;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Events
{
    public class ProcessoRemovidoEvent : IRequest
    {
        public ProcessoRemovidoEvent(string numeroProcesso, int[] responsaveisIds)
        {
            this.NumeroProcesso = numeroProcesso;
            this.ResponsaveisIds = responsaveisIds;
        }

        public string NumeroProcesso { get; }

        public int[] ResponsaveisIds { get; }
    }
}