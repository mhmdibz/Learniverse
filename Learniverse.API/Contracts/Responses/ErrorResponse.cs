namespace Learniverse.API.Contracts.Responses;

public sealed class ErrorResponse
{
    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public IDictionary<string, string[]>? Errors { get; init; }
}