namespace Inventory.Api.Common
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Msg { get; set; } = "";
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T? data, string msg = "OK", int code = 200)
            => new() { Code = code, Msg = msg, Data = data };

        public static ApiResponse<T> Created(T? data, string msg = "Created")
            => new() { Code = 201, Msg = msg, Data = data };

        public static ApiResponse<T> Fail(int code, string msg)
            => new() { Code = code, Msg = msg, Data = default };
    }

    public static class ApiResponse
    {
        public static ApiResponse<T> Success<T>(T? data, string msg = "OK", int code = 200)
            => ApiResponse<T>.Success(data, msg, code);
        public static ApiResponse<T> Created<T>(T? data, string msg = "Created")
            => ApiResponse<T>.Created(data, msg);
        public static ApiResponse<T> Fail<T>(int code, string msg)
            => ApiResponse<T>.Fail(code, msg);
    }
}