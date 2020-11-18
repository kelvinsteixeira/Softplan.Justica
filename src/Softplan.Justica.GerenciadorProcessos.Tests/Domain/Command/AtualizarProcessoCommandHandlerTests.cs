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
    public class AtualizarProcessoCommandHandlerTests
    {
        private readonly int? ProcessoPaiId = 1;
        private readonly int? ProcessoId = 2;

        private Mock<IProcessoRepository> mockProcessoRepository;
        private Mock<IResponsavelRepository> mockResponsavelRepository;
        private Mock<IAtualizarProcessoCommandValidator> mockValidator;
        private Mock<INotificationContext> mockNotificationContext;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<ILogger<AtualizarProcessoCommandHandler>> mockLogger;

        private AtualizarProcessoCommand command;
        private AtualizarProcessoCommandHandler sut;

        private Processo processoPai;
        private Processo processo;
        private List<Responsavel> responsaveis;
        private int situacaoProcessoId = 1;

        public AtualizarProcessoCommandHandlerTests()
        {
            this.mockProcessoRepository = new Mock<IProcessoRepository>();
            this.mockResponsavelRepository = new Mock<IResponsavelRepository>();
            this.mockValidator = new Mock<IAtualizarProcessoCommandValidator>();
            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockLogger = new Mock<ILogger<AtualizarProcessoCommandHandler>>();

            this.command = new AtualizarProcessoCommand
            {
                NumeroProcesso = "123",
                DataDistribuicao = DateTimeOffset.Now,
                SegredoJustica = true,
                PastaFisicaCliente = "pastaCliente",
                Descricao = "descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1, 2 },
                ProcessoVinculadoId = 1
            };

            this.responsaveis = new List<Responsavel>
            {
                new Responsavel
                {
                    Id = 1,
                    Cpf = "Cpf",
                    Email = "Email",
                    Foto = new byte[1],
                    Nome = "Nome"
                },
                new Responsavel
                {
                    Id = 2,
                    Cpf = "Cpf2",
                    Email = "Email2",
                    Foto = new byte[1],
                    Nome = "Nome2"
                }
            };

            this.processoPai = new Processo
            {
                Id = ProcessoPaiId.Value,
                DataDistribuicao = DateTimeOffset.Now,
                Descricao = "PaiDescricao",
                NumeroProcesso = "PaiNumProcesso",
                PastaFisicaCliente = "PaiPastaFisicaCliente",
                SegredoJustica = true,
            };

            this.processo = new Processo
            {
                Id = ProcessoId.Value,
                DataDistribuicao = DateTimeOffset.Now,
                Descricao = "Descricao",
                NumeroProcesso = "NumProcesso",
                PastaFisicaCliente = "PastaFisicaCliente",
                ProcessoVinculado = processoPai,
                ProcessoVinculadoId = processoPai.Id,
                SegredoJustica = true,
                SituacaoId = situacaoProcessoId
            };

            this.processo.AtribuirResponsaveis(responsaveis);

            this.sut = new AtualizarProcessoCommandHandler(
                this.mockProcessoRepository.Object,
                this.mockResponsavelRepository.Object,
                this.mockValidator.Object,
                this.mockNotificationContext.Object,
                this.mockUnitOfWork.Object,
                this.mockLogger.Object);
        }

        [Fact]
        public async void ValidarAtualizarProcesso()
        {
            // Arrange
            mockProcessoRepository
                .Setup(pr => pr.Atualizar(It.IsNotNull<Processo>()));

            mockProcessoRepository
                .Setup(pr => pr.ObterPorId(It.IsNotNull<int>()))
                .Returns(processo);

            mockResponsavelRepository
                .Setup(pr => pr.Obter(It.IsAny<Expression<Func<Responsavel, bool>>>()))
                .Returns(new List<Responsavel>());

            mockValidator
                .Setup(pv => pv.ValidateModelAsync(command))
                .Returns(Task.FromResult(true));

            command.Id = ProcessoId;

            // Act
            var response = await sut.Handle(command, default);

            // Assert
            mockValidator.Verify(pv => pv.ValidateModelAsync(command), Times.Once);
            mockProcessoRepository.Verify(pr => pr.Atualizar(It.IsNotNull<Processo>()), Times.Once);
            response.Should().NotBeNull();
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

        [Fact]
        public async void ValidarRetornoResponsaveisAdicionados()
        {
            // Arrange
            command.ResponsaveisIds.AddRange(new int[] { 3, 4, 5 });

            mockProcessoRepository
                .Setup(pr => pr.Atualizar(It.IsNotNull<Processo>()));

            mockProcessoRepository
                .Setup(pr => pr.ObterPorId(It.IsNotNull<int>()))
                .Returns(processo);

            mockResponsavelRepository
                .Setup(pr => pr.Obter(It.IsAny<Expression<Func<Responsavel, bool>>>()))
                .Returns(new List<Responsavel>());

            mockValidator
                .Setup(pv => pv.ValidateModelAsync(command))
                .Returns(Task.FromResult(true));

            command.Id = ProcessoId;

            // Act
            var response = await sut.Handle(command, default);

            // Assert
            response.Value.ResponsaveisAdicionadosIds.Should().ContainSingle(x => x == 3);
        }
    }
}