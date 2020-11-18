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
    public class ObterResponsavelQueryHandlerTests
    {
        private Mock<IResponsavelRepository> mockResponsavelRepository;
        private Mock<INotificationContext> mockNotificationContext;
        private Mock<IObterResponsavelQueryValidator> mockValidator;
        private Mock<ILogger<ObterResponsavelQueryHandler>> mockLogger;

        private ObterResponsavelQuery query;
        private ObterResponsavelQueryHandler sut;

        public ObterResponsavelQueryHandlerTests()
        {
            this.mockResponsavelRepository = new Mock<IResponsavelRepository>();
            this.mockNotificationContext = new Mock<INotificationContext>();
            this.mockValidator = new Mock<IObterResponsavelQueryValidator>();
            this.mockLogger = new Mock<ILogger<ObterResponsavelQueryHandler>>();

            this.query = new ObterResponsavelQuery { Id = 1 };
            this.sut = new ObterResponsavelQueryHandler(
                this.mockResponsavelRepository.Object,
                this.mockNotificationContext.Object,
                this.mockValidator.Object,
                this.mockLogger.Object);
        }

        [Fact]
        public async void ValidarObterResponsavel()
        {
            // Arrange
            var responsavel = new Responsavel
            {
                Id = 1,
                Cpf = "123.456.789-11",
                Email = "email@email.com",
                Foto = new byte[1],
                Nome = "Nome"
            };

            this.mockResponsavelRepository
                .Setup(r => r.ObterPorId(1))
                .Returns(responsavel);

            this.mockValidator
                .Setup(v => v.ValidateModelAsync(It.IsNotNull<ObterResponsavelQuery>()))
                .Returns(Task.FromResult(true));

            // Act
            var response = await sut.Handle(query, default);

            // Assert
            this.mockResponsavelRepository.Verify(r => r.ObterPorId(1), Times.Once);
            this.mockValidator.Verify(r => r.ValidateModelAsync(query), Times.Once);
            response.Success.Should().BeTrue();
            response.Value.Id.Should().Be(responsavel.Id);
            response.Value.Cpf.Should().Be(responsavel.Cpf);
            response.Value.Email.Should().Be(responsavel.Email);
            response.Value.Nome.Should().Be(responsavel.Nome);
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