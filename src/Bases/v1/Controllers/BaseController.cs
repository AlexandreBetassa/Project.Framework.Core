using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Store.Framework.Core.Bases.v1.Exceptions;
using System.Net;

namespace Store.Framework.Core.Bases.v1.Controllers
{
    public class BaseController<T> : ControllerBase
    {
        public IMediator Mediator { get; set; }
        public ILogger Logger { get; set; }

        public BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }

        public async Task<IActionResult> ExecuteAsync<TResponse>(Func<Task<TResponse>> func, HttpStatusCode statusCode)
        {
            try
            {
                var result = await func();

                if (statusCode.Equals(HttpStatusCode.NoContent))
                    return StatusCode((int)statusCode);

                return StatusCode((int)statusCode, result);
            }
            catch (CustomException ex)
            {
                return StatusCode(
                    ex.StatusCode,
                    JsonConvert.SerializeObject(ex.Message));
            }
            catch (ValidationException ex)
            {
                return StatusCode(400, new { ErrorsMessage = ex.Errors.Select(x => x.ErrorMessage) });
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    JsonConvert.SerializeObject("Ocorreu um erro ao acessar o servidor. " +
                    "Entre em contato com o administrador ou tente novamente mais tarde."));
            }
        }
    }
}
