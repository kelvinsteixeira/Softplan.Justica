using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.Domain.Validators
{
    public class ObterProcessoQueryValidatorTests
    {
        private ObterProcessoQueryValidator sut;
        public Mock<INotificationContext> mockNotificationContext;
        private List<Notification> notifications;

        public ObterProcessoQueryValidatorTests()
        {
            this.notifications = new List<Notification>();

            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockNotificationContext
                .Setup(n => n.Add(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Callback<string, string>((key, value) =>
                {
                    this.notifications.Add(new Notification(key, value));
                });

            this.sut = new ObterProcessoQueryValidator(this.mockNotificationContext.Object);
        }

        [Fact]
        public async void ValidarValidator()
        {
            // Arrange
            var query = new ObterProcessoQuery { Id = 1 };

            // Act
            var result = await this.sut.ValidateModelAsync(query);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void ValidarFalhaValidacao()
        {
            // Arrange
            var query = new ObterProcessoQuery();

            // Act
            var result = await this.sut.ValidateModelAsync(query);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(notifications => notifications.Message == $"Id: {ErrorMessages.ErroVazio}");
        }
    }
}