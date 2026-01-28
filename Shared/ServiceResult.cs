namespace AMZN.Shared
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public string? ErrorCode { get; }
        public T? Data { get; }

        private ServiceResult(bool isSuccess, T? data, string? errorMessage, string? errorCode = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        public static ServiceResult<T> Ok(T data)
        {
            return new ServiceResult<T>(true, data, null);
        }

        public static ServiceResult<T> Fail(string errorMessage, string? errorCode = null)
        {
            return new ServiceResult<T>(false, default, errorMessage, errorCode);
        }
    }
}
