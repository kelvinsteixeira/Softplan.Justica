using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.Domain.Command
{
    public class CriarResponsavelCommandHandlerTests
    {
        private Mock<IResponsavelRepository> mockResponsavelRepository;
        private Mock<INotificationContext> mockNotificationContext;
        private Mock<ICriarResponsavelCommandValidator> mockValidator;
        private Mock<ILogger<CriarResponsavelCommandHandler>> mockLogger;

        private Mock<IUnitOfWork> mockUnitOfWork;
        private CriarResponsavelCommandHandler sut;
        private CriarResponsavelCommand command;

        public CriarResponsavelCommandHandlerTests()
        {
            this.mockResponsavelRepository = new Mock<IResponsavelRepository>();
            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockValidator = new Mock<ICriarResponsavelCommandValidator>();
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockLogger = new Mock<ILogger<CriarResponsavelCommandHandler>>();

            this.command = new CriarResponsavelCommand
            {
                Nome = "Kelvin",
                Cpf = "602.346.080-13",
                Email = "mail@mail.com",
                Foto = new byte[1]
            };

            this.sut = new CriarResponsavelCommandHandler(
                this.mockResponsavelRepository.Object,
                this.mockNotificationContext.Object,
                this.mockValidator.Object,
                this.mockUnitOfWork.Object,
                this.mockLogger.Object);
        }

        [Fact]
        public async void ValidarCriarResponsavel()
        {
            // Arrange
            this.mockResponsavelRepository
                .Setup(r => r.Criar(It.IsNotNull<Responsavel>()));

            this.mockValidator
                .Setup(v => v.ValidateModelAsync(It.IsNotNull<CriarResponsavelCommand>()))
                .Returns(Task.FromResult(true));

            // Act
            var response = await sut.Handle(command, default);

            // Assert
            this.mockValidator.Verify(v => v.ValidateModelAsync(command), Times.Once);
            this.mockResponsavelRepository.Verify(r => r.Criar(It.IsNotNull<Responsavel>()), Times.Once);
            response.Success.Should().BeTrue();
        }

        [Fact]
        public async void ValidarFalhaValidacao()
        {
            // Arrange
            this.mockValidator
                .Setup(v => v.ValidateModelAsync(It.IsNotNull<CriarResponsavelCommand>()))
                .Returns(Task.FromResult(false));

            // Act
            var response = await sut.Handle(command, default);

            // Assert
            this.mockValidator.Verify(v => v.ValidateModelAsync(command), Times.Once);
            response.Success.Should().BeFalse();
        }
    }
}
