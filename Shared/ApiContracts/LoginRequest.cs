namespace ApiContracts;

//what the client sends to log in, same style as my create DTOs
public class LoginRequest
{
    public required string UserName { get; set;}
    public required string Password { get; set; }
}