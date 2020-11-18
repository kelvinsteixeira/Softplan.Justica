using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Dtos;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Extensions;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Query
{
    public class ObterProcessosQueryHandler : RequestHandlerBase<ObterProcessosQuery, RequestResponseWrapper<PagedList<ProcessoDto>>, IObterProcessosQueryValidator>
    {
        private readonly IProcessoRepository processoRepository;
        private readonly ILogger<ObterProcessosQueryHandler> logger;

        public ObterProcessosQueryHandler(
            IProcessoRepository processoRepository,
            INotificationContext notificationContext,
            IObterProcessosQueryValidator validator,
            ILogger<ObterProcessosQueryHandler> logger) : base(notificationContext, validator)
        {
            this.processoRepository = processoRepository;
            this.logger = logger;
        }

        public override async Task<RequestResponseWrapper<PagedList<ProcessoDto>>> Handle(ObterProcessosQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (await this.ValidateAsync(request))
                {
                    var query = this.processoRepository.Query();

                    if (!string.IsNullOrEmpty(request.NumeroProcesso))
                    {
                        Util.NumeroProcesso numeroProcesso = request.NumeroProcesso;
                        query = query.Where(p => p.NumeroProcesso == numeroProcesso.ToString());
                    }

                    if (request.DataDistribuicaoInicio.HasValue)
                    {
                        query = query.Where(p => p.DataDistribuicao >= request.DataDistribuicaoInicio);
                    }

                    if (request.DataDistribuicaoFim.HasValue)
                    {
                        query = query.Where(p => p.DataDistribuicao <= request.DataDistribuicaoFim);
                    }

                    if (request.SegredoJustica.HasValue)
                    {
                        query = query.Where(p => p.SegredoJustica == request.SegredoJustica.Value);
                    }

                    if (!string.IsNullOrEmpty(request.PastaFisicaCliente))
                    {
                        query = query.Where(p => p.PastaFisicaCliente.ContainsIgnoreCase(request.PastaFisicaCliente));
                    }

                    if (request.SituacaoProcessoId.HasValue)
                    {
                        query = query.Where(p => p.SituacaoId == request.SituacaoProcessoId);
                    }

                    if (!string.IsNullOrEmpty(request.NomeResponsavel))
                    {
                        query = query.Where(p => p.ProcessoResponsaveis.Any(r => r.Responsavel.Nome.ToLower().Contains(request.NomeResponsavel.ToLower())));
                    }

                    var pagedList = PagedList<Processo>.Create(query, request.PageNumber, request.PageSize);
                    var processoDtos = pagedList.Select(p => new ProcessoDto
                    {
                        Id = p.Id,
                        NumeroProcesso = p.NumeroProcesso,
                        DataDistribuicao = p.DataDistribuicao,
                        SegredoJustica = p.SegredoJustica,
                        PastaFisicaCliente = p.PastaFisicaCliente,
                        Descricao = p.Descricao,
                        SituacaoProcessoId = p.SituacaoId,
                        Responsaveis = p.Responsaveis?.Select(r => new ResponsavelDto
                        {
                            Id = r.Id,
                            Cpf = r.Cpf,
                            Email = r.Email,
                            Nome = r.Nome,
                            Foto = r.Foto
                        })
                    });

                    var pagedResult = PagedList<ProcessoDto>.Create(processoDtos.ToList(), pagedList.TotalCount, request.PageNumber, request.PageSize);
                    return new RequestResponseWrapper<PagedList<ProcessoDto>>(true, pagedResult);
                }
            }
            catch (System.Exception e)
            {
                this.logger.LogError(e, "Erro Inesperado");
                this.NotificationContext.Add(NotificationKeys.UnexpectedError, "Erro inesperado.");
            }

            return new RequestResponseWrapper<PagedList<ProcessoDto>>(false, null);
        }
    }
}