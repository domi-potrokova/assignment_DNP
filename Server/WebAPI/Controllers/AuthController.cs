using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("auth")] //base route: /auth
public class AuthController : ControllerBase
{
    //same DI pattern as the other controllers = container injects the repo
    private readonly IUserRepository userRepository;

    public AuthController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    
    //POST /auth/login
    [HttpPost("login")]
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        //the lowercase value is computed first outside the query
        string loginName = request.UserName.ToLower();
        
        //1. find the user by username(case-sensitive rule as the uniqueness check)
        User? user = userRepository.GetMany().FirstOrDefault(u =>
            u.UserName.ToLower() == loginName);
        
        // TEMPORARY debug line: remove it afterwards, it prints a password!
// the quotes make spaces at the start or end visible
        Console.WriteLine($"DEBUG stored: '{user?.Password}' | received: '{request.Password}'");
        
        //2. user does not exist -> 401
        //3. wrong password -> 401
        //same message for both on purpose, it doesn't tell an attacker which part was wrong
        if (user is null || user.Password != request.Password)
        {
            return Unauthorized("Username or password is incorrect");
        }
        //4. convert user -> UserDto, so the password is never sent back
        UserDto dto = new UserDto()
        {
            Id = user.Id,
            UserName = user.UserName
        };
        //5. return the DTO -> 200 OK
        return Ok(dto);
    }
}