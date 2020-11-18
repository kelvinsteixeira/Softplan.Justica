using FluentAssertions;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.InfraCrossCutting.Email
{
    public class EmailMessageBuilderTests
    {
        private EmailMessageBuilder sut;

        public EmailMessageBuilderTests()
        {
            this.sut = new EmailMessageBuilder();
        }

        [Fact]
        public void ValidarCriacaoEmail()
        {
            // Arrange
            var from = "From";
            var subject = "Subject";
            var to1 = "To1";
            var to2 = "To2";
            var body = "Body";

            // Act
            var emailMessage = this.sut
                .From(from)
                .Subject(subject)
                .To(to1).To(to2)
                .Body(body)
                .Build();

            // Assert
            emailMessage.From.Should().Be(from);
            emailMessage.Subject.Should().Be(subject);
            emailMessage.To.Should().Equal(to1, to2);
            emailMessage.Body.Should().Be(body);
        }
    }
}