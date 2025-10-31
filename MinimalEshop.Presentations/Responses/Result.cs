using System.Collections.Generic;

namespace MinimalEshop.Presentation.Responses
{
    public class Result
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public List<string>? Errors { get; init; }
        public int StatusCode { get; init; }
        public object? Payload { get; init; }

        public static Result Fail(IEnumerable<string>? errors = null, string? message = null, int statusCode = 400) =>
            new Result { Success = false, Errors = errors is null ? null : new List<string>(errors), Message = message, StatusCode = statusCode };

        public static Result Ok(object? payload = null, string? message = null, int statusCode = 200) =>
            new Result { Success = true, Payload = payload, Message = message, StatusCode = statusCode };
    }

    public class Result<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public List<string>? Errors { get; init; }
        public int StatusCode { get; init; }
        public T? Payload { get; init; }

        public static Result<T> Fail(IEnumerable<string>? errors = null, string? message = null, int statusCode = 400) =>
            new Result<T> { Success = false, Errors = errors is null ? null : new List<string>(errors), Message = message, StatusCode = statusCode };

        public static Result<T> Ok(T? payload = default, string? message = null, int statusCode = 200) =>
            new Result<T> { Success = true, Payload = payload, Message = message, StatusCode = statusCode };
    }
}
