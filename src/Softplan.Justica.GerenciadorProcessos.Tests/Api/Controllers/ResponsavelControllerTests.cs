using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Application.Services.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Controllers;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Handlers.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;
using Softplan.Justica.GerenciadorProcessos.Parameters;
using Softplan.Justica.GerenciadorProcessos.Tests.Api.Helpers;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.Api.Controllers
{
    public class ResponsavelControllerTests
    {
        private readonly int? ResponsavelId = 1;

        private ResponsavelController sut;
        private CriarResponsavelCommand criarResponsavelCommand;
        private AtualizarResponsavelCommand atualizarResponsavelCommand;
        private Mock<IResponsavelService> mockResponsavelService;
        private Mock<IApiResultHandler> mockApiResultHandler;
        private Mock<ILogger<ResponsavelController>> mockLogger;

        public ResponsavelControllerTests()
        {
            this.mockResponsavelService = new Mock<IResponsavelService>();
            this.mockApiResultHandler = new Mock<IApiResultHandler>();
            this.mockLogger = new Mock<ILogger<ResponsavelController>>();

            this.sut = new ResponsavelController(
                this.mockApiResultHandler.Object,
                this.mockResponsavelService.Object,
                this.mockLogger.Object);

            this.sut.ControllerContext = new ControllerContext()
            {
                HttpContext = MockHelpers.CreateHttpContextMock()
            };

            this.criarResponsavelCommand = new CriarResponsavelCommand
            {
                Nome = "Nome",
                Cpf = "Cpf",
                Email = "Email",
                Foto = new byte[1]
            };

            this.atualizarResponsavelCommand = new AtualizarResponsavelCommand
            {
                Id = 1,
                Nome = "Nome",
                Cpf = "Cpf",
                Email = "Email",
                Foto = new byte[1]
            };
        }

        [Fact]
        public async void ValidarCriarResponsavel()
        {
            // Arrange
            mockResponsavelService
                .Setup(rs => rs.CriarResponsavelAsync(criarResponsavelCommand))
                .Returns(Task.FromResult(ResponsavelId));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, ResponsavelId))
                .Returns(sut.Ok(ResponsavelId));

            // Act
            IActionResult actionResult = await sut.CriarResponsavelAsync(criarResponsavelCommand);

            // Assert
            var objectResult = actionResult.Should().BeOfType<CreatedAtRouteResult>().Which;
            objectResult.RouteName.Should().Be(ResponsavelController.ObterResponsavelRouteName);
            objectResult.Value.Should().Be(ResponsavelId);
        }

        [Fact]
        public async void ValidarResultNotFound()
        {
            // Arrange

            mockResponsavelService
                .Setup(rs => rs.CriarResponsavelAsync(criarResponsavelCommand))
                .Returns(Task.FromResult<int?>(null));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, null))
                .Returns(sut.NotFound(null));

            // Act
            IActionResult actionResult = await sut.CriarResponsavelAsync(criarResponsavelCommand);

            // Assert
            actionResult.Should().BeOfType<NotFoundObjectResult>().Subject.Value.Should().BeNull();
        }

        [Fact]
        public async void ValidarResult500ServerError()
        {
            // Arrange
            mockResponsavelService
                .Setup(rs => rs.CriarResponsavelAsync(criarResponsavelCommand))
                .Throws(new System.Exception());

            // Act
            IActionResult actionResult = await sut.CriarResponsavelAsync(criarResponsavelCommand);

            // Assert
            actionResult.Should().BeOfType<StatusCodeResult>().Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async void ValidarAtualizarResponsavel()
        {
            // Arrange
            mockResponsavelService
                .Setup(rs => rs.AtualizarResponsavelAsync(atualizarResponsavelCommand));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, null))
                .Returns(sut.Ok());

            // Act
            IActionResult actionResult = await sut.AtualizarResponsavelAsync(ResponsavelId, atualizarResponsavelCommand);

            // Assert
            actionResult.Should().BeOfType<OkResult>();
            atualizarResponsavelCommand.Id.Should().Be(ResponsavelId);
        }

        [Fact]
        public async void ValidarRemoverResponsavel()
        {
            // Arrange
            mockResponsavelService
                .Setup(rs => rs.RemoverResponsavelAsync(It.IsAny<RemoverResponsavelCommand>()));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, null))
                .Returns(sut.Ok());

            // Act
            IActionResult actionResult = await sut.RemoverResponsavelAsync(ResponsavelId);

            // Assert
            actionResult.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async void ValidarObterResponsaveis()
        {
            // Arrange
            var query = new ObterResponsaveisQuery();

            var parameters = new ObterResponsaveisParameter
            {
                PageNumber = 1,
                PageSize = 30
            };

            var pagedList = PagedList<ResponsavelDto>.Create(
                source: new List<ResponsavelDto>()
                {
                    new ResponsavelDto
                    {
                        Id = 1,
                        Nome = "Nome1",
                        Cpf = "Cpf1",
                        Email= "Email1",
                        Foto = new byte[1]
                    },
                    new ResponsavelDto
                    {
                        Id = 2,
                        Nome = "Nome2",
                        Cpf = "Cpf2",
                        Email= "Email2",
                        Foto = new byte[1]
                    }
                }.AsQueryable(),
                pageNumber: parameters.PageNumber,
                pageSize: parameters.PageSize);

            mockResponsavelService
                .Setup(rs => rs.ObterResponsaveisAsync(It.IsAny<ObterResponsaveisQuery>()))
                .Returns(Task.FromResult(pagedList));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, pagedList))
                .Returns(sut.Ok(pagedList));

            // Act
            IActionResult actionResult = await sut.ObterResponsaveisAsync(query, parameters);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(pagedList);
        }

        [Fact]
        public async void ValidarObterResponsavel()
        {
            // Arrange
            var result = new ResponsavelDto
            {
                Id = ResponsavelId,
                Nome = "Nome1",
                Cpf = "Cpf1",
                Email = "Email1",
                Foto = new byte[1]
            };

            mockResponsavelService
                .Setup(rs => rs.ObterResponsavelAsync(It.IsAny<ObterResponsavelQuery>()))
                .Returns(Task.FromResult(result));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, result))
                .Returns(sut.Ok(result));

            // Act
            IActionResult actionResult = await sut.ObterResponsavelAsync(ResponsavelId);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(result);
        }
    }
}