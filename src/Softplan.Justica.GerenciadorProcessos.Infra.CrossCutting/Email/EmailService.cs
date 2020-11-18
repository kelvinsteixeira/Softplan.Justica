using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailServiceSettings emailServiceSettings;

        public EmailService(IOptions<EmailServiceSettings> emailServiceSettings)
        {
            this.emailServiceSettings = emailServiceSettings.Value;
        }

        public void SendEmail(EmailMessage emailMessage)
        {
            // Submit Email

            //using (var smtp = new SmtpClient())
            //{
            //    smtp.Connect(this.emailServiceSettings.SmtpAddress, this.emailServiceSettings.Port, SecureSocketOptions.StartTls);
            //    smtp.Authenticate(this.emailServiceSettings.Username, this.emailServiceSettings.Password);
            //    smtp.Send(emailMessage.CreateMimeMessage());
            //    smtp.Disconnect(true);
            //}
        }
    }
}