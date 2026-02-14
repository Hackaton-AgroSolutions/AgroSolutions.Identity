namespace AgroSolutions.Identity.API.Responses;

public record RestResponseWithInvalidFields
{
    public IReadOnlyDictionary<string, string[]> InvalidFields { get; init; } = new Dictionary<string, string[]>();
}
