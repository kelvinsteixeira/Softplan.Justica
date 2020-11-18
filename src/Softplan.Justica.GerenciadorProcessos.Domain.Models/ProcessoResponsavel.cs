namespace Softplan.Justica.GerenciadorProcessos.Domain.Models
{
    public class ProcessoResponsavel
    {
        public int ProcessoId { get; set; }

        public Processo Processo { get; set; }

        public int ResponsavelId { get; set; }

        public Responsavel Responsavel { get; set; }
    }
}