namespace Learniverse.API.Contracts.Requests;

public sealed record UpdateSectionRequest(
    string Title,
    string Description,
    int Order
);