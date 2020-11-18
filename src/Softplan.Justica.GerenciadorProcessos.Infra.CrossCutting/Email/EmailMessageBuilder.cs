using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email
{
    public class EmailMessageBuilder : IEmailMessageBuilder
    {
        private EmailMessage instance;

        public EmailMessageBuilder()
        {
            this.instance = new EmailMessage();
        }

        public EmailMessageBuilder From(string from)
        {
            this.instance.From = from;
            return this;
        }

        public EmailMessageBuilder To(string to)
        {
            this.instance.InternalTo.Add(to);
            return this;
        }

        public EmailMessageBuilder Subject(string subject)
        {
            this.instance.Subject = subject;
            return this;
        }

        public EmailMessageBuilder Body(string body)
        {
            this.instance.Body = body;
            return this;
        }

        public EmailMessage Build()
        {
            return this.instance;
        }
    }
}