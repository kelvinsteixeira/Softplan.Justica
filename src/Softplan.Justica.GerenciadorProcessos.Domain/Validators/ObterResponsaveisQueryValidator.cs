using System.Threading.Tasks;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Validators
{
    public class ObterResponsaveisQueryValidator : DomainValidatorBase<ObterResponsaveisQuery>, IObterResponsaveisQueryValidator
    {
        public ObterResponsaveisQueryValidator(INotificationContext notificationContext) : base(notificationContext)
        {
        }

        public override Task<bool> ValidateModelAsync(ObterResponsaveisQuery model)
        {
            return Task.FromResult(true);
        }
    }
}