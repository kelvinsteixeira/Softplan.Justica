using MediatR;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Query
{
    public class ObterProcessoQuery : IRequest<RequestResponseWrapper<ProcessoDto>>
    {
        public int? Id { get; set; }
    }
}