namespace ApiContracts;

//DTOs are reshaped version of entity: for security reasons (entity user has password, dto wont use it)
//secondly for different shapes and operations: different variables for each operation (create needs some variables and update needs smth elese)
public class UserDto
{
    //not using the password as in entity
    public int Id { get; set; }
    public string? UserName { get; set; }
}