using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.Domain.Validators
{
    public class RemoverProcessoCommandValidatorTests : ValidatorTestDataBase
    {
        private RemoverProcessoCommandValidator sut;
        public Mock<INotificationContext> mockNotificationContext;
        private List<Notification> notifications;

        public RemoverProcessoCommandValidatorTests()
        {
            this.notifications = new List<Notification>();

            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockNotificationContext
                .Setup(n => n.Add(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Callback<string, string>((key, value) =>
                {
                    this.notifications.Add(new Notification(key, value));
                });

            this.sut = new RemoverProcessoCommandValidator(this.MockProcessoRepository.Object, this.mockNotificationContext.Object);
        }

        [Fact]
        public async void ValidarValidator()
        {
            // Arrange
            var command = new RemoverProcessoCommand { Id = 1 };

            this.MockProcessoRepository
                .Setup(pr => pr.ObterPorId(1))
                .Returns(this.Processo1);

            this.MockSituacaoProcessoRepository
                .Setup(sp => sp.ObterPorId(1))
                .Returns(this.SituacaoEmAndamento);

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void ValidarFalhaIdVazio()
        {
            // Arrange
            var command = new RemoverProcessoCommand();

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(n => n.Message == $"Id: {ErrorMessages.ErroVazio}").Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaProcessoNaoExiste()
        {
            // Arrange
            var command = new RemoverProcessoCommand { Id = 1 };

            this.MockProcessoRepository
                .Setup(pr => pr.ObterPorId(1))
                .Returns((Processo)null);

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(n => n.Message == string.Format(ErrorMessages.ProcessoNaoEcontrado, command.Id)).Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaSituacaoFinalizado()
        {
            // Arrange
            var command = new RemoverProcessoCommand { Id = 1 };

            this.MockProcessoRepository
                .Setup(pr => pr.ObterPorId(1))
                .Returns(this.Processo2);

            this.MockSituacaoProcessoRepository
                .Setup(sp => sp.ObterPorId(2))
                .Returns(this.SituacaoArquivado);

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(n => n.Message == string.Format(ErrorMessages.ProcessoFinalizado, command.Id)).Should().NotBeNull();
        }
    }
}