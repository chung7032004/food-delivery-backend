using System.Text.Json;
using FoodDelivery.Common;
using Microsoft.EntityFrameworkCore;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }   
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var statusCode = StatusCodes.Status500InternalServerError;
        var errorCode = "INTERNAL_SERVER_ERROR";
        var message = "Đã có lỗi xảy ra, vui lòng thử lại sau.";
        switch (exception)
        {
            case AppException appException:
                statusCode = appException.StatusCode;
                errorCode = appException.ErrorCode;
                message = appException.Message;
                break;
            case UnauthorizedAccessException:
                statusCode = StatusCodes.Status401Unauthorized;
                errorCode = "UNAUTHORIZED";
                message = "Yêu cầu xác thực.";
                break;
            case DbUpdateException:
                statusCode = StatusCodes.Status400BadRequest;
                errorCode = "DATABASE_UPDATE_ERROR";
                message = "Lỗi cập nhật cơ sở dữ liệu.";
                break;
            case IOException:
                statusCode = StatusCodes.Status503ServiceUnavailable;
                errorCode = "IO_ERROR";
                message = "Lỗi hệ thống, vui lòng thử lại sau.";
                break;
        }
        if (statusCode == 500)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");
        }
        var result = Result.Failure(errorCode, message);
        var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        var jsonString = JsonSerializer.Serialize(result,jsonOptions);
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(jsonString);  
    }
}