namespace Learniverse.Application.Interfaces.Email;

public sealed record EmailMessage(
    EmailPurpose Purpose,
    string ToEmail,
    string Subject,
    string TextBody,
    string HtmlBody);
