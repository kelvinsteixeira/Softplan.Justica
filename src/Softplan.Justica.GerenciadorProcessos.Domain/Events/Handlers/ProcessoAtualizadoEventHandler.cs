using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Events.Handlers
{
    public class ProcessoAtualizadoEventHandler : IRequestHandler<ProcessoAtualizadoEvent>
    {
        private readonly IProcessoRepository processoRepository;
        private readonly IEmailService emailService;
        private readonly IEmailMessageBuilder emailMessageBuilder;
        private readonly ILogger<ProcessoAtualizadoEventHandler> logger;

        public ProcessoAtualizadoEventHandler(
            IProcessoRepository processoRepository,
            IEmailService emailService,
            IEmailMessageBuilder emailMessageBuilder,
            ILogger<ProcessoAtualizadoEventHandler> logger)
        {
            this.processoRepository = processoRepository;
            this.emailService = emailService;
            this.emailMessageBuilder = emailMessageBuilder;
            this.logger = logger;
        }

        public Task<Unit> Handle(ProcessoAtualizadoEvent request, CancellationToken cancellationToken)
        {
            try
            {

                var processo = this.processoRepository.ObterPorId(request.ProcessoId);

                foreach (var responsavel in processo.Responsaveis)
                {
                    this.emailMessageBuilder.To(responsavel.Email.ToString());
                }

                var emailMessage = this.emailMessageBuilder
                    .Subject($"Atualização Processo - Número: {processo.NumeroProcesso}")
                    .Body($"Você foi adicionado como envolvido no processo de número {processo.NumeroProcesso}.").Build();

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