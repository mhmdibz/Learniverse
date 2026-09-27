namespace Learniverse.Application.Interfaces.Identity;

public interface ITokenService
{
    TokenResult GenerateToken(AuthenticatedUser user);
}