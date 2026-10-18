using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNSCore.Entities
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
        public List<string>? Errors { get; set; }

        public static Result<T> Success(T data, string message = "Success")
            => new() { IsSuccess = true, Data = data, Message = message };

        public static Result<T> Failure(string message = "Error", List<string>? errors = null, string? errorCode = null)
            => new() { IsSuccess = false, Errors = errors, Message = message, ErrorCode = errorCode };
    }

    public class PagedResult<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyList<T>? Items { get; set; }
    }

    public class ListItems<T>
    {
        public IReadOnlyList<T>? Items { get; set; }
    }
}
