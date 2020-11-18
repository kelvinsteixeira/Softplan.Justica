using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Service.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Validators
{
    public class CriarProcessoCommandValidator : DomainValidatorBase<CriarProcessoCommand>, ICriarProcessoCommandValidator
    {
        private readonly IProcessoRepository processoRepository;
        private readonly ISituacaoProcessoRepository situacaoProcessoRepository;
        private readonly IResponsavelRepository responsavelRepository;
        private readonly IProcessoDomainService processoDomainService;

        public CriarProcessoCommandValidator(
            IProcessoRepository processoRepository,
            ISituacaoProcessoRepository situacaoProcessoRepository,
            IResponsavelRepository responsavelRepository,
            IProcessoDomainService processoDomainService,
            INotificationContext notificationContext) : base(notificationContext)
        {
            this.processoRepository = processoRepository;
            this.situacaoProcessoRepository = situacaoProcessoRepository;
            this.responsavelRepository = responsavelRepository;
            this.processoDomainService = processoDomainService;
            this.RuleFor(p => p.NumeroProcesso)
                .NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Número Processo: {ErrorMessages.ErroVazio}")
                .MaximumLength(20).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Número Processo: {string.Format(ErrorMessages.ErroTamanhoMaximo, 20)}");

            this.RuleFor(p => p.DataDistribuicao)
                .LessThanOrEqualTo(DateTimeOffset.Now).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Data Distribuição: {string.Format(ErrorMessages.ErroDataDeveSerAnterior, DateTimeOffset.Now.Date.ToString())}");

            this.RuleFor(p => p.PastaFisicaCliente)
                .MaximumLength(50).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Pasta Física Cliente: {string.Format(ErrorMessages.ErroTamanhoMaximo, 50)}");

            this.RuleFor(p => p.Descricao)
                .MaximumLength(1000).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Descrição: {string.Format(ErrorMessages.ErroTamanhoMaximo, 1000)}");

            this.RuleFor(p => p.SituacaoProcessoId)
                .NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Situação Processo: {ErrorMessages.ErroVazio}");

            this.RuleFor(p => p.ResponsaveisIds)
                .NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Responsável: {ErrorMessages.ErroVazio}")

                .Must(r => r.Count >= 1 && r.Count <= 3).When(r => r.ResponsaveisIds != null, ApplyConditionTo.CurrentValidator).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Quantidade Responsável: {string.Format(ErrorMessages.ErroQuantidadeResponsavel, 1, 3)}")

                .Must(r => r.Distinct().Count() == r.Count).When(r => r.ResponsaveisIds != null, ApplyConditionTo.CurrentValidator).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Responsável: {ErrorMessages.ResponsavelRepetido}");
        }

        public override Task<bool> ValidateModelAsync(CriarProcessoCommand model)
        {
            var result = this.Validate(model);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    this.AddError(error.ErrorCode, error.ErrorMessage);
                }

                return Task.FromResult(false);
            }

            Util.NumeroProcesso numeroProcesso = model.NumeroProcesso;
            var processo = this.processoRepository.Obter(p => p.NumeroProcesso == numeroProcesso.ToString()).FirstOrDefault();
            if (processo != null)
            {
                this.AddError(NotificationKeys.AlreadyExists, string.Format(ErrorMessages.ProcessoJaExiste, model.NumeroProcesso));
                return Task.FromResult(false);
            }

            var situacaoProcesso = this.situacaoProcessoRepository.ObterPorId(model.SituacaoProcessoId.Value);
            if (situacaoProcesso == null)
            {
                this.AddError(NotificationKeys.NotFound, string.Format(ErrorMessages.SituacaoNaoEncontrada, model.SituacaoProcessoId));
                return Task.FromResult(false);
            }

            var responsaveisNaoEncontrados = model.ResponsaveisIds.Where(rId => this.responsavelRepository.ObterPorId(rId) == null);
            if (responsaveisNaoEncontrados?.Any() == true)
            {
                this.AddError(NotificationKeys.NotFound, ErrorMessages.ResponsavelNaoEncontrado);
                return Task.FromResult(false);
            }

            processo = new Processo { ProcessoVinculadoId = model.ProcessoVinculadoId };

            if (!this.processoDomainService.ValidarHierarquiaQuantidade(processo))
            {
                this.AddError(NotificationKeys.InvalidArgument, ErrorMessages.ProcessoQuantidadeHierarquiaExcedido);
                return Task.FromResult(false);
            }

            if (!this.processoDomainService.ValidarNaoExistenteNaHierarquia(processo))
            {
                this.AddError(NotificationKeys.InvalidArgument, ErrorMessages.ProcessoJaConstaNaHierarquia);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}