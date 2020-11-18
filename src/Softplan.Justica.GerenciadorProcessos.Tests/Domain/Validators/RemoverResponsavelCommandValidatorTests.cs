using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Tests.Domain.Validators;
using Xunit;

namespace Softplan.Justica.GerenciadorResponsavels.Tests.Domain.Validators
{
    public class RemoverResponsavelCommandValidatorTests : ValidatorTestDataBase
    {
        private RemoverResponsavelCommandValidator sut;
        public Mock<INotificationContext> mockNotificationContext;
        private List<Notification> notifications;

        public RemoverResponsavelCommandValidatorTests()
        {
            this.notifications = new List<Notification>();

            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockNotificationContext
                .Setup(n => n.Add(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Callback<string, string>((key, value) =>
                {
                    this.notifications.Add(new Notification(key, value));
                });

            this.sut = new RemoverResponsavelCommandValidator(this.MockResponsavelRepository.Object, this.mockNotificationContext.Object);
        }

        [Fact]
        public async void ValidarValidator()
        {
            // Arrange
            var command = new RemoverResponsavelCommand { Id = 1 };

            this.MockResponsavelRepository
                .Setup(pr => pr.ObterPorId(1))
                .Returns(this.UsuarioSemProcesso);

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
            var command = new RemoverResponsavelCommand();

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(n => n.Message == $"Id: {ErrorMessages.ErroVazio}").Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaResponsavelNaoExiste()
        {
            // Arrange
            var command = new RemoverResponsavelCommand { Id = 1 };

            this.MockResponsavelRepository
                .Setup(pr => pr.ObterPorId(1))
                .Returns((Responsavel)null);

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(n => n.Message == ErrorMessages.ResponsavelNaoEncontrado).Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaResponsavelVinculadoProcesso()
        {
            // Arrange
            var command = new RemoverResponsavelCommand { Id = 1 };

            this.MockResponsavelRepository
                .Setup(pr => pr.ObterPorId(1))
                .Returns(new Responsavel
                {
                    ProcessoResponsaveis = new List<ProcessoResponsavel>
                    {
                        new ProcessoResponsavel
                        {
                            Processo = new Processo { }
                        }
                    }
                });

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(n => n.Message == ErrorMessages.ResponsavelVinculadoProcesso).Should().NotBeNull();
        }

    }
}