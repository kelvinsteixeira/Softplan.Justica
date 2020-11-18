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
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;
using Xunit;

namespace Softplan.Justica.GerenciadorResponsaveis.Tests.ApplicationServices
{
    public class ResponsavelServiceTests
    {
        private readonly int? ResponsavelId = 1;

        private ResponsavelService sut;
        private CriarResponsavelCommand criarResponsavelCommand;
        private AtualizarResponsavelCommand atualizarResponsavelCommand;
        private Mock<IMediator> mockMediator;

        public ResponsavelServiceTests()
        {
            this.mockMediator = new Mock<IMediator>();

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

            this.sut = new ResponsavelService(this.mockMediator.Object);
        }

        [Fact]
        public async void ValidarCriarResponsavel()
        {
            // Arrange
            this.mockMediator
                .Setup(m => m.Send(It.IsNotNull<CriarResponsavelCommand>(), default))
                .Returns(Task.FromResult(new RequestResponseWrapper<int?>(true, this.ResponsavelId)));

            // Act
            var response = await this.sut.CriarResponsavelAsync(this.criarResponsavelCommand);

            // Assert
            response.Should().NotBeNull();
            response.Should().Be(this.ResponsavelId);
            mockMediator.Verify(m => m.Send(It.IsNotNull<CriarResponsavelCommand>(), default), Times.Once);
        }

        [Fact]
        public async void ValidarAtualizarResponsavel()
        {
            // Arrange
            this.mockMediator
                .Setup(m => m.Send(It.IsNotNull<AtualizarResponsavelCommand>(), default));

            // Act
            await this.sut.AtualizarResponsavelAsync(this.atualizarResponsavelCommand);

            // Assert
            mockMediator.Verify(m => m.Send(It.IsNotNull<AtualizarResponsavelCommand>(), default), Times.Once);
        }

        [Fact]
        public async void ValidarObterResponsavel()
        {
            // Arrange
            var query = new ObterResponsavelQuery { Id = this.ResponsavelId };

            var Responsavel = new ResponsavelDto
            {
                Id = 1,
                Nome = "Nome",
                Cpf = "Cpf",
                Email = "Email",
                Foto = new byte[1]
            };

            this.mockMediator
                .Setup(m => m.Send(query, default))
                .Returns(Task.FromResult(new RequestResponseWrapper<ResponsavelDto>(true, Responsavel)));

            // Act
            var response = await this.sut.ObterResponsavelAsync(query);

            // Assert
            response.Should().BeEquivalentTo(Responsavel);
            this.mockMediator.Verify(m => m.Send(It.IsNotNull<ObterResponsavelQuery>(), default), Times.Once);
        }

        [Fact]
        public async void ValidarObterResponsaveis()
        {
            // Arrange
            var query = new ObterResponsaveisQuery();
            var responsaveis = PagedList<ResponsavelDto>.Create(new List<ResponsavelDto>
            {
                new ResponsavelDto
                {
                    Id = 1,
                    Nome = "Nome",
                    Cpf = "Cpf",
                    Email = "Email",
                    Foto = new byte[1]
                },
                new ResponsavelDto
                {
                    Id = 2,
                    Nome = "Nome2",
                    Cpf = "Cpf2",
                    Email = "Email2",
                    Foto = new byte[1]
                }
            }.AsQueryable(),
            pageNumber: 1,
            pageSize: 30);

            this.mockMediator
                .Setup(m => m.Send(query, default))
                .Returns(Task.FromResult(new RequestResponseWrapper<PagedList<ResponsavelDto>>(true, responsaveis)));

            // Act
            var response = await this.sut.ObterResponsaveisAsync(query);

            // Assert
            response.Should().BeEquivalentTo(responsaveis);
            this.mockMediator.Verify(mockMediator => mockMediator.Send(It.IsNotNull<ObterResponsaveisQuery>(), default), Times.Once);
        }
    }
}