namespace ApiContracts;

//what the client sends to create a user
//no Id bc server assigns it
public class CreateUserDto
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}