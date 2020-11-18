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
    public class AtualizarResponsavelCommandHandlerTests
    {
        private Mock<IResponsavelRepository> mockResponsavelRepository;
        private Mock<INotificationContext> mockNotificationContext;
        private Mock<IAtualizarResponsavelCommandValidator> mockValidator;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<ILogger<AtualizarResponsavelCommandHandler>> mockLogger;

        private AtualizarResponsavelCommandHandler sut;
        private AtualizarResponsavelCommand command;

        public AtualizarResponsavelCommandHandlerTests()
        {
            this.mockResponsavelRepository = new Mock<IResponsavelRepository>();
            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockValidator = new Mock<IAtualizarResponsavelCommandValidator>();
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockLogger = new Mock<ILogger<AtualizarResponsavelCommandHandler>>();

            this.command = new AtualizarResponsavelCommand
            {
                Id = 1,
                Nome = "Kelvin",
                Cpf = "602.346.080-13",
                Email = "mail@mail.com",
                Foto = new byte[1]
            };

            this.sut = new AtualizarResponsavelCommandHandler(
                this.mockResponsavelRepository.Object,
                this.mockNotificationContext.Object,
                this.mockValidator.Object,
                this.mockUnitOfWork.Object,
                this.mockLogger.Object);
        }

        [Fact]
        public async void ValidarAtualizarResponsavel()
        {
            // Arrange
            var responsavel = new Responsavel { Id = 1, Nome = "Kelvin", Cpf = "602.346.080-13", Email = "mail@mail.com", Foto = new byte[1] };

            mockResponsavelRepository
                .Setup(r => r.ObterPorId(1))
                .Returns(responsavel);

            mockResponsavelRepository
                .Setup(r => r.Atualizar(It.IsNotNull<Responsavel>()));

            mockValidator
                .Setup(v => v.ValidateModelAsync(It.IsNotNull<AtualizarResponsavelCommand>()))
                .Returns(Task.FromResult(true));

            // Act
            var response = await sut.Handle(command, default);

            // Assert
            mockValidator.Verify(v => v.ValidateModelAsync(command), Times.Once);
            mockResponsavelRepository.Verify(r => r.Atualizar(It.IsNotNull<Responsavel>()), Times.Once);
            response.Success.Should().BeTrue();
        }

        [Fact]
        public async void ValidarFalhaValidacao()
        {
            // Arrange
            mockValidator
                .Setup(v => v.ValidateModelAsync(It.IsNotNull<AtualizarResponsavelCommand>()))
                .Returns(Task.FromResult(false));

            // Act
            var response = await sut.Handle(command, default);

            // Assert
            mockValidator.Verify(v => v.ValidateModelAsync(command), Times.Once);
            response.Success.Should().BeFalse();
        }
    }
}