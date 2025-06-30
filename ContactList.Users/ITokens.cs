using ContactList.Shared.Models;

namespace ContactList.Users
{
    public interface ITokens
    {
        string GenerateToken(User user);

        ValidateToken ValidateCurrentToken(string token);

        ValidateToken GetClaim(string token, string claimType, string claimValue);

        ValidateToken DemandClaim(string token, string claimType);

    }
}