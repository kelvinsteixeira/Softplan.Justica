using System.Threading.Tasks;
using FluentValidation;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Validators
{
    public abstract class DomainValidatorBase<TModel> : AbstractValidator<TModel>, IDomainValidator<TModel>
    {
        private readonly INotificationContext notificationContext;

        public DomainValidatorBase(INotificationContext notificationContext)
        {
            this.notificationContext = notificationContext;
        }

        public abstract Task<bool> ValidateModelAsync(TModel t);

        protected void AddError(string key, string message)
        {
            this.notificationContext.Add(key, message);
        }
    }
}