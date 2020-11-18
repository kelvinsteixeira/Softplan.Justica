using System;
using System.Collections.Generic;
using System.Linq;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Models
{
    public class Processo : IEntity
    {
        public Processo()
        {
            this.ProcessoResponsaveis = new List<ProcessoResponsavel>();
        }

        public int Id { get; set; }

        public string NumeroProcesso { get; set; }

        public DateTimeOffset? DataDistribuicao { get; set; }

        public bool SegredoJustica { get; set; }

        public string PastaFisicaCliente { get; set; }

        public string Descricao { get; set; }

        public int? ProcessoVinculadoId { get; set; }

        public Processo ProcessoVinculado { get; set; }

        public ICollection<ProcessoResponsavel> ProcessoResponsaveis { get; set; }

        public IReadOnlyCollection<Responsavel> Responsaveis => this.ProcessoResponsaveis?.Select(pr => pr.Responsavel).ToList();

        public int? SituacaoId { get; set; }

        public SituacaoProcesso Situacao { get; set; }

        public void AtribuirResponsaveis(List<Responsavel> responsaveis)
        {
            if (responsaveis?.Any() == false)
            {
                return;
            }

            foreach (var responsavel in responsaveis)
            {
                if (!this.ProcessoResponsaveis.Any(pr => pr.Responsavel.Id == responsavel.Id))
                {
                    this.ProcessoResponsaveis.Add(new ProcessoResponsavel { Processo = this, Responsavel = responsavel });
                }
            }
        }
    }
}
