namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email.Interfaces
{
    public interface IEmailMessageBuilder
    {
        EmailMessageBuilder From(string from);

        EmailMessageBuilder To(string to);

        EmailMessageBuilder Subject(string subject);

        EmailMessageBuilder Body(string body);

        EmailMessage Build();
    }
}