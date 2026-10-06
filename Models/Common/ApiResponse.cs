using System;
using System.Collections.Generic;

namespace VLCTraders.Api.Models.Common
{
    /// <summary>
    /// Standard API Response Contract for all endpoints
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<ApiError> Errors { get; set; } = new List<ApiError>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string TraceId { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ApiError> Errors { get; set; } = new List<ApiError>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string TraceId { get; set; }
    }

    /// <summary>
    /// Validation Error Details
    /// </summary>
    public class ApiError
    {
        public string Field { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public object Value { get; set; }
    }

    /// <summary>
    /// Business Rule Error
    /// </summary>
    public class BusinessRuleError : ApiError
    {
        public string ErrorCode { get; set; }
        public Dictionary<string, object> Details { get; set; } = new Dictionary<string, object>();
    }
}
