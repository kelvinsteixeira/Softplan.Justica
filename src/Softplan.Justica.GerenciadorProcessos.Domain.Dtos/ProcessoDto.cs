using System;
using System.Collections.Generic;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Dtos
{
    public class ProcessoDto
    {
        public int? Id { get; set; }

        public string NumeroProcesso { get; set; }

        public DateTimeOffset? DataDistribuicao { get; set; }

        public bool SegredoJustica { get; set; }

        public string PastaFisicaCliente { get; set; }

        public string Descricao { get; set; }

        public int? SituacaoProcessoId { get; set; }

        public IEnumerable<ResponsavelDto> Responsaveis { get; set; }
    }
}