namespace Learniverse.API.Contracts.Requests.Payments;

public sealed record StartPaymentRequest(
    Guid CourseId,
    string PhoneNumber);