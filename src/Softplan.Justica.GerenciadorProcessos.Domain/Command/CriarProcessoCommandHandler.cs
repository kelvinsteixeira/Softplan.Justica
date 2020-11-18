using System.Linq;
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
    public class CriarProcessoCommandHandler : RequestHandlerBase<CriarProcessoCommand, RequestResponseWrapper<int?>, ICriarProcessoCommandValidator>
    {
        private readonly IProcessoRepository processoRepository;
        private readonly IResponsavelRepository responsavelRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CriarProcessoCommandHandler> logger;

        public CriarProcessoCommandHandler(
            IProcessoRepository processoRepository,
            IResponsavelRepository responsavelRepository,
            ICriarProcessoCommandValidator commandValidator,
            INotificationContext notificationContext,
            IUnitOfWork unitOfWork,
            ILogger<CriarProcessoCommandHandler> logger) : base(notificationContext, commandValidator)
        {
            this.processoRepository = processoRepository;
            this.responsavelRepository = responsavelRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public override async Task<RequestResponseWrapper<int?>> Handle(CriarProcessoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await this.ValidateAsync(request))
                {
                    var processo = new Processo();
                    processo.DataDistribuicao = request.DataDistribuicao;
                    processo.Descricao = request.Descricao;
                    processo.NumeroProcesso = request.NumeroProcesso;
                    processo.PastaFisicaCliente = request.PastaFisicaCliente;
                    processo.ProcessoVinculadoId = request.ProcessoVinculadoId;
                    processo.SituacaoId = request.SituacaoProcessoId;
                    processo.SegredoJustica = request.SegredoJustica;

                    var responsaveis = this.responsavelRepository.Obter(r => request.ResponsaveisIds.Contains(r.Id)).ToList();
                    processo.AtribuirResponsaveis(responsaveis);

                    this.processoRepository.Criar(processo);
                    await this.unitOfWork.SaveChangesAsync();

                    return new RequestResponseWrapper<int?>(true, processo.Id);
                }
            }
            catch (System.Exception e)
            {
                // Log
                this.logger.LogError(e, "Erro Inesperado");
                this.NotificationContext.Add(NotificationKeys.UnexpectedError, "Erro inesperado.");
            }

            return new RequestResponseWrapper<int?>(false, null);
        }
    }
}