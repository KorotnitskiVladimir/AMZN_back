namespace AMZN.DTOs.Common
{
    public class ApiErrorResponse
    {
        public string Code { get; init; } = "";
        public string Message { get; init; } = "";
        public string? TraceId { get; init; }        // id запроса (request id), помогает найти этот конкретный ответ/ошибку в логах
    }
}
