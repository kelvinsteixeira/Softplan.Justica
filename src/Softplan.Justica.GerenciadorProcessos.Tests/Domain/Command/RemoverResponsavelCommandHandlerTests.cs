using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;
using Xunit;

namespace Softplan.Justica.GerenciadorResponsavels.Tests.Domain.Command
{
    public class RemoverResponsavelCommandHandlerTests
    {
        private const int ProcessId = 1;

        private Mock<IResponsavelRepository> mockResponsavelRepository;
        private Mock<INotificationContext> mockNotificationContext;
        private Mock<IRemoverResponsavelCommandValidator> mockValidator;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<ILogger<RemoverResponsavelCommandHandler>> mockLogger;

        private RemoverResponsavelCommand command;
        private RemoverResponsavelCommandHandler sut;

        public RemoverResponsavelCommandHandlerTests()
        {
            this.mockResponsavelRepository = new Mock<IResponsavelRepository>();
            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockValidator = new Mock<IRemoverResponsavelCommandValidator>();
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockLogger = new Mock<ILogger<RemoverResponsavelCommandHandler>>();

            this.command = new RemoverResponsavelCommand { Id = ProcessId };

            this.sut = new RemoverResponsavelCommandHandler(
                this.mockResponsavelRepository.Object,
                this.mockNotificationContext.Object,
                this.mockValidator.Object,
                this.mockUnitOfWork.Object,
                this.mockLogger.Object);
        }

        [Fact]
        public void ValidarRemoverResponsavel()
        {
            // Arrange
            this.mockValidator
                .Setup(r => r.ValidateModelAsync(It.IsNotNull<RemoverResponsavelCommand>()))
                .Returns(Task.FromResult(true));

            this.mockResponsavelRepository
                .Setup(p => p.Remover(It.IsNotNull<int>()));

            // Act
            var response = sut.Handle(command, default);

            // Assert
            this.mockValidator.Verify(r => r.ValidateModelAsync(command), Times.Once);
            this.mockResponsavelRepository.Verify(r => r.Remover(ProcessId), Times.Once);
            response.Result.Success.Should().BeTrue();
        }

        [Fact]
        public async void ValidarFalhaValidacao()
        {
            // Arrange
            this.mockValidator
                .Setup(pv => pv.ValidateModelAsync(command))
                .Returns(Task.FromResult(false));

            // Act
            var response = await sut.Handle(command, default);

            // Assert
            this.mockValidator.Verify(pv => pv.ValidateModelAsync(command), Times.Once);
            response.Success.Should().BeFalse();
        }
    }
}