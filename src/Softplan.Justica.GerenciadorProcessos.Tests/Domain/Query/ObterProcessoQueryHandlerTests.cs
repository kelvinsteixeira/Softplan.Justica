using System;
using System.Collections.Generic;
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
    public class ObterProcessoQueryHandlerTests
    {
        private Mock<IObterProcessoQueryValidator> mockValidator;
        private Mock<IProcessoRepository> mockProcessoRepository;
        private Mock<INotificationContext> mockNotificationContext;
        private Mock<ILogger<ObterProcessoQueryHandler>> mockLogger;

        private ObterProcessoQuery query;
        private Responsavel respKelvin;
        private ObterProcessoQueryHandler sut;
        private Processo processo;

        public ObterProcessoQueryHandlerTests()
        {
            this.mockValidator = new Mock<IObterProcessoQueryValidator>();
            this.mockProcessoRepository = new Mock<IProcessoRepository>();
            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockLogger = new Mock<ILogger<ObterProcessoQueryHandler>>();

            query = new ObterProcessoQuery { Id = 1 };

            respKelvin = new Responsavel
            {
                Id = 3,
                Nome = "Kelvin Teixeira"
            };

            processo = new Processo
            {
                Id = 1,
                DataDistribuicao = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Descricao = "Aqui uma descrição simples",
                NumeroProcesso = "115566-88-79",
                PastaFisicaCliente = "Alguma coisa deve ser colocada aqui"
            };

            processo.AtribuirResponsaveis(new List<Responsavel> { respKelvin });

            sut = new ObterProcessoQueryHandler(
                this.mockProcessoRepository.Object,
                this.mockNotificationContext.Object,
                this.mockValidator.Object,
                this.mockLogger.Object);
        }

        [Fact]
        public async void ValidarObterProcesso()
        {
            // Arrange
            mockValidator
                .Setup(v => v.ValidateModelAsync(query))
                .Returns(Task.FromResult(true));

            mockProcessoRepository
                .Setup(p => p.ObterPorId(1))
                .Returns(processo);

            // Act
            var response = await sut.Handle(query, default);

            // Assert
            response.Success.Should().BeTrue();
            response.Value.Id.Should().Be(query.Id);
        }

        [Fact]
        public async void ValidarFalhaValidacao()
        {
            // Arrange
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