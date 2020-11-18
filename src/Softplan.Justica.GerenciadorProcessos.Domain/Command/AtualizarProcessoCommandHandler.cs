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
    public class AtualizarProcessoCommandHandler : RequestHandlerBase<AtualizarProcessoCommand, RequestResponseWrapper<AtualizarProcessoResponse>, IAtualizarProcessoCommandValidator>
    {
        private readonly IProcessoRepository processoRepository;
        private readonly IResponsavelRepository responsavelRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<AtualizarProcessoCommandHandler> logger;

        public AtualizarProcessoCommandHandler(
            IProcessoRepository processoRepository,
            IResponsavelRepository responsavelRepository,
            IAtualizarProcessoCommandValidator validator,
            INotificationContext notificationContext,
            IUnitOfWork unitOfWork,
            ILogger<AtualizarProcessoCommandHandler> logger) : base(notificationContext, validator)
        {
            this.processoRepository = processoRepository;
            this.responsavelRepository = responsavelRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public override async Task<RequestResponseWrapper<AtualizarProcessoResponse>> Handle(AtualizarProcessoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await this.ValidateAsync(request))
                {
                    var processo = this.processoRepository.ObterPorId(request.Id.Value);

                    var novosResponsaveis = request.ResponsaveisIds?.Where(id => processo.Responsaveis?.Any(r => r.Id == id) == false).ToList();

                    processo.DataDistribuicao = request.DataDistribuicao;
                    processo.Descricao = request.Descricao;
                    processo.NumeroProcesso = request.NumeroProcesso;
                    processo.PastaFisicaCliente = request.PastaFisicaCliente;
                    processo.ProcessoVinculadoId = request.ProcessoVinculadoId;
                    processo.SituacaoId = request.SituacaoProcessoId;
                    processo.SegredoJustica = request.SegredoJustica;

                    var responsaveis = this.responsavelRepository.Obter(r => request.ResponsaveisIds.Contains(r.Id)).ToList();
                    processo.AtribuirResponsaveis(responsaveis);

                    this.processoRepository.Atualizar(processo);
                    await this.unitOfWork.SaveChangesAsync();

                    return new RequestResponseWrapper<AtualizarProcessoResponse>(true, new AtualizarProcessoResponse(processo.Id, novosResponsaveis));
                }
            }
            catch (System.Exception e)
            {
                this.logger.LogError(e, "Erro Inesperado");
                this.NotificationContext.Add(NotificationKeys.UnexpectedError, "Erro inesperado.");
            }

            return new RequestResponseWrapper<AtualizarProcessoResponse>(false, null);
        }
    }
}