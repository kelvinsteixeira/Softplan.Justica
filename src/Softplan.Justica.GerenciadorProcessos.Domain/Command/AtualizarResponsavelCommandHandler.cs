using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Command
{
    public class AtualizarResponsavelCommandHandler : RequestHandlerBase<AtualizarResponsavelCommand, RequestResponseWrapper, IAtualizarResponsavelCommandValidator>
    {
        private readonly IResponsavelRepository responsavelRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<AtualizarResponsavelCommandHandler> logger;

        public AtualizarResponsavelCommandHandler(
            IResponsavelRepository responsavelRepository,
            INotificationContext notificationContext,
            IAtualizarResponsavelCommandValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<AtualizarResponsavelCommandHandler> logger) : base(notificationContext, validator)
        {
            this.responsavelRepository = responsavelRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public override async Task<RequestResponseWrapper> Handle(AtualizarResponsavelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await this.ValidateAsync(request))
                {
                    var responsavel = this.responsavelRepository.ObterPorId(request.Id.Value);
                    Util.Cpf cpf = request.Cpf;
                    responsavel.Cpf = cpf.ToString();
                    responsavel.Email = request.Email;
                    responsavel.Foto = request.Foto;
                    responsavel.Nome = request.Nome;

                    this.responsavelRepository.Atualizar(responsavel);
                    await this.unitOfWork.SaveChangesAsync();

                    return new RequestResponseWrapper(true);
                }
            }
            catch (System.Exception e)
            {
                this.logger.LogError(e, "Erro Inesperado");
                this.NotificationContext.Add(NotificationKeys.UnexpectedError, "Erro inesperado.");
            }

            return new RequestResponseWrapper(false);
        }
    }
}