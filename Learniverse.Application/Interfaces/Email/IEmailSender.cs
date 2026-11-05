namespace Learniverse.Application.Interfaces.Email;

public interface IEmailSender
{
    Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken);
}
