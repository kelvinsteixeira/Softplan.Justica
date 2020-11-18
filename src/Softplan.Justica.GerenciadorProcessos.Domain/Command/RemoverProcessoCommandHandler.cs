using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Command
{
    public class RemoverProcessoCommandHandler : RequestHandlerBase<RemoverProcessoCommand, RequestResponseWrapper<RemoverResponsavelResponse>, IRemoverProcessoCommandValidator>
    {
        private readonly IProcessoRepository processoRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<RemoverProcessoCommandHandler> logger;

        public RemoverProcessoCommandHandler(
            IProcessoRepository processoRepository,
            INotificationContext notificationContext,
            IRemoverProcessoCommandValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<RemoverProcessoCommandHandler> logger) : base(notificationContext, validator)
        {
            this.processoRepository = processoRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public override async Task<RequestResponseWrapper<RemoverResponsavelResponse>> Handle(RemoverProcessoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await this.ValidateAsync(request))
                {
                    var processo = this.processoRepository.ObterPorId(request.Id.Value);
                    string numeroProcesso = processo.NumeroProcesso;
                    var responsaveisIds = processo.Responsaveis.Select(r => r.Id);

                    this.processoRepository.Remover(request.Id.Value);
                    await this.unitOfWork.SaveChangesAsync();

                    return new RequestResponseWrapper<RemoverResponsavelResponse>(true, new RemoverResponsavelResponse
                    {
                        NumeroProcesso = numeroProcesso,
                        ResponsaveisIds = responsaveisIds.ToArray()
                    });
                }
            }
            catch (System.Exception e)
            {
                // Log
                this.logger.LogError(e, "Erro Inesperado");
                this.NotificationContext.Add(NotificationKeys.UnexpectedError, "Erro inesperado.");
            }

            return new RequestResponseWrapper<RemoverResponsavelResponse>(false, null);
        }
    }
}