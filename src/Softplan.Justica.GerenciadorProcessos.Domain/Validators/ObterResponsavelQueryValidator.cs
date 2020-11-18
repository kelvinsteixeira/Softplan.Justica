using System.Threading.Tasks;
using FluentValidation;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Validators
{
    public class ObterResponsavelQueryValidator : DomainValidatorBase<ObterResponsavelQuery>, IObterResponsavelQueryValidator
    {
        public ObterResponsavelQueryValidator(INotificationContext notificationContext) : base(notificationContext)
        {
            this.RuleFor(p => p.Id).NotEmpty();
        }

        public override Task<bool> ValidateModelAsync(ObterResponsavelQuery model)
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

            return Task.FromResult(true);
        }
    }
}