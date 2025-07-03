using System.Text.Json;
using ContactList.Shared.Models;
using ContactList.Users;
using ContactList.Users.Requests;
using ContactList.Users.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Exceptions;
using ValidationException = ContactList.Exceptions.ValidationException;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersDatabase usersDatabase; // Database service for user data
    private readonly ITokens tokens; // Token management service

    // Constructor to inject dependencies
    public UsersController(IUsersDatabase usersDatabase, ITokens tokens)
    {
        this.usersDatabase = usersDatabase;
        this.tokens = tokens;
    }

    // Endpoint to fetch a user by ID
    [HttpGet("{id}")]
    public ActionResult<User> Get(string id)
    {
        User user = usersDatabase.Select(id); // Query the database
        if (user == null)
        {
            // Return 404 if the user is not found
            return NotFound(id);
        }
        // Return the user as a serialized JSON object
        return Ok(JsonSerializer.Serialize(user));
    }

    // Search functionality for users with token-based access validation
    [HttpPost("search")]
    public ActionResult Search([FromHeader] string token, [FromBody] SearchUsersRequest searchUsers)
    {
        // Validate the token to ensure the user is an admin
        ValidateToken validate = tokens.DemandClaim(token, "userAdmin");
        switch (validate)
        {
            case ValidateToken.Valid:
                break;
            case ValidateToken.TimeExpired:
                return Conflict("TimeExpired");
            case ValidateToken.Invalid:
                return Unauthorized();
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Get the list of users and their total count matching the request
        List<User> users = usersDatabase.SelectAll(searchUsers);
        int count = usersDatabase.SelectCount(searchUsers);
        SearchUsersResponse result = new()
        {
            Users = users,
            CurrentUser = searchUsers.From,
            All = count
        };
        return Ok(result); // Return the search results
    }

    // Register a new user
    [HttpPost("Register")]
    public ActionResult Register([FromBody] RegisterRequest registerRequest)
    {
        User newUser;
        try
        {
            // Add the new user to the database
            newUser = usersDatabase.Add(registerRequest);
        }
        catch (ValidationException e)
        {
            // Return conflict if validation fails
            return Conflict(e.Message);
        }
        catch (SqlException)
        {
            // Handle case when login already exists
            return Conflict("LoginAlreadyExist");
        }

        return Created($"/api{newUser.Id}", newUser); // Return the created user
    }

    // Edit user details by ID
    [HttpPut("{id}")]
    public ActionResult Edit(string id, [FromHeader] string token, [FromBody] User user)
    {
        ValidateToken validate = tokens.DemandClaim(token, "userAdmin");
        switch (validate)
        {
            case ValidateToken.Valid:
                break;
            case ValidateToken.TimeExpired:
                return Conflict("TimeExpired");
            case ValidateToken.Invalid:
                return Unauthorized();
            default:
                throw new ArgumentOutOfRangeException();
        }

        User newUser;
        try
        {
            // Update the user in the database
        }
        catch (ValidationException e)
        {
            return Conflict(e.Message); // Validation failed
        }
        catch (NotFoundException)
        {
            return NotFound(id); // User not found
        }

        return Ok(); // Return the updated user
    }

    // Delete a user by ID
    [HttpDelete("{id}")]
    public ActionResult Delete(string id, [FromHeader] string token)
    {
        ValidateToken validate = tokens.DemandClaim(token, "userAdmin");
        switch (validate)
        {
            case ValidateToken.Valid:
                break;
            case ValidateToken.TimeExpired:
                return Conflict("TimeExpired");
            case ValidateToken.Invalid:
                return Unauthorized();
            default:
                throw new ArgumentOutOfRangeException();
        }

        try
        {
            // Delete the user from the database
        }
        catch (NotFoundException)
        {
            return NotFound(id); // User not found
        }
        return NoContent(); // Return no content after deletion
    }

    // Handle user login
    [HttpPost("/login")]
    public ActionResult Login([FromBody] LoginRequest loginRequest)
    {
        User user = usersDatabase.Login(loginRequest); // Authenticate the user
        if (user == null)
        {
            // Login failed
            return Conflict("loginRequest");
        }

        user.Token = tokens.GenerateToken(user); // Generate token for the user
        user.Password = ""; // Clear the password for security

        return Ok(user); // Return authenticated user details
    }

    // Handle user logout
    [HttpPost("/logout")]
    public ActionResult Logout()
    {
        return Ok(); // Return success for logout
    }
}