using MediatR;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Query
{
    public class ObterResponsavelQuery : IRequest<RequestResponseWrapper<ResponsavelDto>>
    {
        public int? Id { get; set; }
    }
}