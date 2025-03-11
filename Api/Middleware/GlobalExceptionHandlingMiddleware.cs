namespace Api.Middleware
{
    using Api.Utilities;
    using System.Net;
    using System.Text.Json;
    using YorubaOrganization.Application.Exceptions;

    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            Dictionary<string, string> errorResponse;
            switch (exception)
            {
                case ClientException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = ResponseHelper.GetResponseDict(ex.Message);
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = ResponseHelper.GetResponseDict("Internal server error!");
                    break;
            }
            _logger.LogError(exception, "Unhandled Application Exception");
            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}
