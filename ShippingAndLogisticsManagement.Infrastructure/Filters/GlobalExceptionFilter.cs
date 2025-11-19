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
            if (context.Exception is BusinessException businessException)
            {
                var validation = new
                {
                    Status = 400,
                    Title = "BadRequest",
                    Detail = businessException.Message
                };
                var json = new
                {
                    Errors = new[] { validation }
                };

                context.Result = new BadRequestObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.ExceptionHandled = true;
            }
            else
            {
                // Para otras excepciones, retornar 500
                var validation = new
                {
                    Status = 500,
                    Title = "InternalServerError",
                    Detail = context.Exception.Message
                };
                var json = new
                {
                    Errors = new[] { validation }
                };

                context.Result = new ObjectResult(json)
                {
                    StatusCode = 500
                };
                context.HttpContext.Response.StatusCode = 500;
                context.ExceptionHandled = true;
            }
        }
    }
}