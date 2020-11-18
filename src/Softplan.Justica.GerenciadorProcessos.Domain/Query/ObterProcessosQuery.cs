using System;
using MediatR;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Query
{
    public class ObterProcessosQuery : IRequest<RequestResponseWrapper<PagedList<ProcessoDto>>>
    {
        public string NumeroProcesso { get; set; }

        public DateTimeOffset? DataDistribuicaoInicio { get; set; }

        public DateTimeOffset? DataDistribuicaoFim { get; set; }

        public bool? SegredoJustica { get; set; }

        public string PastaFisicaCliente { get; set; }

        public int? SituacaoProcessoId { get; set; }

        public string NomeResponsavel { get; set; }

        public int PageSize { get; private set; }

        public int PageNumber { get; private set; }

        public void ConfigurarPaginacao(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }
    }
}