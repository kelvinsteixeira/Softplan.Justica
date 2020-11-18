using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Events.Handlers
{
    public class ProcessoCriadoEventHandler : IRequestHandler<ProcessoCriadoEvent>
    {
        private readonly IProcessoRepository processoRepository;
        private readonly IEmailService emailService;
        private readonly ILogger<ProcessoCriadoEventHandler> logger;

        public ProcessoCriadoEventHandler(
            IProcessoRepository processoRepository,
            IEmailService emailService,
            ILogger<ProcessoCriadoEventHandler> logger)
        {
            this.processoRepository = processoRepository;
            this.emailService = emailService;
            this.logger = logger;
        }

        public Task<Unit> Handle(ProcessoCriadoEvent request, CancellationToken cancellationToken)
        {
            try
            {
                var processo = this.processoRepository.ObterPorId(request.ProcessoId);
                var emailMessageBuilder = new EmailMessageBuilder();

                foreach (var responsavel in processo.Responsaveis)
                {
                    emailMessageBuilder.To(responsavel.Email);
                }

                var emailMessage = emailMessageBuilder
                    .Subject($"Novo processo - Número: {processo.NumeroProcesso}")
                    .Body($"Você foi cadastrado como envolvido no processo de número {processo.NumeroProcesso}.").Build();

                this.emailService.SendEmail(emailMessage);
            }
            catch (System.Exception e)
            {
                this.logger.LogError(e, "Erro Inesperado");
            }

            return Unit.Task;
        }
    }
}