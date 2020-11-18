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
    public class RemoverResponsavelCommandValidator : DomainValidatorBase<RemoverResponsavelCommand>, IRemoverResponsavelCommandValidator
    {
        private readonly IResponsavelRepository responsavelRepository;

        public RemoverResponsavelCommandValidator(IResponsavelRepository responsavelRepository,
            INotificationContext notificationContext) : base(notificationContext)
        {
            this.RuleFor(p => p.Id)
                .NotEmpty().NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Id: {ErrorMessages.ErroVazio}");

            this.responsavelRepository = responsavelRepository;
        }

        public override Task<bool> ValidateModelAsync(RemoverResponsavelCommand model)
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

            var responsavel = this.responsavelRepository.ObterPorId(model.Id.Value);
            if (responsavel == null)
            {
                this.AddError(NotificationKeys.NotFound, ErrorMessages.ResponsavelNaoEncontrado);
                return Task.FromResult(false);
            }

            if (responsavel.Processos?.Any() == true)
            {
                this.AddError(NotificationKeys.InvalidArgument, ErrorMessages.ResponsavelVinculadoProcesso);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}