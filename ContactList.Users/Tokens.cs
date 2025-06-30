using ContactList.Shared.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContactList.Users;

public class Tokens : ITokens
{
    private const string MySecret = "aadawdawdadkadkajdhakdjadadjawdjawdaa";
    private const string MyIssuer = "dadawdawdawdaadada";
    private const string MyAudience = "dadawdwadawdawdawdawda";

    public string GenerateToken(User user)
    {
        SymmetricSecurityKey mySecurityKey = new(Encoding.ASCII.GetBytes(MySecret));
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim("id", user.Id.ToString())
        }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = MyIssuer,
            Audience = MyAudience,
            SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };

    
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ValidateToken ValidateCurrentToken(string token)
    {
        SymmetricSecurityKey mySecurityKey = new(Encoding.ASCII.GetBytes(MySecret));
        JwtSecurityTokenHandler tokenHandler = new();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = MyIssuer,
                ValidAudience = MyAudience,
                IssuerSigningKey = mySecurityKey
            }, out SecurityToken _);
        }
        catch (SecurityTokenExpiredException)
        {
            return ValidateToken.TimeExpired;
        }
        catch
        {
            return ValidateToken.Invalid;
        }
        return ValidateToken.Valid;
    }

    public ValidateToken GetClaim(string token, string claimType, string claimValue)
    {
        ValidateToken validate = ValidateCurrentToken(token);
        switch (validate)
        {
            case ValidateToken.Valid:
                {
                    JwtSecurityTokenHandler tokenHandler = new();
                    JwtSecurityToken securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                    if (securityToken != null)
                    {
                        Claim first = null;
                        foreach (Claim claim in securityToken.Claims)
                        {
                            if (claim.Type == claimType)
                            {
                                first = claim;
                                break;
                            }
                        }

                        if (first != null)
                        {
                            string stringClaimValue = first.Value;
                            if (stringClaimValue == claimValue)
                            {
                                return ValidateToken.Valid;
                            }
                        }
                    }
                }
                break;
            case ValidateToken.TimeExpired:
                return ValidateToken.TimeExpired;
            case ValidateToken.Invalid:
                return ValidateToken.Invalid;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return ValidateToken.Invalid;
    }

    public ValidateToken DemandClaim(string token, string claimType)
    {
        ValidateToken validate = ValidateCurrentToken(token);
        switch (validate)
        {
            case ValidateToken.Valid:
                {
                    ValidateToken claim = GetClaim(token, claimType, "true");
                    if (claim == ValidateToken.Valid)
                    {
                        return ValidateToken.Valid;
                    }
                }
                break;
            case ValidateToken.TimeExpired:
                return ValidateToken.TimeExpired;
            case ValidateToken.Invalid:
                return ValidateToken.Invalid;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return ValidateToken.Invalid;
    }

}
