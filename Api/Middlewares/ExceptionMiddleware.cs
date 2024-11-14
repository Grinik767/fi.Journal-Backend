using System.Net;
using Api.Contracts;

namespace Api.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            await HandleExceptionAsync(context, HttpStatusCode.NotFound, "Object not found");
        }
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        var errorResponse = new ErrorResponse(context.Response.StatusCode, message);

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}