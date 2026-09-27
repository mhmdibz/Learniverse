namespace Learniverse.API.Contracts.Requests;

public sealed record CreateSectionRequest(
    string Title,
    string Description,
    int Order
);