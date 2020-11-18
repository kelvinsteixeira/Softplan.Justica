using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Application.Services;
using Softplan.Justica.GerenciadorProcessos.Domain;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;
using Softplan.Justica.GerenciadorProcessos.Domain.Events;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.ApplicationServices
{
    public class ProcessoServiceTests
    {
        private readonly int? ProcessoId = 1;

        private ProcessoService sut;
        private CriarProcessoCommand criarProcessoCommand;
        private AtualizarProcessoCommand atualizarProcessoCommand;
        private Mock<IMediator> mockMediator;

        public ProcessoServiceTests()
        {
            this.mockMediator = new Mock<IMediator>();

            this.criarProcessoCommand = new CriarProcessoCommand
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

            this.atualizarProcessoCommand = new AtualizarProcessoCommand
            {
                Id = 1,
                NumeroProcesso = "123",
                DataDistribuicao = DateTimeOffset.Now,
                SegredoJustica = true,
                PastaFisicaCliente = "pastaCliente",
                Descricao = "descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1, 2 },
                ProcessoVinculadoId = 1
            };

            this.sut = new ProcessoService(this.mockMediator.Object);
        }

        [Fact]
        public async void ValidarCriarProcesso()
        {
            // Arrange
            var criarProcessoCommandResponse = new RequestResponseWrapper<int?>(true, this.ProcessoId);

            this.mockMediator
                .Setup(m => m.Send(It.IsNotNull<CriarProcessoCommand>(), default))
                .Returns(Task.FromResult(criarProcessoCommandResponse));

            // Act
            var response = await this.sut.CriarProcessoAsync(this.criarProcessoCommand);

            // Assert
            response.Should().NotBeNull();
            response.Should().Be(this.ProcessoId);
            mockMediator.Verify(m => m.Send(It.IsNotNull<CriarProcessoCommand>(), default), Times.Once);
            mockMediator.Verify(m => m.Send(It.IsNotNull<ProcessoCriadoEvent>(), default), Times.Once);
        }

        [Fact]
        public async void ValidarAtualizarProcesso()
        {
            // Arrange
            var response = new RequestResponseWrapper<AtualizarProcessoResponse>(true, new AtualizarProcessoResponse(this.ProcessoId.Value, new List<int> { 1, 2 }));

            this.mockMediator
                .Setup(m => m.Send(It.IsNotNull<AtualizarProcessoCommand>(), default))
                .Returns(Task.FromResult(response));

            // Act
            await this.sut.AtualizarProcessoAsync(this.atualizarProcessoCommand);

            // Assert
            mockMediator.Verify(m => m.Send(It.IsNotNull<AtualizarProcessoCommand>(), default), Times.Once);
            mockMediator.Verify(m => m.Send(It.IsNotNull<ProcessoAtualizadoEvent>(), default), Times.Once);
        }

        [Fact]
        public async void ValidarObterProcesso()
        {
            // Arrange
            var query = new ObterProcessoQuery { Id = this.ProcessoId };

            var processo = new ProcessoDto
            {
                Id = 1,
                DataDistribuicao = DateTimeOffset.Now,
                Descricao = "Descrição",
                NumeroProcesso = "NumeroProcesso",
                PastaFisicaCliente = "PastaFisicaCliente",
                SegredoJustica = true,
                SituacaoProcessoId = 1
            };

            this.mockMediator
                .Setup(m => m.Send(query, default))
                .Returns(Task.FromResult(new RequestResponseWrapper<ProcessoDto>(true, processo)));

            // Act
            var response = await this.sut.ObterProcessoAsync(query);

            // Assert
            response.Should().BeEquivalentTo(processo);
        }

        [Fact]
        public async void ValidarObterProcessos()
        {
            // Arrange
            var query = new ObterProcessosQuery
            {
                NumeroProcesso = "987515-88-15"
            };
            var processos = PagedList<ProcessoDto>.Create(new List<ProcessoDto>
            {
                new ProcessoDto
                {
                    Id = 1,
                    DataDistribuicao = DateTimeOffset.Now,
                    Descricao = "Descrição",
                    NumeroProcesso = "NumeroProcesso",
                    PastaFisicaCliente = "PastaFisicaCliente",
                    SegredoJustica = true,
                    SituacaoProcessoId = 1
                },
                    new ProcessoDto
                {
                    Id = 1,
                    DataDistribuicao = DateTimeOffset.Now,
                    Descricao = "Descrição",
                    NumeroProcesso = "NumeroProcesso",
                    PastaFisicaCliente = "PastaFisicaCliente",
                    SegredoJustica = true,
                    SituacaoProcessoId = 1
                }
            }.AsQueryable(),
            pageNumber: 1,
            pageSize: 30);

            this.mockMediator
                .Setup(m => m.Send(query, default))
                .Returns(Task.FromResult(new RequestResponseWrapper<PagedList<ProcessoDto>>(true, processos)));

            // Act
            var response = await this.sut.ObterProcessosAsync(query);

            // Assert
            response.Should().BeEquivalentTo(processos);
        }
    }
}