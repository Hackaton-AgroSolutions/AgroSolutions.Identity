namespace AgroSolutions.Identity.API.Responses;

public record RestResponse
{
    public object? Data { get; init; }
    public IEnumerable<string> Notifications { get; init; } = [];
};
