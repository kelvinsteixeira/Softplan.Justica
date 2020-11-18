using System;
using System.Collections.Generic;
using System.Text;
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
    public class ObterResponsavelQueryHandler : RequestHandlerBase<ObterResponsavelQuery, RequestResponseWrapper<ResponsavelDto>, IObterResponsavelQueryValidator>
    {
        private readonly IResponsavelRepository responsavelRepository;
        private readonly ILogger<ObterResponsavelQueryHandler> logger;

        public ObterResponsavelQueryHandler(
            IResponsavelRepository responsavelRepository,
            INotificationContext notificationContext,
            IObterResponsavelQueryValidator validator,
            ILogger<ObterResponsavelQueryHandler> logger) : base(notificationContext, validator)
        {
            this.responsavelRepository = responsavelRepository;
            this.logger = logger;
        }

        public override async Task<RequestResponseWrapper<ResponsavelDto>> Handle(ObterResponsavelQuery request, CancellationToken cancellationToken)
        {
            if (await this.ValidateAsync(request))
            {
                try
                {
                    ResponsavelDto result = null;

                    var responavel = this.responsavelRepository.ObterPorId(request.Id.Value);
                    if (responavel != null)
                    {
                        result = new ResponsavelDto
                        {
                            Id = responavel.Id,
                            Cpf = responavel.Cpf,
                            Email = responavel.Email,
                            Foto = responavel.Foto,
                            Nome = responavel.Nome
                        };
                    }

                    return new RequestResponseWrapper<ResponsavelDto>(true, result);
                }
                catch (Exception e)
                {
                    // Log
                    this.logger.LogError(e, "Erro Inesperado");
                    this.NotificationContext.Add(NotificationKeys.UnexpectedError, "Erro inesperado.");
                }
            }

            return new RequestResponseWrapper<ResponsavelDto>(false, null);
        }
    }
}
