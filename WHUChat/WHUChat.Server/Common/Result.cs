namespace WHUChat.Server.Common
{
    public class Result<T>
    {
        public int Code { get; set; } = 1; // 1: 成功, 0: 失败
        public string Message { get; set; } = "Success";
        public T Data { get; set; }

        public static Result<T> Ok(T data, string message = "Success") => new Result<T> { Data = data, Message = message };
        public static Result<T> Fail(string message) => new Result<T> { Code = 0, Message = message, Data = default };
    }
}

