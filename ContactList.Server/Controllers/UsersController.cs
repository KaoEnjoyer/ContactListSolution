using ContactList.Shared.Dto;
using ContactList.Shared.Models;
using ContactList.Users;
using ContactList.Users.Requests;
using ContactList.Exceptions;
using ContactList.Users.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Exceptions;
using ValidationException = ContactList.Exceptions.ValidationException;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersDatabase usersDatabase;
    private readonly ITokens tokens;

    public UsersController(IUsersDatabase usersDatabase, ITokens tokens)
    {
        this.usersDatabase = usersDatabase;
        this.tokens = tokens;
    }

    [HttpPost("search")]
    public ActionResult Search([FromHeader] string token, [FromBody] SearchUsersRequest searchUsers)
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

        List<User> users = usersDatabase.SelectAll(searchUsers);
        int count = usersDatabase.SelectCount(searchUsers);
        SearchUsersResponse result = new()
        {
            Users = users,
            CurrentUser = searchUsers.From,
            All = count
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult Get( [FromHeader] string token)
    {
        string id;
        id = "21";
        ValidateToken tokenCkeck = tokens.DemandClaim(token, "userAdmin");
        ValidateToken idCheck = tokens.GetClaim(token, "id", id.ToString());

        switch (tokenCkeck)
        {
            case ValidateToken.Valid:
                break;
            case ValidateToken.TimeExpired:
                if (idCheck != ValidateToken.Valid)
                {
                    return Conflict("TimeExpired");
                }
                break;
            case ValidateToken.Invalid:
                if (idCheck != ValidateToken.Valid)
                {
                    return Unauthorized();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        User user = usersDatabase.Select(id.ToString());//do zmieny 
        if (user == null)
        {
            return NotFound(id);
        }

        user.Password = "";

        return Ok(user);
    }

    //[HttpPost("add")]
    //public ActionResult Add([FromHeader] string token, [FromBody] User user)
    //{
    //    ValidateToken validate = tokens.DemandClaim(token, "userAdmin");
    //    switch (validate)
    //    {
    //        case ValidateToken.Valid:
    //            break;
    //        case ValidateToken.TimeExpired:
    //            return Conflict("TimeExpired");
    //        case ValidateToken.Invalid:
    //            return Unauthorized();
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }

    //    User newUser;
    //    try
    //    {
    //        newUser = usersDatabase.Add(user);

    //    }
    //    catch (ValidationException e)
    //    {
    //        return Conflict(e.Message);
    //    }
    //    catch (SqlException)
    //    {
    //        return Conflict("LoginAlreadyExist");
    //    }

    //    return Created($"/api/books/{newUser.Id}", newUser);
    //}
    //[FromHeader] string token,
    [HttpPost("Register")]
    public ActionResult Register( [FromBody]RegisterRequest registerRequest)
    {
        User newUser;
        try
        {

            newUser = usersDatabase.Add(registerRequest);

           
        }
        catch (ValidationException e)
        {
            return Conflict(e.Message);
        }
        catch (SqlException)
        {
            return Conflict("LoginAlreadyExist");
        }

        return Created($"/api/books/{newUser.Id}", newUser);
    }

    [HttpPut("{id}")]
    public ActionResult Edit(int id, [FromHeader] string token, [FromBody] User user)
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
            newUser = usersDatabase.Edit(id, user);
        }
        catch (ValidationException e)
        {
            return Conflict(e.Message);
        }
        catch (NotFoundException)
        {
            return NotFound(id);
        }

        return Ok(newUser);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id, [FromHeader] string token)
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
            usersDatabase.Delete(id);
        }
        catch (NotFoundException)
        {
            return NotFound(id);
        }
        return NoContent();
    }

    [HttpPost("/login")]
    public ActionResult Login([FromBody] LoginRequest loginRequest)
    {
        User user = usersDatabase.Login(loginRequest);
        if (user == null)
        {
            return Conflict("loginRequest");
        }

        user.Token = tokens.GenerateToken(user);

        user.Password = "";

        return Ok(user);
    }

    [HttpPost("/logout")]
    public ActionResult Logout()
    {
        return Ok();
    }
}


