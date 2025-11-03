using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShippingAndLogisticsManagement.Core.Exceptions;
using System.Net;

namespace ShippingAndLogisticsManagement.Infrastructure.Filters
{
    public class GlobalExceptionFilter: IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = (BusinessException)context.Exception;
            var validation = new
            {
                Status = 400,
                Title = "BadRequest",
                Detail = exception.Message
            };
            var json = new
            {
                Errors = new[] { validation }
            };

            context.Result = new BadRequestObjectResult(json);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.ExceptionHandled = true;
        }
    }
}
