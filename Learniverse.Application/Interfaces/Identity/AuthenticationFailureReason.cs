namespace Learniverse.Application.Interfaces.Identity;

public enum AuthenticationFailureReason
{
    InvalidCredentials,
    LockedOut,
    NotAllowed,
    RequiresTwoFactor
}