using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Service.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Validators
{
    public class AtualizarProcessoCommandValidator : DomainValidatorBase<AtualizarProcessoCommand>, IAtualizarProcessoCommandValidator
    {
        private readonly IProcessoRepository processoRepository;
        private readonly IResponsavelRepository responsavelRepository;
        private readonly ISituacaoProcessoRepository situacaoProcessoRepository;
        private readonly IProcessoDomainService processoDomainService;

        public AtualizarProcessoCommandValidator(
            IProcessoRepository processoRepository,
            IResponsavelRepository responsavelRepository,
            ISituacaoProcessoRepository situacaoProcessoRepository,
            IProcessoDomainService processoDomainService,
            INotificationContext notificationContext) : base(notificationContext)
        {
            this.processoRepository = processoRepository;
            this.responsavelRepository = responsavelRepository;
            this.situacaoProcessoRepository = situacaoProcessoRepository;
            this.processoDomainService = processoDomainService;
            this.RuleFor(p => p.NumeroProcesso)
                .NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Número Processo: {ErrorMessages.ErroVazio}")
                .MaximumLength(20).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Número Processo: {string.Format(ErrorMessages.ErroTamanhoMaximo, 20)}");

            this.RuleFor(p => p.DataDistribuicao)
                .LessThanOrEqualTo(DateTimeOffset.Now).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Data Distribuição: {string.Format(ErrorMessages.ErroDataDeveSerAnterior, DateTimeOffset.Now.Date)}");

            this.RuleFor(p => p.PastaFisicaCliente)
                .MaximumLength(50).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Pasta Física Cliente: {string.Format(ErrorMessages.ErroTamanhoMaximo, 50)}"); ;

            this.RuleFor(p => p.Descricao)
                .MaximumLength(1000).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Descrição: {string.Format(ErrorMessages.ErroTamanhoMaximo, 1000)}"); ;

            this.RuleFor(p => p.SituacaoProcessoId)
                .NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Situação Processo: {ErrorMessages.ErroVazio}");

            this.RuleFor(p => p.ResponsaveisIds)
                .NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Responsável: {ErrorMessages.ErroVazio}")

                .Must(r => r.Count >= 1 && r.Count <= 3).When(r => r.ResponsaveisIds != null, ApplyConditionTo.CurrentValidator).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Quantidade Responsável: {string.Format(ErrorMessages.ErroQuantidadeResponsavel, 1, 3)}")

                .Must(r => r.Distinct().Count() == r.Count).When(r => r.ResponsaveisIds != null, ApplyConditionTo.CurrentValidator).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Responsável: {ErrorMessages.ResponsavelRepetido}");
        }

        public override Task<bool> ValidateModelAsync(AtualizarProcessoCommand model)
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

            var processo = this.processoRepository.ObterPorId(model.Id.Value);
            if (processo == null)
            {
                this.AddError(NotificationKeys.NotFound, string.Format(ErrorMessages.ProcessoNaoEcontrado, model.NumeroProcesso));
                return Task.FromResult(false);
            }

            if (processo.Situacao.Finalizado)
            {
                this.AddError(NotificationKeys.InvalidArgument, string.Format(ErrorMessages.ProcessoFinalizado, model.NumeroProcesso));
                return Task.FromResult(false);
            }

            var situacaoProcesso = this.situacaoProcessoRepository.ObterPorId(model.SituacaoProcessoId.Value);
            if (situacaoProcesso == null)
            {
                this.AddError(NotificationKeys.NotFound, string.Format(ErrorMessages.SituacaoNaoEncontrada, model.SituacaoProcessoId));
                return Task.FromResult(false);
            }

            if (situacaoProcesso.Finalizado)
            {
                this.AddError(NotificationKeys.InvalidArgument, string.Format(ErrorMessages.ProcessoFinalizado, model.SituacaoProcessoId));
                return Task.FromResult(false);
            }

            var responsaveisNaoEncontrados = model.ResponsaveisIds.Where(rId => this.responsavelRepository.ObterPorId(rId) == null);
            if (responsaveisNaoEncontrados?.Any() == true)
            {
                this.AddError(NotificationKeys.NotFound, ErrorMessages.ResponsavelNaoEncontrado);
                return Task.FromResult(false);
            }

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