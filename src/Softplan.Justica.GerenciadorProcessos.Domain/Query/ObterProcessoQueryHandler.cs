using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Query
{
    public class ObterProcessoQueryHandler : RequestHandlerBase<ObterProcessoQuery, RequestResponseWrapper<ProcessoDto>, IObterProcessoQueryValidator>
    {
        private readonly IProcessoRepository processoRepository;
        private readonly ILogger<ObterProcessoQueryHandler> logger;

        public ObterProcessoQueryHandler(
            IProcessoRepository processoRepository,
            INotificationContext notificationContext,
            IObterProcessoQueryValidator validator,
            ILogger<ObterProcessoQueryHandler> logger) : base(notificationContext, validator)
        {
            this.processoRepository = processoRepository;
            this.logger = logger;
        }

        public override async Task<RequestResponseWrapper<ProcessoDto>> Handle(ObterProcessoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (await this.ValidateAsync(request))
                {
                    ProcessoDto result = null;

                    var processo = this.processoRepository.ObterPorId(request.Id.Value);
                    if (processo != null)
                    {
                        result = new ProcessoDto
                        {
                            Id = processo.Id,
                            DataDistribuicao = processo.DataDistribuicao,
                            Descricao = processo.Descricao,
                            NumeroProcesso = processo.NumeroProcesso,
                            PastaFisicaCliente = processo.PastaFisicaCliente,
                            SegredoJustica = processo.SegredoJustica,
                            SituacaoProcessoId = processo.SituacaoId,
                            Responsaveis = processo.Responsaveis.Select(r => new ResponsavelDto
                            {
                                Id = r.Id,
                                Cpf = r.Cpf,
                                Email = r.Email,
                                Foto = r.Foto,
                                Nome = r.Nome
                            })
                        };
                    }

                    return new RequestResponseWrapper<ProcessoDto>(true, result);
                }
            }
            catch (Exception e)
            {
                // Log
                this.logger.LogError(e, "Erro Inesperado");
                this.NotificationContext.Add(NotificationKeys.UnexpectedError, "Erro inesperado.");
            }

            return new RequestResponseWrapper<ProcessoDto>(false, null);
        }
    }
}