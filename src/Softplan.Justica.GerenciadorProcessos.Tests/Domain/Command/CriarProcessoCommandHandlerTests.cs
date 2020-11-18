using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public class CriarProcessoCommandHandlerTests
    {
        private Mock<IProcessoRepository> mockProcessoRepository;
        private Mock<IResponsavelRepository> mockResponsavelRepository;
        private Mock<ICriarProcessoCommandValidator> mockValidator;
        private Mock<INotificationContext> mockNotificationContext;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<ILogger<CriarProcessoCommandHandler>> mockLogger;

        private CriarProcessoCommand command;
        private CriarProcessoCommandHandler sut;

        public CriarProcessoCommandHandlerTests()
        {
            this.mockProcessoRepository = new Mock<IProcessoRepository>();
            this.mockResponsavelRepository = new Mock<IResponsavelRepository>();
            this.mockValidator = new Mock<ICriarProcessoCommandValidator>();
            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockLogger = new Mock<ILogger<CriarProcessoCommandHandler>>();

            this.command = new CriarProcessoCommand
            {
                NumeroProcesso = "NumProcesso",
                DataDistribuicao = DateTimeOffset.Now,
                SegredoJustica = true,
                PastaFisicaCliente = "PastaFisicaCliente",
                Descricao = "Descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1, 2, 3 },
                ProcessoVinculadoId = 1
            };

            this.sut = new CriarProcessoCommandHandler(
                this.mockProcessoRepository.Object,
                this.mockResponsavelRepository.Object,
                this.mockValidator.Object,
                this.mockNotificationContext.Object,
                this.mockUnitOfWork.Object,
                this.mockLogger.Object);
        }

        [Fact]
        public async void ValidarCriarProcesso()
        {
            // Arrange
            this.mockProcessoRepository
                .Setup(pr => pr.Criar(It.IsNotNull<Processo>()));

            this.mockProcessoRepository
                .Setup(pr => pr.ObterPorId(It.IsNotNull<int>()))
                .Returns((Processo)null);

            this.mockResponsavelRepository
                .Setup(pr => pr.Obter(It.IsAny<Expression<Func<Responsavel, bool>>>()))
                .Returns(new List<Responsavel>());

            this.mockValidator
                .Setup(pv => pv.ValidateModelAsync(command))
                .Returns(Task.FromResult(true));

            // Act
            var response = await sut.Handle(command, default);

            // Assert
            mockValidator.Verify(pv => pv.ValidateModelAsync(command), Times.Once);
            mockProcessoRepository.Verify(pr => pr.Criar(It.IsNotNull<Processo>()), Times.Once);
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
        }

        [Fact]
        public async void ValidarFalhaValidacao()
        {
            // Arrange
            mockValidator
                .Setup(pv => pv.ValidateModelAsync(command))
                .Returns(Task.FromResult(false));

            // Act
            var response = await sut.Handle(command, default);

            // Assert
            mockValidator.Verify(pv => pv.ValidateModelAsync(command), Times.Once);
            response.Success.Should().BeFalse();
        }
    }
}