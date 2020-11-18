using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class ObterResponsaveisQueryHandlerTests
    {
        private Mock<IResponsavelRepository> mockResponsavelRepository;
        private Mock<INotificationContext> mockNotificationContext;
        private Mock<IObterResponsaveisQueryValidator> mockValidator;
        private Mock<ILogger<ObterResponsaveisQueryHandler>> mockLogger;

        private ObterResponsaveisQuery query;
        private ObterResponsaveisQueryHandler sut;

        private List<Responsavel> responsaveis;
        private Processo processo1;
        private Processo processo2;
        private Processo processo3;

        public ObterResponsaveisQueryHandlerTests()
        {
            this.mockResponsavelRepository = new Mock<IResponsavelRepository>();
            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockValidator = new Mock<IObterResponsaveisQueryValidator>();
            this.mockLogger = new Mock<ILogger<ObterResponsaveisQueryHandler>>();

            this.processo1 = new Processo
            {
                Id = 1,
                NumeroProcesso = "115566-88-79"
            };

            this.processo2 = new Processo
            {
                Id = 2,
                NumeroProcesso = "987515-88-15"
            };

            this.processo3 = new Processo
            {
                Id = 3,
                NumeroProcesso = "574781-85-451"
            };

            var aline = new Responsavel
            {
                Id = 1,
                Nome = "Aline Schorn",
                Cpf = "40729427072"
            };

            var calvin = new Responsavel
            {
                Id = 2,
                Nome = "Calvin Schmaltz",
                Cpf = "12252771089"
            };

            var kelvin = new Responsavel
            {
                Id = 3,
                Nome = "Kelvin Teixeira",
                Cpf = "39455771012"
            };

            this.responsaveis = new List<Responsavel> { aline, calvin, kelvin };

            aline.AtribuirProcessos(new List<Processo> { processo1, processo2 });
            calvin.AtribuirProcessos(new List<Processo> { processo1, processo2 });
            kelvin.AtribuirProcessos(new List<Processo> { processo1, processo3 });

            processo1.AtribuirResponsaveis(new List<Responsavel> { aline, calvin });
            processo2.AtribuirResponsaveis(new List<Responsavel> { aline });
            processo3.AtribuirResponsaveis(new List<Responsavel> { calvin, kelvin });

            this.responsaveis = new List<Responsavel> { aline, calvin, kelvin };

            this.sut = new ObterResponsaveisQueryHandler(
                this.mockResponsavelRepository.Object,
                this.mockNotificationContext.Object,
                this.mockValidator.Object,
                this.mockLogger.Object);
        }

        [Fact]
        public async void ValidarObterResponsaveis()
        {
            // Arrange
            var query = new ObterResponsaveisQuery();
            query.ConfigurarPaginacao(1, 10);

            this.mockResponsavelRepository
                .Setup(r => r.Query())
                .Returns(this.responsaveis.AsQueryable());

            this.mockValidator
               .Setup(v => v.ValidateModelAsync(It.IsNotNull<ObterResponsaveisQuery>()))
               .Returns(Task.FromResult(true));

            // Act
            var result = await sut.Handle(query, default);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Should().HaveCount(this.responsaveis.Count);
        }

        [Theory]
        [InlineData(1, 1, 1, 3, 3)]
        [InlineData(2, 2, 1, 2, 3)]
        [InlineData(1, 3, 3, 1, 3)]
        public async void ValidarRetornoPaginado(int pageNumber, int pageSize, int expectedItemsCount, int expectedTotalPages, int expectedTotalCount)
        {
            // Arrange
            var query = new ObterResponsaveisQuery();
            query.ConfigurarPaginacao(pageNumber, pageSize);

            this.mockResponsavelRepository
                .Setup(r => r.Query())
                .Returns(this.responsaveis.AsQueryable());

            this.mockValidator
               .Setup(v => v.ValidateModelAsync(It.IsNotNull<ObterResponsaveisQuery>()))
               .Returns(Task.FromResult(true));

            // Act
            var result = await sut.Handle(query, default);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Should().HaveCount(expectedItemsCount);
            result.Value.TotalPages.Should().Be(expectedTotalPages);
            result.Value.TotalCount.Should().Be(expectedTotalCount);
        }

        [Theory]
        [InlineData("115566-88-79", 3, new int[] { 1, 2, 3 })]
        [InlineData("987515-88-15", 2, new int[] { 1, 2 })]
        [InlineData("574781-85-451", 1, new int[] { 3 })]
        public async void ValidarFiltroNumeroProcesso(string numeroProcesso, int responsaveisCountEsperado, int[] responsaveisIdsEsperados)
        {
            // Arrange
            var query = new ObterResponsaveisQuery { NumeroProcesso = numeroProcesso };
            query.ConfigurarPaginacao(1, 10);

            this.mockResponsavelRepository
                .Setup(r => r.Query())
                .Returns(this.responsaveis.AsQueryable());

            this.mockValidator
               .Setup(v => v.ValidateModelAsync(It.IsNotNull<ObterResponsaveisQuery>()))
               .Returns(Task.FromResult(true));

            // Act
            var result = await sut.Handle(query, default);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Should().HaveCount(responsaveisCountEsperado);
            result.Value.Select(r => r.Id).Should().Equal(responsaveisIdsEsperados);
        }

        [Theory]
        [InlineData("aline", new int[] { 1 })]
        [InlineData("cALVin", new int[] { 2 })]
        [InlineData("lvin", new int[] { 2, 3 })]
        public async void ValidarFiltroNome(string nome, int[] responsaveisIdsEsperados)
        {
            // Arrange
            var query = new ObterResponsaveisQuery { Nome = nome };
            query.ConfigurarPaginacao(1, 10);

            this.mockResponsavelRepository
                .Setup(r => r.Query())
                .Returns(this.responsaveis.AsQueryable());

            this.mockValidator
               .Setup(v => v.ValidateModelAsync(It.IsNotNull<ObterResponsaveisQuery>()))
               .Returns(Task.FromResult(true));

            // Act
            var result = await sut.Handle(query, default);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Select(r => r.Id).Should().Equal(responsaveisIdsEsperados);
        }

        [Theory]
        [InlineData("407.294.270-72", 1)]
        [InlineData("12252771089", 2)]
        [InlineData("394.557710-12", 3)]
        public async void ValidarFiltroCpf(string cpf, int responsavelIdEsperado)
        {
            // Arrange
            var query = new ObterResponsaveisQuery { Cpf = cpf };
            query.ConfigurarPaginacao(1, 10);

            this.mockResponsavelRepository
                .Setup(r => r.Query())
                .Returns(this.responsaveis.AsQueryable());

            this.mockValidator
               .Setup(v => v.ValidateModelAsync(It.IsNotNull<ObterResponsaveisQuery>()))
               .Returns(Task.FromResult(true));

            // Act
            var result = await sut.Handle(query, default);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value[0].Id.Should().Be(responsavelIdEsperado);
        }
    }
}
