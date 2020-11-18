using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain
{
    public abstract class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public abstract class RequestHandlerBase<TRequest, TResponse, TValidator> : RequestHandlerBase<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TValidator : IDomainValidator<TRequest>
    {
        public RequestHandlerBase(INotificationContext notificationContext, TValidator validator)
        {
            this.NotificationContext = notificationContext;
            this.Validator = validator;
        }

        protected INotificationContext NotificationContext { get; }

        protected TValidator Validator { get; }

        protected Task<bool> ValidateAsync(TRequest request)
        {
            return this.Validator.ValidateModelAsync(request);
        }
    }
}