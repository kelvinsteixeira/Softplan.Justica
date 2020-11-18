using System;
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
    public class CriarResponsavelCommandHandler : RequestHandlerBase<CriarResponsavelCommand, RequestResponseWrapper<int?>, ICriarResponsavelCommandValidator>
    {
        private readonly IResponsavelRepository responsavelRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CriarResponsavelCommandHandler> logger;

        public CriarResponsavelCommandHandler(
            IResponsavelRepository responsavelRepository,
            INotificationContext notificationContext,
            ICriarResponsavelCommandValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<CriarResponsavelCommandHandler> logger) : base(notificationContext, validator)
        {
            this.responsavelRepository = responsavelRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public override async Task<RequestResponseWrapper<int?>> Handle(CriarResponsavelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await this.ValidateAsync(request))
                {
                    Util.Cpf cpf = request.Cpf;

                    var responsavel = new Responsavel
                    {
                        Cpf = cpf.ToString(),
                        Email = request.Email,
                        Nome = request.Nome,
                        Foto = request.Foto
                    };

                    this.responsavelRepository.Criar(responsavel);
                    await this.unitOfWork.SaveChangesAsync();

                    return new RequestResponseWrapper<int?>(true, responsavel.Id);
                }
            }
            catch (Exception e)
            {
                // Log
                this.logger.LogError(e, "Erro Inesperado");
                this.NotificationContext.Add(NotificationKeys.UnexpectedError, "Erro inesperado.");
            }

            return new RequestResponseWrapper<int?>(false, null);
        }
    }
}