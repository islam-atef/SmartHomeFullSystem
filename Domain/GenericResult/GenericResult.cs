namespace Domain.GenericResult
{
    public class GenericResult<T> 
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? ErrorMessage { get; } = string.Empty;
        public string? SucessMessage { get; } = string.Empty;
        public ErrorType ErrorType { get; }

        private GenericResult(T? value, string? successMessage = "")
        {
            IsSuccess = true;
            Value = value;
            SucessMessage = successMessage;
        }

        private GenericResult(ErrorType errorType, string? errorMessage)
        {
            IsSuccess = false;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }

        public static GenericResult<T> Success(T? value,string? successMessage = "") => new(value , successMessage);
        public static GenericResult<T> Failure(ErrorType errorType, string? errorMessage = "") => new(errorType, errorMessage);
    }
}