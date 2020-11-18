using System.Collections.Generic;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Models
{
    public class SituacaoProcesso : IEntity
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public bool Finalizado { get; set; }

        public ICollection<Processo> Processos { get; set; }
    }
}