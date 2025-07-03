using ContactList.Shared.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContactList.Users;

/// Implementation of ITokens interface for JWT token generation and validation
public class Tokens : ITokens
{
    // Secret key used for signing the tokens (Note: In production, this should be stored securely)
    private const string MySecret = "aadawdawdadkadkajdhakdjadadjawdjawdaa";
    // Issuer of the tokens
    private const string MyIssuer = "listakontaktow";
    // Intended audience of the tokens
    private const string MyAudience = "listakontaktow";

    /// Generates a JWT token for the specified user
    /// <param name="user">User object containing user details</param>
    /// <returns>Generated JWT token as a string</returns>
    public string GenerateToken(User user)
    {
        // Create a symmetric security key using the secret
        SymmetricSecurityKey mySecurityKey = new(Encoding.ASCII.GetBytes(MySecret));
        // Create a token handler for JWT tokens
        JwtSecurityTokenHandler tokenHandler = new();
        
        // Configure the token descriptor with claims, expiration, and signing credentials
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(new[]
            {
                // Add user ID as a claim
                new Claim("id", user.Id.ToString())
            }),
            // Token expires in 7 days
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = MyIssuer,
            Audience = MyAudience,
            // Sign the token using HMAC SHA256 algorithm
            SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        // Create and write the token
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// Validates the provided JWT token
    /// <param name="token">JWT token to validate</param>
    /// <returns>Validation result as ValidateToken enum</returns>
    public ValidateToken ValidateCurrentToken(string token)
    {
        SymmetricSecurityKey mySecurityKey = new(Encoding.ASCII.GetBytes(MySecret));
        JwtSecurityTokenHandler tokenHandler = new();
        try
        {
            // Validate the token against the specified parameters
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
            // Token is valid but expired
            return ValidateToken.TimeExpired;
        }
        catch
        {
            // Token is invalid for any other reason
            return ValidateToken.Invalid;
        }
        // Token is valid
        return ValidateToken.Valid;
    }

    /// Checks if the token contains a specific claim with a specific value
    /// <param name="token">JWT token to check</param>
    /// <param name="claimType">Type of claim to look for</param>
    /// <param name="claimValue">Expected value of the claim</param>
    /// <returns>Validation result as ValidateToken enum</returns>
    public ValidateToken GetClaim(string token, string claimType, string claimValue)
    {
        // First validate the token itself
        ValidateToken validate = ValidateCurrentToken(token);
        switch (validate)
        {
            case ValidateToken.Valid:
                {
                    // Token is valid, proceed to check claims
                    JwtSecurityTokenHandler tokenHandler = new();
                    JwtSecurityToken securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                    if (securityToken != null)
                    {
                        Claim first = null;
                        // Search for the specified claim type
                        foreach (Claim claim in securityToken.Claims)
                        {
                            if (claim.Type == claimType)
                            {
                                first = claim;
                                break;
                            }
                        }

                        // If claim found, check its value
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
                // Token is expired
                return ValidateToken.TimeExpired;
            case ValidateToken.Invalid:
                // Token is invalid
                return ValidateToken.Invalid;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Claim not found or value doesn't match
        return ValidateToken.Invalid;
    }

    /// Checks if the token contains a specific claim (regardless of value)
    /// <param name="token">JWT token to check</param>
    /// <param name="claimType">Type of claim to look for</param>
    /// <returns>Validation result as ValidateToken enum</returns>
    public ValidateToken DemandClaim(string token, string claimType)
    {
        // First validate the token itself
        ValidateToken validate = ValidateCurrentToken(token);
        switch (validate)
        {
            case ValidateToken.Valid:
                {
                    // Check if the claim exists with value "true"
                    ValidateToken claim = GetClaim(token, claimType, "true");
                    if (claim == ValidateToken.Valid)
                    {
                        return ValidateToken.Valid;
                    }
                }
                break;
            case ValidateToken.TimeExpired:
                // Token is expired
                return ValidateToken.TimeExpired;
            case ValidateToken.Invalid:
                // Token is invalid
                return ValidateToken.Invalid;
            default:
                throw new ArgumentOutOfRangeException();
        }
        // Claim not found
        return ValidateToken.Invalid;
    }
}