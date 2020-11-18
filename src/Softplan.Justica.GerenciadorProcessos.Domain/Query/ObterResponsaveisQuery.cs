using MediatR;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Query
{
    public class ObterResponsaveisQuery : IRequest<RequestResponseWrapper<PagedList<ResponsavelDto>>>
    {
        public string Nome { get; set; }

        public string Cpf { get; set; }

        public string NumeroProcesso { get; set; }

        public int PageSize { get; private set; }

        public int PageNumber { get; private set; }

        public void ConfigurarPaginacao(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }
    }
}