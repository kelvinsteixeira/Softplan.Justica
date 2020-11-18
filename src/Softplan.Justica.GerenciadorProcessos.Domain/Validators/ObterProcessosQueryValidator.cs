using System.Threading.Tasks;
using Softplan.Justica.GerenciadorProcessos.Domain.Query;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Validators
{
    public class ObterProcessosQueryValidator : DomainValidatorBase<ObterProcessosQuery>, IObterProcessosQueryValidator
    {
        public ObterProcessosQueryValidator(INotificationContext notificationContext) : base(notificationContext)
        {

        }

        public override Task<bool> ValidateModelAsync(ObterProcessosQuery model)
        {
            return Task.FromResult(true);
        }
    }
}