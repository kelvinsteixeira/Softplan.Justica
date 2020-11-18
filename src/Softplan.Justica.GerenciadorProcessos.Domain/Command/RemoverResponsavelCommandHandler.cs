using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Command
{
    public class RemoverResponsavelCommandHandler : RequestHandlerBase<RemoverResponsavelCommand, RequestResponseWrapper, IRemoverResponsavelCommandValidator>
    {
        private readonly IResponsavelRepository responsavelRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<RemoverResponsavelCommandHandler> logger;

        public RemoverResponsavelCommandHandler(
            IResponsavelRepository responsavelRepository,
            INotificationContext notificationContext,
            IRemoverResponsavelCommandValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<RemoverResponsavelCommandHandler> logger) : base(notificationContext, validator)
        {
            this.responsavelRepository = responsavelRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public override async Task<RequestResponseWrapper> Handle(RemoverResponsavelCommand request, CancellationToken cancellationToken)
        {
            if (await this.ValidateAsync(request))
            {
                try
                {
                    this.responsavelRepository.Remover(request.Id.Value);
                    await this.unitOfWork.SaveChangesAsync();

                    return new RequestResponseWrapper(true);
                }
                catch (Exception e)
                {
                    // Log
                    this.logger.LogError(e, "Erro Inesperado");
                    this.NotificationContext.Add(NotificationKeys.UnexpectedError, "Erro inesperado.");
                }
            }

            return new RequestResponseWrapper(false);
        }
    }
}