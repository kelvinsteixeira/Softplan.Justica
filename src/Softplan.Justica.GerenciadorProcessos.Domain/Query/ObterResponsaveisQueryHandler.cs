using System;
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
    public class ObterResponsaveisQueryHandler : RequestHandlerBase<ObterResponsaveisQuery, RequestResponseWrapper<PagedList<ResponsavelDto>>, IObterResponsaveisQueryValidator>
    {
        private readonly IResponsavelRepository responsavelRepository;
        private readonly ILogger<ObterResponsaveisQueryHandler> logger;

        public ObterResponsaveisQueryHandler(
            IResponsavelRepository responsavelRepository,
            INotificationContext notificationContext,
            IObterResponsaveisQueryValidator validator,
            ILogger<ObterResponsaveisQueryHandler> logger) : base(notificationContext, validator)
        {
            this.responsavelRepository = responsavelRepository;
            this.logger = logger;
        }

        public override async Task<RequestResponseWrapper<PagedList<ResponsavelDto>>> Handle(ObterResponsaveisQuery request, CancellationToken cancellationToken)
        {
            if (await this.ValidateAsync(request))
            {
                try
                {
                    var query = this.responsavelRepository.Query();

                    if (!string.IsNullOrWhiteSpace(request.Nome))
                    {
                        query = query.Where(r => r.Nome.ContainsIgnoreCase(request.Nome));
                    }

                    if (!string.IsNullOrWhiteSpace(request.Cpf))
                    {
                        Util.Cpf cpf = request.Cpf;
                        query = query.Where(r => r.Cpf == cpf.ToString());
                    }

                    if (!string.IsNullOrWhiteSpace(request.NumeroProcesso))
                    {
                        query = query.Where(r => r.Processos.Any(p => p.NumeroProcesso.Contains(request.NumeroProcesso)));
                    }

                    var pagedList = PagedList<Responsavel>.Create(query, request.PageNumber, request.PageSize);
                    var responsavelDtos = pagedList.Select(r => new ResponsavelDto
                    {
                        Id = r.Id,
                        Cpf = r.Cpf,
                        Email = r.Email,
                        Foto = r.Foto,
                        Nome = r.Nome
                    });

                    var pagedResult = PagedList<ResponsavelDto>.Create(responsavelDtos.ToList(), pagedList.TotalCount, request.PageNumber, request.PageSize);
                    return new RequestResponseWrapper<PagedList<ResponsavelDto>>(true, pagedResult);
                }
                catch (Exception e)
                {
                    // Log
                    this.logger.LogError(e, "Erro Inesperado");
                    this.NotificationContext.Add(NotificationKeys.UnexpectedError, "Erro inesperado.");
                }
            }

            return new RequestResponseWrapper<PagedList<ResponsavelDto>>(false, null);
        }
    }
}