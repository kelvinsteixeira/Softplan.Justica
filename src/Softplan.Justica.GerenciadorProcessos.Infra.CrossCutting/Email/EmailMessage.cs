using System.Collections.Generic;
using System.Linq;
using MimeKit;
using MimeKit.Text;

namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email
{
    public class EmailMessage
    {
        internal EmailMessage()
        {
            this.InternalTo = new List<string>();
        }

        public string From { get; internal set; }

        public IReadOnlyCollection<string> To => this.InternalTo;

        public string Subject { get; internal set; }

        public string Body { get; internal set; }

        internal List<string> InternalTo { get; private set; }

        internal MimeMessage CreateMimeMessage()
        {
            var mimeMessage = new MimeMessage
            {
                Sender = MailboxAddress.Parse(this.From),
                Subject = this.Subject,
                Body = new TextPart(TextFormat.Plain) { Text = this.Body }
            };

            mimeMessage.To.AddRange(this.To.Select(to => MailboxAddress.Parse(to)));

            return mimeMessage;
        }
    }
}