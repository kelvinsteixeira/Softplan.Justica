using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Handlers.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Models;

namespace Softplan.Justica.GerenciadorProcessos.Handlers
{
    public class ApiResultHandler : IApiResultHandler
    {
        private readonly INotificationContext notificationContext;

        public ApiResultHandler(INotificationContext notificationContext)
        {
            this.notificationContext = notificationContext;
        }

        public IActionResult Handle(ControllerBase controller, object result = default)
        {
            if (controller == null)
            {
                return null;
            }

            if (!this.notificationContext.HasNotifications)
            {
                return controller.Ok(result);
            }

            var errorResponse = new ErrorResponseWrapper<object>();
            errorResponse.Value = result;
            errorResponse.Errors = new List<KeyValuePair<string, string>>(this.notificationContext.Notifications.Select(n => new KeyValuePair<string, string>(n.Key, n.Message)));
            errorResponse.ErrorMessage = "Um ou mais erros ocorreram";

            if (this.notificationContext.Notifications.Any(n => n.Key == NotificationKeys.NotFound))
            {
                return controller.NotFound(errorResponse);
            }
            else if (this.notificationContext.Notifications.Any(n => n.Key == NotificationKeys.InvalidArgument))
            {
                return controller.UnprocessableEntity(errorResponse);
            }
            else if (this.notificationContext.Notifications.Any(n => n.Key == NotificationKeys.AlreadyExists))
            {
                return controller.Conflict(errorResponse);
            }

            return controller.StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }
}