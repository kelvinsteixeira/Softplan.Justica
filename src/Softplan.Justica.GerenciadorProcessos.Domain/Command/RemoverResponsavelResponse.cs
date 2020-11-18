namespace Softplan.Justica.GerenciadorProcessos.Domain.Command
{
    public class RemoverResponsavelResponse
    {
        public string NumeroProcesso { get; set; }

        public int[] ResponsaveisIds { get; set; }
    }
}