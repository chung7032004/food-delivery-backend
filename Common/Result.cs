namespace FoodDelivery.Common;
public class Result
{
    public bool IsSuccess {get;}
    public string? ErrorCode {get;}
    public string? Message {get;}
    protected Result(bool success, string? errorCode, string? message)
    {   
        IsSuccess = success;
        ErrorCode = errorCode;
        Message = message;
    }
    public static Result Success()
    {
        return new Result( 
            success: true,
            errorCode: null,
            message: null
        );
    }

    public static Result Failure(string errorCode,string message)
    {
        return new Result(
            success: false,
            errorCode: errorCode,
            message:message
        );
    }  
}

public class Result<T> : Result
{
    public T? Data {get;}
    private Result (bool success, T? data, string? errorCode, string? message)
        : base(success, errorCode, message)
    {
        Data = data;
    }
    public static Result<T> Success(T data)
    {
        return new Result<T>(
        success: true,
        data: data,
        errorCode: null,
        message: null);
    } 
    public static new Result<T> Failure(string errorCode, string message)
    {
        return new Result<T>(
        success: false,
        data: default,
        errorCode: errorCode,
        message: message
    );
    }
} 