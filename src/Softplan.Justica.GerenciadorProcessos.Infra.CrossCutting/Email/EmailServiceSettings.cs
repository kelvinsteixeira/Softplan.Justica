namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email
{
    public class EmailServiceSettings
    {
        public string SmtpAddress { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}