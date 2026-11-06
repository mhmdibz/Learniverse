namespace Learniverse.Application.Interfaces.Identity;

public interface IPasswordResetLinkBuilder
{
    string Build(string email, string token);
}
