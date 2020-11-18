using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class CriarResponsavelCommandValidatorTests : ValidatorTestDataBase
    {
        private CriarResponsavelCommandValidator sut;
        public Mock<INotificationContext> mockNotificationContext;
        private List<Notification> notifications;

        public CriarResponsavelCommandValidatorTests()
        {
            this.notifications = new List<Notification>();

            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockNotificationContext
                .Setup(n => n.Add(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Callback<string, string>((key, value) =>
                {
                    this.notifications.Add(new Notification(key, value));
                });

            this.sut = new CriarResponsavelCommandValidator(this.MockResponsavelRepository.Object, this.mockNotificationContext.Object);
        }

        [Fact]
        public async void ValidarValidator()
        {
            // Arrange
            var command = new CriarResponsavelCommand
            {
                Nome = "Nome",
                Cpf = "227.793.300-74",
                Email = "mail@mail.com",
                Foto = new byte[1]
            };

            this.MockResponsavelRepository
                .Setup(rp => rp.Count(It.IsNotNull<Expression<Func<Responsavel, bool>>>()))
                .Returns(0);

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void ValidarFalhaCamposVazios()
        {
            // Arrange
            var command = new CriarResponsavelCommand
            {
                Nome = null,
                Cpf = null,
                Email = null,
                Foto = new byte[1]
            };

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(n => n.Message == $"Nome: {ErrorMessages.ErroVazio}").Should().NotBeNull();
            this.notifications.FirstOrDefault(n => n.Message == $"Cpf: {ErrorMessages.ErroVazio}").Should().NotBeNull();
            this.notifications.FirstOrDefault(n => n.Message == $"Email: {ErrorMessages.ErroVazio}").Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaTamanhoMaximo()
        {
            // Arrange
            var command = new CriarResponsavelCommand
            {
                Nome = this.Faker.Random.String(151),
                Cpf = "227.793.300-74",
                Email = this.Faker.Random.String(401),
                Foto = new byte[1]
            };

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(n => n.Message == $"Nome: {string.Format(ErrorMessages.ErroTamanhoMaximo, 150)}").Should().NotBeNull();
            this.notifications.FirstOrDefault(n => n.Message == $"Email: {string.Format(ErrorMessages.ErroTamanhoMaximo, 400)}").Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaCpfInvalido()
        {
            // Arrange
            var command = new CriarResponsavelCommand
            {
                Nome = "Nome",
                Cpf = "64585494823",
                Email = "mail@mail.com",
                Foto = new byte[1]
            };

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(n => n.Message == string.Format(ErrorMessages.CpfInvalido, command.Cpf)).Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaEmailInvalido()
        {
            // Arrange
            var command = new CriarResponsavelCommand
            {
                Nome = "Nome",
                Cpf = "227.793.300-74",
                Email = "ma@il@mail.com",
                Foto = new byte[1]
            };

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(n => n.Message == string.Format(ErrorMessages.EmailInvalido, command.Email)).Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaCpfEmUso()
        {
            // Arrange
            var command = new CriarResponsavelCommand
            {
                Nome = "Nome",
                Cpf = "227.793.300-74",
                Email = "mail@mail.com",
                Foto = new byte[1]
            };

            this.MockResponsavelRepository
                .Setup(rp => rp.ObterPorId(1))
                .Returns(this.Aline);

            this.MockResponsavelRepository
                .Setup(rp => rp.Count(It.IsNotNull<Expression<Func<Responsavel, bool>>>()))
                .Returns(1);

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.notifications.FirstOrDefault(notifications => notifications.Message == string.Format(ErrorMessages.CpfEmUso, command.Cpf)).Should().NotBeNull();
        }
    }
}