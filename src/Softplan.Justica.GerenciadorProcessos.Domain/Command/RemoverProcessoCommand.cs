using MediatR;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Command
{
    public class RemoverProcessoCommand : IRequest<RequestResponseWrapper<RemoverResponsavelResponse>>
    {
        public int? Id { get; set; }
    }
}