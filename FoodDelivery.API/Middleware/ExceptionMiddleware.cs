using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;

namespace FoodDelivery.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ValidationException => HttpStatusCode.BadRequest,
                ArgumentException or ArgumentNullException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = GetErrorMessage(exception),
                // В режимі розробки додаємо детальну інформацію про помилку
                DetailedMessage = _environment.IsDevelopment() ? exception.StackTrace : null,
                Errors = exception is ValidationException validationException ?
                    validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) : null
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }

        private string GetErrorMessage(Exception exception)
        {
            // Для ValidationException повертаємо спеціальне повідомлення
            if (exception is ValidationException)
            {
                return "Помилка валідації даних";
            }

            // Для виробничого середовища не показуємо деталі помилок
            return _environment.IsDevelopment() ?
                exception.Message :
                "Сталася внутрішня помилка сервера. Будь ласка, спробуйте пізніше.";
        }
    }
}