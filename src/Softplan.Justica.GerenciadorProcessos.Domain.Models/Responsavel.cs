using System.Collections.Generic;
using System.Linq;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Models
{
    public class Responsavel : IEntity
    {
        public Responsavel()
        {
            this.ProcessoResponsaveis = new List<ProcessoResponsavel>();
        }

        public int Id { get; set; }

        public string Nome { get; set; }

        public string Cpf { get; set; }

        public string Email { get; set; }

        public byte[] Foto { get; set; }

        public ICollection<ProcessoResponsavel> ProcessoResponsaveis { get; set; }

        public ICollection<Processo> Processos => this.ProcessoResponsaveis?.Select(pr => pr.Processo).ToList();

        public void AtribuirProcessos(List<Processo> processos)
        {
            if (processos?.Any() == false)
            {
                return;
            }

            foreach (var processo in processos)
            {
                if (!this.ProcessoResponsaveis.Any(pr => pr.Processo.Id == processo.Id))
                {
                    this.ProcessoResponsaveis.Add(new ProcessoResponsavel { Processo = processo, Responsavel = this });
                }
            }
        }
    }
}