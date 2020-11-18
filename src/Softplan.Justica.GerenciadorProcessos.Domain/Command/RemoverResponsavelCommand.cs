using MediatR;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Command
{
    public class RemoverResponsavelCommand : IRequest<RequestResponseWrapper>
    {
        public int? Id { get; set; }
    }
}