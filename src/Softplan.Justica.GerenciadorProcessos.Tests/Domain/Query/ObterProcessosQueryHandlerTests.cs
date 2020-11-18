using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.Domain.Query
{
    public class ObterProcessosQueryHandlerTests
    {
        private Mock<IObterProcessosQueryValidator> mockValidator;
        private Mock<IProcessoRepository> mockProcessoRepository;
        private Mock<INotificationContext> mockNotificationContext;
        private Mock<ILogger<ObterProcessosQueryHandler>> mockLogger;

        private Responsavel respAline;
        private Responsavel respCalvin;
        private Responsavel respKelvin;
        private ObterProcessosQueryHandler sut;
        private List<Processo> processos;

        public ObterProcessosQueryHandlerTests()
        {
            this.mockValidator = new Mock<IObterProcessosQueryValidator>();
            this.mockProcessoRepository = new Mock<IProcessoRepository>();
            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockLogger = new Mock<ILogger<ObterProcessosQueryHandler>>();

            respAline = new Responsavel
            {
                Id = 1,
                Nome = "Aline Schorn"
            };

            respCalvin = new Responsavel
            {
                Id = 2,
                Nome = "Calvin Schmaltz"
            };

            respKelvin = new Responsavel
            {
                Id = 3,
                Nome = "Kelvin Teixeira"
            };

            var p1 = new Processo
            {
                Id = 1,
                DataDistribuicao = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Descricao = "Aqui uma descrição simples",
                NumeroProcesso = "1155668879",
                PastaFisicaCliente = "Alguma coisa deve ser colocada aqui"
            };

            p1.AtribuirResponsaveis(new List<Responsavel> { respKelvin });

            var p2 = new Processo
            {
                Id = 2,
                DataDistribuicao = new DateTimeOffset(2021, 10, 10, 0, 0, 0, TimeSpan.Zero),
                Descricao = "Sempre pensar antes de falar",
                NumeroProcesso = "9875158815",
                PastaFisicaCliente = "Em algum lugar muito além da escuridão do universo"
            };

            p2.AtribuirResponsaveis(new List<Responsavel> { respCalvin });

            var p3 = new Processo
            {
                Id = 3,
                DataDistribuicao = new DateTimeOffset(2022, 5, 5, 0, 0, 0, TimeSpan.Zero),
                Descricao = "Podemos parar por mim",
                NumeroProcesso = "57478185451",
                PastaFisicaCliente = "Enfim, tudo acabou"
            };

            p3.AtribuirResponsaveis(new List<Responsavel> { respAline, respKelvin });

            this.processos = new List<Processo> { p1, p2, p3 };

            this.sut = new ObterProcessosQueryHandler(
                this.mockProcessoRepository.Object,
                this.mockNotificationContext.Object,
                this.mockValidator.Object,
                this.mockLogger.Object);
        }

        [Fact]
        public async void ValidarObterProcessos()
        {
            // Arrange
            var query = new ObterProcessosQuery();
            query.ConfigurarPaginacao(1, 10);

            mockValidator
                .Setup(v => v.ValidateModelAsync(query))
                .Returns(Task.FromResult(true));

            mockProcessoRepository
                .Setup(p => p.Query())
                .Returns(processos.AsQueryable());

            // Act
            var response = await sut.Handle(query, default);

            // Assert
            response.Success.Should().BeTrue();
            response.Value.Should().HaveCount(this.processos.Count);
        }

        [Theory]
        [InlineData(1, 1, 1, 3, 3)]
        [InlineData(2, 2, 1, 2, 3)]
        [InlineData(1, 3, 3, 1, 3)]
        public async void ValidarRetornoPaginado(int pageNumber, int pageSize, int expectedItemsCount, int expectedTotalPages, int expectedTotalCount)
        {
            // Arrange
            var query = new ObterProcessosQuery();
            query.ConfigurarPaginacao(pageNumber, pageSize);

            mockValidator
                .Setup(v => v.ValidateModelAsync(query))
                .Returns(Task.FromResult(true));

            mockProcessoRepository
                .Setup(p => p.Query())
                .Returns(processos.AsQueryable());

            // Act
            var response = await sut.Handle(query, default);

            // Assert
            response.Success.Should().BeTrue();
            response.Value.Should().HaveCount(expectedItemsCount);
            response.Value.TotalPages.Should().Be(expectedTotalPages);
            response.Value.TotalCount.Should().Be(expectedTotalCount);
        }

        [Fact]
        public async void ValidarFiltroNumeroProcesso()
        {
            // Arrange
            var query = new ObterProcessosQuery { NumeroProcesso = "987515-88-15" };
            query.ConfigurarPaginacao(1, 10);

            mockValidator
                .Setup(v => v.ValidateModelAsync(query))
                .Returns(Task.FromResult(true));

            mockProcessoRepository
                .Setup(p => p.Query())
                .Returns(processos.AsQueryable());

            // Act
            var response = await sut.Handle(query, default);

            // Assert
            response.Success.Should().BeTrue();
            response.Value.Should().HaveCount(1);
            response.Value[0].Id.Should().Be(2);
        }

        [Fact]
        public async void ValidarFiltroData()
        {
            // Arrange
            var query = new ObterProcessosQuery
            {
                DataDistribuicaoInicio = new DateTimeOffset(2022, 4, 1, 0, 0, 0, TimeSpan.Zero),
                DataDistribuicaoFim = new DateTimeOffset(2022, 6, 1, 0, 0, 0, TimeSpan.Zero)
            };

            query.ConfigurarPaginacao(1, 10);

            mockValidator
                .Setup(v => v.ValidateModelAsync(query))
                .Returns(Task.FromResult(true));

            mockProcessoRepository
                .Setup(p => p.Query())
                .Returns(processos.AsQueryable());

            // Act
            var response = await sut.Handle(query, default);

            // Assert
            response.Success.Should().BeTrue();
            response.Value.Should().HaveCount(1);
            response.Value[0].Id.Should().Be(3);
        }

        [Fact]
        public async void ValidarFiltroNomeResponsavel()
        {
            // Arrange
            var query = new ObterProcessosQuery { NomeResponsavel = "kelvin" };
            query.ConfigurarPaginacao(1, 10);

            mockValidator
                .Setup(v => v.ValidateModelAsync(query))
                .Returns(Task.FromResult(true));

            mockProcessoRepository
                .Setup(p => p.Query())
                .Returns(processos.AsQueryable());

            // Act
            var response = await sut.Handle(query, default);

            // Assert
            response.Success.Should().BeTrue();
            response.Value.Should().HaveCount(2);
            response.Value[0].Id.Should().Be(1);
            response.Value[1].Id.Should().Be(3);
        }

        [Fact]
        public async void ValidarFalhaValidação()
        {
            // Arrange
            var query = new ObterProcessosQuery();
            query.ConfigurarPaginacao(1, 10);

            mockValidator
                .Setup(pv => pv.ValidateModelAsync(query))
                .Returns(Task.FromResult(false));

            // Act
            var response = await sut.Handle(query, default);

            // Assert
            mockValidator.Verify(pv => pv.ValidateModelAsync(query), Times.Once);
            response.Success.Should().BeFalse();
        }
    }
}