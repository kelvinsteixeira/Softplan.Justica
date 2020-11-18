using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Validators
{
    public class RemoverProcessoCommandValidator : DomainValidatorBase<RemoverProcessoCommand>, IRemoverProcessoCommandValidator
    {
        private readonly IProcessoRepository processoRepository;

        public RemoverProcessoCommandValidator(IProcessoRepository processoRepository,
            INotificationContext notificationContext) : base(notificationContext)
        {
            this.processoRepository = processoRepository;

            this.RuleFor(p => p.Id).NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Id: {ErrorMessages.ErroVazio}");
        }

        public override Task<bool> ValidateModelAsync(RemoverProcessoCommand model)
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
                this.AddError(NotificationKeys.NotFound, string.Format(ErrorMessages.ProcessoNaoEcontrado, model.Id));
                return Task.FromResult(false);
            }

            if (processo.Situacao.Finalizado)
            {
                this.AddError(NotificationKeys.InvalidArgument, string.Format(ErrorMessages.ProcessoFinalizado, model.Id));
                return Task.FromResult(false);
            }

            var processoFilho = this.processoRepository.Obter(p => p.ProcessoVinculadoId == model.Id).FirstOrDefault();
            if (processoFilho != null)
            {
                this.AddError(NotificationKeys.InvalidArgument, string.Format(ErrorMessages.ProcessoPaiNaoPodeSerRemovido, model.Id));
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}