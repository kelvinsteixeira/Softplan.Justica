using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Softplan.Justica.GerenciadorProcessos.Handlers.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiControllerBase : ControllerBase
    {
        private readonly IApiResultHandler apiResultHandler;
        private readonly ILogger logger;

        protected ApiControllerBase(IApiResultHandler apiResultHandler, ILogger logger)
        {
            this.apiResultHandler = apiResultHandler;
            this.logger = logger;
        }

        protected async Task<IActionResult> ExecAndHandleAsync<TReturnType>(Func<Task<TReturnType>> action, Func<TReturnType, IActionResult> onSuccessActionResultHandler = default)
        {
            if (action == null)
            {
                return this.Ok();
            }

            try
            {
                var result = await action.Invoke();

                var handledResult = this.apiResultHandler.Handle(this, result);
                if (handledResult is OkObjectResult && onSuccessActionResultHandler != null)
                {
                    return onSuccessActionResultHandler.Invoke(result);
                }

                return handledResult;
            }
            catch (Exception e)
            {
                // Log
                this.logger.LogError(e, "Error");
                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        protected async Task<IActionResult> ExecAndHandleAsync(Func<Task> action, Func<IActionResult> onSuccessActionResultHandler = default)
        {
            if (action == null)
            {
                return this.Ok();
            }

            try
            {
                await action.Invoke();

                var handledResult = this.apiResultHandler.Handle(this);
                if (handledResult is OkResult && onSuccessActionResultHandler != null)
                {
                    return onSuccessActionResultHandler.Invoke();
                }

                return handledResult;
            }
            catch (Exception e)
            {
                // Log
                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        protected void EnsurePaginationMetadata(IPagedList pagedList, string routeName)
        {
            string nextPage = null;
            string previousPage = null;

            if (pagedList.HasNext)
            {
                nextPage = Url.Link(routeName, new { pageNumber = pagedList.CurrentPage + 1, pageSize = pagedList.PageSize });
            }

            if (pagedList.HasPrevious)
            {
                previousPage = Url.Link(routeName, new { pageNumber = pagedList.CurrentPage - 1, pageSize = pagedList.PageSize });
            }

            var metadata = new PaginationMetadata(pagedList.TotalCount, pagedList.PageSize, pagedList.CurrentPage, pagedList.TotalPages, nextPage, previousPage);
            this.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }
    }
}