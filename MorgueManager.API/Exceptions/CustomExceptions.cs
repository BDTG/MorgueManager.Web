using System;
using System.Collections.Generic;

namespace MorgueManager.API.Exceptions;

// Ngoại lệ cơ sở cho toàn ứng dụng
public class AppException : Exception
{
    public int StatusCode { get; }

    public AppException(string message, int statusCode = 500) : base(message)
    {
        StatusCode = statusCode;
    }
}

// Ngoại lệ khi không tìm thấy tài nguyên (HTTP 404)
public class ResourceNotFoundException : AppException
{
    public ResourceNotFoundException(string message) : base(message, 404)
    {
    }
}

// Ngoại lệ khi dữ liệu không hợp lệ (HTTP 400)
public class AppValidationException : AppException
{
    public Dictionary<string, string[]> Errors { get; }

    public AppValidationException(string message) : base(message, 400)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public AppValidationException(string message, Dictionary<string, string[]> errors) : base(message, 400)
    {
        Errors = errors;
    }
}
