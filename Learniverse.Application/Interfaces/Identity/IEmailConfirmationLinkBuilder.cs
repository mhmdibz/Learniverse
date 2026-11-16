namespace Learniverse.Application.Interfaces.Identity;

public interface IEmailConfirmationLinkBuilder
{
    string Build(string email, string token);
}
