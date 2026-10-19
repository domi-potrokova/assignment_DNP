namespace ApiContracts;

//what the client sends to update the user, similar to create, but kept separately so the callers are not broken
public class UpdateUserDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
}