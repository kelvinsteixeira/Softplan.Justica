using System;
using System.Collections.Generic;
using Bogus;
using Moq;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Service.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Tests.Domain.Validators
{
    public abstract class ValidatorTestDataBase
    {
        public ValidatorTestDataBase()
        {
            this.Faker = new Faker("pt_BR");

            this.MockProcessoRepository = new Mock<IProcessoRepository>();
            this.MockResponsavelRepository = new Mock<IResponsavelRepository>();
            this.MockSituacaoProcessoRepository = new Mock<ISituacaoProcessoRepository>();
            this.MockProcessoDomainService = new Mock<IProcessoDomainService>();
            this.MockNotificationContext = new Mock<INotificationContext>();

            this.Notifications = new List<Notification>();

            this.SituacaoEmAndamento = new SituacaoProcesso
            {
                Id = 1,
                Nome = "Em Andamento",
                Finalizado = false
            };

            this.SituacaoArquivado = new SituacaoProcesso
            {
                Id = 2,
                Nome = "Arquivado",
                Finalizado = true
            };

            this.Processo1 = new Processo
            {
                Id = 1,
                DataDistribuicao = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Descricao = this.Faker.Random.String(15),
                NumeroProcesso = "1155668879",
                PastaFisicaCliente = this.Faker.Random.String(20),
                Situacao = this.SituacaoEmAndamento,
                SituacaoId = 1
            };

            this.Processo2 = new Processo
            {
                Id = 2,
                DataDistribuicao = new DateTimeOffset(2021, 10, 10, 0, 0, 0, TimeSpan.Zero),
                Descricao = this.Faker.Random.String(15),
                NumeroProcesso = "9875158815",
                PastaFisicaCliente = this.Faker.Random.String(20),
                ProcessoVinculado = Processo1,
                ProcessoVinculadoId = 1,
                Situacao = SituacaoArquivado,
                SituacaoId = 2
            };

            this.Processo3 = new Processo
            {
                Id = 3,
                DataDistribuicao = new DateTimeOffset(2022, 5, 5, 0, 0, 0, TimeSpan.Zero),
                Descricao = this.Faker.Random.String(15),
                NumeroProcesso = "57478185451",
                PastaFisicaCliente = this.Faker.Random.String(20),
                ProcessoVinculado = Processo2,
                ProcessoVinculadoId = 2
            };

            this.Processo4 = new Processo
            {
                Id = 4,
                DataDistribuicao = new DateTimeOffset(2023, 10, 10, 0, 0, 0, TimeSpan.Zero),
                Descricao = this.Faker.Random.String(15),
                NumeroProcesso = "030947609345",
                PastaFisicaCliente = this.Faker.Random.String(20),
                ProcessoVinculado = Processo3,
                ProcessoVinculadoId = 3
            };

            this.Processo5 = new Processo
            {
                Id = 5,
                DataDistribuicao = new DateTimeOffset(2024, 5, 5, 0, 0, 0, TimeSpan.Zero),
                Descricao = this.Faker.Random.String(15),
                NumeroProcesso = "230523905",
                PastaFisicaCliente = this.Faker.Random.String(20)
            };

            this.Aline = new Responsavel
            {
                Id = 1,
                Nome = "Aline Schorn",
                Cpf = "40729427072",
                Email = "aline@mail.com"
            };

            this.Calvin = new Responsavel
            {
                Id = 2,
                Nome = "Calvin Schmaltz",
                Cpf = "12252771089",
                Email = "calvin@mail.com"
            };

            this.Kelvin = new Responsavel
            {
                Id = 3,
                Nome = "Kelvin Teixeira",
                Cpf = "39455771012",
                Email = "kelvin@mail.com"
            };

            this.UsuarioSemProcesso = new Responsavel
            {
                Id = 4,
                Nome = "Usuario",
                Cpf = "39455771012",
                Email = "usuario@mail.com"
            };

            this.Processo1.AtribuirResponsaveis(new List<Responsavel> { Aline, Calvin });
            this.Processo2.AtribuirResponsaveis(new List<Responsavel> { Calvin });
            this.Processo3.AtribuirResponsaveis(new List<Responsavel> { Kelvin });

            this.Aline.AtribuirProcessos(new List<Processo> { this.Processo1 });
            this.Calvin.AtribuirProcessos(new List<Processo> { this.Processo1, this.Processo2 });
            this.Kelvin.AtribuirProcessos(new List<Processo> { this.Processo3 });

            this.Processos = new List<Processo> { Processo1, Processo2, Processo3, Processo4, Processo5 };
            this.Responsaveis = new List<Responsavel> { Aline, Calvin, Kelvin, UsuarioSemProcesso };

            this.SituacoesProcesso = new List<SituacaoProcesso> { SituacaoArquivado, SituacaoEmAndamento };
        }

        public List<Processo> Processos { get; private set; }

        public List<SituacaoProcesso> SituacoesProcesso { get; private set; }

        public List<Responsavel> Responsaveis { get; private set; }

        public Responsavel Aline { get; }

        public Responsavel Calvin { get; }

        public Responsavel Kelvin { get; }

        public Responsavel UsuarioSemProcesso { get; }

        public Processo Processo1 { get; }

        public Processo Processo2 { get; }

        public Processo Processo3 { get; }

        public Processo Processo4 { get; }

        public Processo Processo5 { get; }

        public SituacaoProcesso SituacaoEmAndamento { get; }

        public SituacaoProcesso SituacaoArquivado { get; }

        public Mock<IProcessoRepository> MockProcessoRepository { get; }

        public Mock<IResponsavelRepository> MockResponsavelRepository { get; }

        public Mock<ISituacaoProcessoRepository> MockSituacaoProcessoRepository { get; }

        public Mock<IProcessoDomainService> MockProcessoDomainService { get; }

        public Mock<INotificationContext> MockNotificationContext { get; }

        public List<Notification> Notifications { get; }

        public Faker Faker { get; }
    }
}