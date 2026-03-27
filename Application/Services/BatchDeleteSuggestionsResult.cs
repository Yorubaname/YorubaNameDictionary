namespace Application.Services;

public class BatchDeleteSuggestionsResult
{
    public string[] DeletedItems { get; init; } = [];
    public string[] NotFoundItems { get; init; } = [];
}