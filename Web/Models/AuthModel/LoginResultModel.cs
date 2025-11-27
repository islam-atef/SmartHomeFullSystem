namespace Web.Models.AuthModel
{
    public record LoginResultModel(bool IsSuccess, Guid? OtoQuestionId = default, string? Errors = null);
}
