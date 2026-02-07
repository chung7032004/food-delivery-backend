using System.ComponentModel.DataAnnotations;
using System.Security;
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
        if (exception is AggregateException agg) exception = agg.Flatten().InnerException ?? agg;
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
            case SecurityException:
                statusCode = StatusCodes.Status403Forbidden;
                errorCode = "FORBIDDEN";
                message = "Không có quyền truy cập.";
                break;
            case UnauthorizedAccessException:
                statusCode = StatusCodes.Status401Unauthorized;
                errorCode = "UNAUTHORIZED";
                message = "Yêu cầu xác thực.";
                break;    
            case KeyNotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                errorCode = "NOT_FOUND";
                message = exception.Message;
                break;
            //tham số không hợp lệ như truyền sai format
            case ArgumentException:
                statusCode = StatusCodes.Status400BadRequest;
                errorCode = "INVALID_ARGUMENT";
                message = exception.Message;
                break;
            case JsonException:
                statusCode = StatusCodes.Status400BadRequest;
                errorCode = "INVALID_JSON";
                message = "Dữ liệu JSON không hợp lệ.";
                break;
            case ValidationException:
                statusCode = StatusCodes.Status400BadRequest;
                errorCode = "VALIDATION_ERROR";
                message = exception.Message;
                break;
            case OperationCanceledException:
                statusCode = StatusCodes.Status408RequestTimeout;
                errorCode = "CANCELED";
                message = "Yêu cầu đã bị hủy.";
                break;
            case DbUpdateException:
                statusCode = StatusCodes.Status500InternalServerError;
                errorCode = "DATABASE_ERROR";
                message = "Lỗi cơ sở dữ liệu.";
                break;
            case IOException:
                statusCode = StatusCodes.Status503ServiceUnavailable;
                errorCode = "IO_ERROR";
                message = "Lỗi hệ thống, vui lòng thử lại sau.";
                break;
            case NotImplementedException:
                statusCode = StatusCodes.Status501NotImplemented;
                errorCode = "NOT_IMPLEMENTED";
                message = "Chức năng chưa được triển khai.";
                break;
        }
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning("Handled exception: {Message}", message);
        }
        var result = Result.Failure(errorCode, message);
        var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        var jsonString = JsonSerializer.Serialize(result,jsonOptions);
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(jsonString);  
    }
}