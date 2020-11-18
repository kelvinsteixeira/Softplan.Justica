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
    public class RemoverProcessoCommandHandlerTests
    {
        private const int ProcessId = 1;

        private Mock<IProcessoRepository> mockProcessoRepository;
        private Mock<INotificationContext> mockNotificationContext;
        private Mock<IRemoverProcessoCommandValidator> mockValidator;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<ILogger<RemoverProcessoCommandHandler>> mockLogger;

        private RemoverProcessoCommand command;
        private RemoverProcessoCommandHandler sut;

        public RemoverProcessoCommandHandlerTests()
        {
            this.mockProcessoRepository = new Mock<IProcessoRepository>();
            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockValidator = new Mock<IRemoverProcessoCommandValidator>();
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockLogger = new Mock<ILogger<RemoverProcessoCommandHandler>>();

            this.command = new RemoverProcessoCommand { Id = ProcessId };

            this.sut = new RemoverProcessoCommandHandler(
                this.mockProcessoRepository.Object,
                this.mockNotificationContext.Object,
                this.mockValidator.Object,
                this.mockUnitOfWork.Object,
                this.mockLogger.Object);
        }

        [Fact]
        public void ValidarRemoverProcesso()
        {
            // Arrange
            var processo = new Processo { NumeroProcesso = "123" };

            this.mockValidator
                .Setup(r => r.ValidateModelAsync(It.IsNotNull<RemoverProcessoCommand>()))
                .Returns(Task.FromResult(true));

            this.mockProcessoRepository
                .Setup(p => p.Remover(It.IsNotNull<int>()));

            this.mockProcessoRepository
                .Setup(p => p.ObterPorId(It.IsNotNull<int>()))
                .Returns(processo);

            // Act
            var response = sut.Handle(command, default);

            // Assert
            response.Result.Success.Should().BeTrue();
            this.mockValidator.Verify(r => r.ValidateModelAsync(command), Times.Once);
            this.mockProcessoRepository.Verify(r => r.Remover(ProcessId), Times.Once);
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