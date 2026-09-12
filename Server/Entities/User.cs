namespace Entities;

public class User
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public int Id { get; set; }
  

    public User(string userName, string password, int Id)
    {
        UserName = userName;
        Password = password;
        Id = Id;
    }
    //obj initializer syntax, default constructor
    public User()
    {}

    public override string ToString()
    {
        return $"User: {Id}: {UserName}";
    }
}