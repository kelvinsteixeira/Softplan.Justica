using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.Domain.Validators
{
    public class AtualizarProcessoCommandValidatorTests : ValidatorTestDataBase
    {
        private AtualizarProcessoCommandValidator sut;

        public AtualizarProcessoCommandValidatorTests()
        {
            this.MockNotificationContext
                .Setup(n => n.Add(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Callback<string, string>((key, value) =>
                {
                    this.Notifications.Add(new Notification(key, value));
                });

            this.sut = new AtualizarProcessoCommandValidator(
                this.MockProcessoRepository.Object,
                this.MockResponsavelRepository.Object,
                this.MockSituacaoProcessoRepository.Object,
                this.MockProcessoDomainService.Object,
                this.MockNotificationContext.Object);
        }

        [Fact]
        public async void ValidarValidator()
        {
            // Arrange
            var command = new AtualizarProcessoCommand
            {
                Id = 1,
                NumeroProcesso = "83765873465",
                DataDistribuicao = DateTimeOffset.Now.AddDays(-1),
                SegredoJustica = true,
                PastaFisicaCliente = "Pasta Fisica Cliente",
                Descricao = "Descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1 },
                ProcessoVinculadoId = 1
            };

            this.MockProcessoRepository
                .Setup(p => p.ObterPorId(1))
                .Returns(this.Processo1);

            this.MockSituacaoProcessoRepository
                .Setup(s => s.ObterPorId(1))
                .Returns(this.SituacaoEmAndamento);

            this.MockResponsavelRepository
                .Setup(r => r.Obter(It.IsNotNull<Expression<Func<Responsavel, bool>>>()))
                .Returns(new List<Responsavel>());

            this.MockResponsavelRepository
                .Setup(r => r.ObterPorId(It.IsNotNull<int>()))
                .Returns(new Responsavel());

            this.MockProcessoDomainService
                .Setup(pd => pd.ValidarNaoExistenteNaHierarquia(It.IsNotNull<Processo>()))
                .Returns(true);

            this.MockProcessoDomainService
                .Setup(pd => pd.ValidarHierarquiaQuantidade(It.IsNotNull<Processo>()))
                .Returns(true);

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void ValidarFalhaTamanhoMaximo()
        {
            // Arrange
            var command = new AtualizarProcessoCommand
            {
                Id = 1,
                NumeroProcesso = this.Faker.Random.String(30),
                DataDistribuicao = DateTimeOffset.Now.AddDays(-1),
                SegredoJustica = true,
                PastaFisicaCliente = this.Faker.Random.String(55),
                Descricao = this.Faker.Random.String(1005),
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1 },
                ProcessoVinculadoId = 1
            };

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.MockNotificationContext.Verify(n => n.Add(It.IsNotNull<string>(), It.IsNotNull<string>()), Times.Exactly(3));
            this.Notifications.Should().HaveCount(3);
            this.Notifications.FirstOrDefault(n => n.Message == $"Número Processo: {string.Format(ErrorMessages.ErroTamanhoMaximo, 20)}").Should().NotBeNull();
            this.Notifications.FirstOrDefault(n => n.Message == $"Pasta Física Cliente: {string.Format(ErrorMessages.ErroTamanhoMaximo, 50)}").Should().NotBeNull();
            this.Notifications.FirstOrDefault(n => n.Message == $"Descrição: {string.Format(ErrorMessages.ErroTamanhoMaximo, 1000)}").Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaCamposVazios()
        {
            // Arrange
            var command = new AtualizarProcessoCommand
            {
                Id = 1,
                NumeroProcesso = null,
                DataDistribuicao = DateTimeOffset.Now.AddDays(-1),
                SegredoJustica = true,
                PastaFisicaCliente = this.Faker.Random.String(40),
                Descricao = this.Faker.Random.String(100),
                SituacaoProcessoId = null,
                ResponsaveisIds = null,
                ProcessoVinculadoId = 1
            };

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.Notifications.Should().HaveCount(3);
            this.Notifications.FirstOrDefault(n => n.Message == $"Número Processo: {ErrorMessages.ErroVazio}").Should().NotBeNull();
            this.Notifications.FirstOrDefault(n => n.Message == $"Responsável: {ErrorMessages.ErroVazio}").Should().NotBeNull();
            this.Notifications.FirstOrDefault(n => n.Message == $"Situação Processo: {ErrorMessages.ErroVazio}").Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaQuantidadeResponsaveis()
        {
            // Arrange
            var command = new AtualizarProcessoCommand
            {
                Id = 1,
                NumeroProcesso = "83765873465",
                DataDistribuicao = DateTimeOffset.Now.AddDays(-1),
                SegredoJustica = true,
                PastaFisicaCliente = "Pasta Fisica Cliente",
                Descricao = "Descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1, 2, 3, 4 },
                ProcessoVinculadoId = 1
            };

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.Notifications.Should().HaveCount(1);
            this.Notifications.FirstOrDefault(n => n.Message == $"Quantidade Responsável: {string.Format(ErrorMessages.ErroQuantidadeResponsavel, 1, 3)}").Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaResponsaveisDuplicados()
        {
            // Arrange
            var command = new AtualizarProcessoCommand
            {
                Id = 1,
                NumeroProcesso = "83765873465",
                DataDistribuicao = DateTimeOffset.Now.AddDays(-1),
                SegredoJustica = true,
                PastaFisicaCliente = "Pasta Fisica Cliente",
                Descricao = "Descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1, 1, 3 },
                ProcessoVinculadoId = 1
            };

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.Notifications.Should().HaveCount(1);
            this.Notifications.FirstOrDefault(n => n.Message == $"Responsável: {ErrorMessages.ResponsavelRepetido}").Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaProcessoNaoEncontrado()
        {
            // Arrange
            var command = new AtualizarProcessoCommand
            {
                Id = 9,
                NumeroProcesso = "12523452345",
                DataDistribuicao = DateTimeOffset.Now.AddDays(-1),
                SegredoJustica = true,
                PastaFisicaCliente = "Pasta Fisica Cliente",
                Descricao = "Descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1, 2, 3 },
                ProcessoVinculadoId = 1
            };

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.Notifications.Should().HaveCount(1);
            this.Notifications.FirstOrDefault(n => n.Message == string.Format(ErrorMessages.ProcessoNaoEcontrado, command.NumeroProcesso)).Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaSituacaoProcessoNaoEncontrada()
        {
            // Arrange
            var command = new AtualizarProcessoCommand
            {
                Id = 1,
                NumeroProcesso = "83765873465",
                DataDistribuicao = DateTimeOffset.Now.AddDays(-1),
                SegredoJustica = true,
                PastaFisicaCliente = "Pasta Fisica Cliente",
                Descricao = "Descricao",
                SituacaoProcessoId = 4,
                ResponsaveisIds = new List<int> { 1 },
                ProcessoVinculadoId = 1
            };

            this.MockProcessoRepository
                .Setup(p => p.ObterPorId(1))
                .Returns(this.Processo1);

            this.MockSituacaoProcessoRepository
                .Setup(s => s.ObterPorId(It.IsNotNull<int>()))
                .Returns((SituacaoProcesso)null);

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.Notifications.Should().HaveCount(1);
            this.Notifications.FirstOrDefault(n => n.Message == string.Format(ErrorMessages.SituacaoNaoEncontrada, command.SituacaoProcessoId)).Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaResponsavelNaoEncontrado()
        {
            // Arrange
            var command = new AtualizarProcessoCommand
            {
                Id = 1,
                NumeroProcesso = "83765873465",
                DataDistribuicao = DateTimeOffset.Now.AddDays(-1),
                SegredoJustica = true,
                PastaFisicaCliente = "Pasta Fisica Cliente",
                Descricao = "Descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1 },
                ProcessoVinculadoId = 1
            };

            this.MockProcessoRepository
                .Setup(p => p.ObterPorId(1))
                .Returns(this.Processo1);

            this.MockSituacaoProcessoRepository
                .Setup(s => s.ObterPorId(1))
                .Returns(this.SituacaoEmAndamento);

            this.MockResponsavelRepository
                .Setup(r => r.Obter(It.IsNotNull<Expression<Func<Responsavel, bool>>>()))
                .Returns(new List<Responsavel> { this.Aline
    });

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.Notifications.Should().HaveCount(1);
            this.Notifications.FirstOrDefault(n => n.Message == ErrorMessages.ResponsavelNaoEncontrado).Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaQuantidadeHierarquia()
        {
            // Arrange
            var command = new AtualizarProcessoCommand
            {
                Id = 1,
                NumeroProcesso = "83765873465",
                DataDistribuicao = DateTimeOffset.Now.AddDays(-1),
                SegredoJustica = true,
                PastaFisicaCliente = "Pasta Fisica Cliente",
                Descricao = "Descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1 },
                ProcessoVinculadoId = 1
            };

            this.MockProcessoRepository
                .Setup(p => p.ObterPorId(1))
                .Returns(this.Processo1);

            this.MockSituacaoProcessoRepository
                .Setup(s => s.ObterPorId(1))
                .Returns(this.SituacaoEmAndamento);

            this.MockResponsavelRepository
                .Setup(r => r.Obter(It.IsNotNull<Expression<Func<Responsavel, bool>>>()))
                .Returns(new List<Responsavel>());

            this.MockResponsavelRepository
                .Setup(r => r.ObterPorId(It.IsNotNull<int>()))
                .Returns(new Responsavel());

            this.MockProcessoDomainService
                .Setup(pd => pd.ValidarNaoExistenteNaHierarquia(It.IsNotNull<Processo>()))
                .Returns(false);

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.Notifications.Should().HaveCount(1);
            this.Notifications.FirstOrDefault(n => n.Message == ErrorMessages.ProcessoQuantidadeHierarquiaExcedido).Should().NotBeNull();
        }

        [Fact]
        public async void ValidarFalhaProcessoJaConstaHierarquia()
        {
            // Arrange
            var command = new AtualizarProcessoCommand
            {
                Id = 1,
                NumeroProcesso = "83765873465",
                DataDistribuicao = DateTimeOffset.Now.AddDays(-1),
                SegredoJustica = true,
                PastaFisicaCliente = "Pasta Fisica Cliente",
                Descricao = "Descricao",
                SituacaoProcessoId = 1,
                ResponsaveisIds = new List<int> { 1 },
                ProcessoVinculadoId = 1
            };

            this.MockProcessoRepository
                .Setup(p => p.ObterPorId(1))
                .Returns(this.Processo1);

            this.MockSituacaoProcessoRepository
                .Setup(s => s.ObterPorId(1))
                .Returns(this.SituacaoEmAndamento);

            this.MockResponsavelRepository
                .Setup(r => r.Obter(It.IsNotNull<Expression<Func<Responsavel, bool>>>()))
                .Returns(new List<Responsavel>());

            this.MockResponsavelRepository
                .Setup(r => r.ObterPorId(It.IsNotNull<int>()))
                .Returns(new Responsavel());

            this.MockProcessoDomainService
                .Setup(pd => pd.ValidarHierarquiaQuantidade(It.IsNotNull<Processo>()))
                .Returns(true);

            this.MockProcessoDomainService
                .Setup(pd => pd.ValidarNaoExistenteNaHierarquia(It.IsNotNull<Processo>()))
                .Returns(false);

            // Act
            var result = await this.sut.ValidateModelAsync(command);

            // Assert
            result.Should().BeFalse();
            this.Notifications.Should().HaveCount(1);
            this.Notifications.FirstOrDefault(n => n.Message == ErrorMessages.ProcessoJaConstaNaHierarquia).Should().NotBeNull();
        }
    }
}
