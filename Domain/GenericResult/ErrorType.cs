namespace Domain.GenericResult
{
    public enum ErrorType
    {
        NotFound,
        Validation,
        DatabaseError,
        NullableValue,
        MissingData,
        InvalidData,
        Unauthorized,
        Conflict
    }
}