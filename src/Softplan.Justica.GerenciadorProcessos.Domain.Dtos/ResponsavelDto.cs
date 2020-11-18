namespace Softplan.Justica.GerenciadorProcessos.Domain.Dtos
{
    public class ResponsavelDto
    {
        public int? Id { get; set; }

        public string Nome { get; set; }

        public string Cpf { get; set; }

        public string Email { get; set; }

        public byte[] Foto { get; set; }
    }
}