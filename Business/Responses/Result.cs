namespace Business.Responses
{
 
    public class Result<T>
    {
        public T? Data { get; set; } 
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static Result<T> SuccessResult(T data, string message = "")
        {
            return new Result<T> { Success = true, Data = data, Message = message };
        }

        public static Result<T> ErrorResult(string message)
        {
            return new Result<T> { Success = false, Data = default, Message = message };
        }
    }
}