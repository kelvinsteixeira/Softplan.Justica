using System;
using System.Collections.Generic;
using MediatR;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Command
{
    public class CriarProcessoCommand : IRequest<RequestResponseWrapper<int?>>
    {
        public string NumeroProcesso { get; set; }

        public DateTimeOffset? DataDistribuicao { get; set; }

        public bool SegredoJustica { get; set; }

        public string PastaFisicaCliente { get; set; }

        public string Descricao { get; set; }

        public int? SituacaoProcessoId { get; set; }

        public List<int> ResponsaveisIds { get; set; }

        public int? ProcessoVinculadoId { get; set; }
    }
}