using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Events.Handlers
{
    public class ProcessoRemovidoEventHandler : IRequestHandler<ProcessoRemovidoEvent>
    {
        private readonly IResponsavelRepository responsavelRepository;
        private readonly IEmailService emailService;
        private readonly ILogger<ProcessoRemovidoEventHandler> logger;

        public ProcessoRemovidoEventHandler(
            IResponsavelRepository responsavelRepository,
            IEmailService emailService,
            ILogger<ProcessoRemovidoEventHandler> logger)
        {
            this.responsavelRepository = responsavelRepository;
            this.emailService = emailService;
            this.logger = logger;
        }

        public Task<Unit> Handle(ProcessoRemovidoEvent request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.ResponsaveisIds?.Any() == true)
                {
                    var emailMessageBuilder = new EmailMessageBuilder();

                    foreach (var responsavelId in request.ResponsaveisIds)
                    {
                        var responsavel = this.responsavelRepository.ObterPorId(responsavelId);
                        emailMessageBuilder.To(responsavel.Email.ToString());
                    }

                    var emailMessage = emailMessageBuilder
                        .Subject($"Processo Removido - Número: {request.NumeroProcesso}")
                        .Body($"O processo de número {request.NumeroProcesso} foi removido.").Build();

                    this.emailService.SendEmail(emailMessage);
                }
            }
            catch (System.Exception e)
            {
                this.logger.LogError(e, "Erro Inesperado");
            }

            return Unit.Task;
        }
    }
}