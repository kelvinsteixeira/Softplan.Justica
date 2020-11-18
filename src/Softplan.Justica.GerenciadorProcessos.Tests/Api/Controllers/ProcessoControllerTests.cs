using System;
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
    public class ProcessoControllerTests
    {
        private readonly int? ProcessoId = 1;

        private ProcessoController sut;
        private CriarProcessoCommand criarProcessoCommand;
        private AtualizarProcessoCommand atualizarProcessoCommand;
        private Mock<IProcessoService> mockProcessoService;
        private Mock<IApiResultHandler> mockApiResultHandler;
        private Mock<ILogger<ProcessoController>> mockLogger;

        public ProcessoControllerTests()
        {
            this.mockProcessoService = new Mock<IProcessoService>();
            this.mockApiResultHandler = new Mock<IApiResultHandler>();
            this.mockLogger = new Mock<ILogger<ProcessoController>>();

            sut = new ProcessoController(
                this.mockApiResultHandler.Object,
                this.mockProcessoService.Object,
                this.mockLogger.Object);

            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = MockHelpers.CreateHttpContextMock()
            };

            criarProcessoCommand = new CriarProcessoCommand
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

            atualizarProcessoCommand = new AtualizarProcessoCommand
            {
                Id = ProcessoId,
                NumeroProcesso = "123",
                DataDistribuicao = DateTimeOffset.Now,
                SegredoJustica = true,
                PastaFisicaCliente = "pastaCliente",
                Descricao = "descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1, 2 },
                ProcessoVinculadoId = 1
            };
        }

        [Fact]
        public async void ValidarCriarProcesso()
        {
            // Arrange
            mockProcessoService
                .Setup(ps => ps.CriarProcessoAsync(criarProcessoCommand))
                .Returns(Task.FromResult(ProcessoId));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, ProcessoId))
                .Returns(sut.Ok(ProcessoId));

            // Act
            IActionResult actionResult = await sut.CriarProcessoAsync(criarProcessoCommand);

            // Assert
            var objectResult = actionResult.Should().BeOfType<CreatedAtRouteResult>().Which;
            objectResult.RouteName.Should().Be(ProcessoController.ObterProcessoRouteName);
            objectResult.Value.Should().Be(ProcessoId);
        }

        [Fact]
        public async void ValidarResultNotFound()
        {
            // Arrange
            int? response = null;

            mockProcessoService
                .Setup(rs => rs.CriarProcessoAsync(criarProcessoCommand))
                .Returns(Task.FromResult(response));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, It.IsAny<int?>()))
                .Returns(sut.NotFound(response));

            // Act
            IActionResult actionResult = await sut.CriarProcessoAsync(criarProcessoCommand);

            // Assert
            actionResult.Should().BeOfType<NotFoundObjectResult>().Subject.Value.Should().BeNull();
        }

        [Fact]
        public async void ValidarResult500ServerError()
        {
            // Arrange
            mockProcessoService
                .Setup(rs => rs.CriarProcessoAsync(criarProcessoCommand))
                .Throws(new Exception());

            // Act
            IActionResult actionResult = await sut.CriarProcessoAsync(criarProcessoCommand);

            // Assert
            actionResult.Should().BeOfType<StatusCodeResult>().Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async void ValidarAtualizarProcesso()
        {
            // Arrange
            mockProcessoService
                .Setup(rs => rs.AtualizarProcessoAsync(atualizarProcessoCommand));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, null))
                .Returns(sut.Ok());

            // Act
            IActionResult actionResult = await sut.AtualizarProcessoAsync(ProcessoId, atualizarProcessoCommand);

            // Assert
            actionResult.Should().BeOfType<OkResult>();
            atualizarProcessoCommand.Id.Should().Be(ProcessoId);
        }

        [Fact]
        public async void ValidarRemoverProcesso()
        {
            // Arrange
            mockProcessoService
                .Setup(rs => rs.RemoverProcessoAsync(It.IsNotNull<RemoverProcessoCommand>()));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, null))
                .Returns(sut.Ok());

            // Act
            IActionResult actionResult = await sut.RemoverProcessoAsync(ProcessoId);

            // Assert
            actionResult.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async void ValidarObterProcessos()
        {
            // Arrange
            var parameters = new ObterProcessosParameter
            {
                PageNumber = 1,
                PageSize = 30
            };

            var pagedList = PagedList<ProcessoDto>.Create(
                source: new List<ProcessoDto>()
                {
                    new ProcessoDto
                    {
                        Id = 1,
                        DataDistribuicao = DateTimeOffset.Now,
                        Descricao = "Descricao",
                        NumeroProcesso = "PastaFisicaCliente",
                        PastaFisicaCliente = "",
                        SegredoJustica = true,
                        SituacaoProcessoId = 1
                    },
                    new ProcessoDto
                    {
                        Id = 2,
                        DataDistribuicao = DateTimeOffset.Now.AddDays(1),
                        Descricao = "Descricao2",
                        NumeroProcesso = "PastaFisicaCliente2",
                        PastaFisicaCliente = "",
                        SegredoJustica = false,
                        SituacaoProcessoId = 2
                    }
                }.AsQueryable(),
                pageNumber: parameters.PageNumber,
                pageSize: parameters.PageSize);

            mockProcessoService
                .Setup(rs => rs.ObterProcessosAsync(It.IsNotNull<ObterProcessosQuery>()))
                .Returns(Task.FromResult(pagedList));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, pagedList))
                .Returns(sut.Ok(pagedList));

            // Act
            IActionResult actionResult = await sut.ObterProcessosAsync(new ObterProcessosQuery(), parameters);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(pagedList);
        }

        [Fact]
        public async void ValidarObterProcesso()
        {
            // Arrange
            var result = new ProcessoDto
            {
                Id = ProcessoId,
                DataDistribuicao = DateTimeOffset.Now,
                Descricao = "Descricao",
                NumeroProcesso = "PastaFisicaCliente",
                PastaFisicaCliente = "",
                SegredoJustica = true,
                SituacaoProcessoId = 1
            };

            mockProcessoService
                .Setup(rs => rs.ObterProcessoAsync(It.IsAny<ObterProcessoQuery>()))
                .Returns(Task.FromResult(result));

            mockApiResultHandler
                .Setup(rh => rh.Handle(sut, result))
                .Returns(sut.Ok(result));

            // Act
            IActionResult actionResult = await sut.ObterProcessoAsync(ProcessoId);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(result);
        }
    }
}