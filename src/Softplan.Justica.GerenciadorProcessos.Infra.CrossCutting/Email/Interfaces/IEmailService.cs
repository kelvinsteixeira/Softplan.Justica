namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(EmailMessage emailMessage);
    }
}