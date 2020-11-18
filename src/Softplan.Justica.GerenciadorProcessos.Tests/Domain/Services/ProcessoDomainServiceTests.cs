using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Service;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.Domain.Services
{
    public class ProcessoDomainServiceTests
    {
        private readonly Mock<IProcessoRepository> mockProcessoRepository;
        private readonly ProcessoDomainService sut;
        private readonly List<Processo> processos;

        public Processo processo1;
        public Processo processo2;
        public Processo processo3;
        public Processo processo4;
        public Processo processo5;

        public ProcessoDomainServiceTests()
        {
            this.mockProcessoRepository = new Mock<IProcessoRepository>();

            IEnumerable<Processo> tempProcessos = new List<Processo>();

            this.mockProcessoRepository
                .Setup(pr => pr.Obter(It.IsNotNull<Expression<Func<Processo, bool>>>()))
                .Returns((Expression<Func<Processo, bool>> predicate) => processos.Where(predicate.Compile()));

            this.mockProcessoRepository
                .Setup(pr => pr.ObterPorId(It.IsNotNull<int>()))
                .Returns((int id) => processos.FirstOrDefault(p => p.Id == id));

            this.processo1 = new Processo
            {
                Id = 1,
                NumeroProcesso = "1155668879"
            };

            this.processo2 = new Processo
            {
                Id = 2,
                NumeroProcesso = "9875158815"
            };

            this.processo3 = new Processo
            {
                Id = 3,
                NumeroProcesso = "57478185451"
            };

            this.processo4 = new Processo
            {
                Id = 4,
                NumeroProcesso = "030947609345"
            };

            this.processo5 = new Processo
            {
                Id = 5,
                NumeroProcesso = "230523905"
            };

            this.processos = new List<Processo> { processo1, processo2, processo3, processo4, processo5 };

            this.sut = new ProcessoDomainService(this.mockProcessoRepository.Object);
        }

        [Fact]
        public void ValidarObterHierarquia1()
        {
            // Arrange
            this.processo3.ProcessoVinculado = processo2;
            this.processo3.ProcessoVinculadoId = processo2.Id;

            this.processo2.ProcessoVinculado = processo1;
            this.processo2.ProcessoVinculadoId = processo1.Id;

            // Act
            var result = sut.ObterHierarquia(this.processo3);

            // Assert
            result.Should().HaveCount(3);
        }

        [Fact]
        public void ValidarObterHierarquia2()
        {
            // Arrange
            // Act
            var result = sut.ObterHierarquia(this.processo1);

            // Assert
            result.Should().HaveCount(1);
        }

        [Fact]
        public void ValidarObterHierarquia3()
        {
            // Arrange
            this.processo5.ProcessoVinculado = processo4;
            this.processo5.ProcessoVinculadoId = processo4.Id;

            this.processo4.ProcessoVinculado = processo3;
            this.processo4.ProcessoVinculadoId = processo3.Id;

            this.processo3.ProcessoVinculado = processo2;
            this.processo3.ProcessoVinculadoId = processo2.Id;

            this.processo2.ProcessoVinculado = processo1;
            this.processo2.ProcessoVinculadoId = processo1.Id;

            // Act
            var result = sut.ObterHierarquia(this.processo3);

            // Assert
            result.Should().HaveCount(5);
        }

        [Fact]
        public void ValidarNaoExistenteNaHierarquia()
        {
            // Arrange
            this.processo4.ProcessoVinculado = processo3;
            this.processo4.ProcessoVinculadoId = processo3.Id;

            this.processo3.ProcessoVinculado = processo2;
            this.processo3.ProcessoVinculadoId = processo2.Id;

            this.processo2.ProcessoVinculado = processo1;
            this.processo2.ProcessoVinculadoId = processo1.Id;

            var novoProcesso = new Processo { ProcessoVinculadoId = 1, ProcessoVinculado = this.processo1 };

            // Act
            var result = this.sut.ValidarNaoExistenteNaHierarquia(novoProcesso);

            // Assert
            result.Should().BeFalse();
        }
    }
}