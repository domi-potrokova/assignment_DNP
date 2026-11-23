using Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcRepositories;

public class AppContext : DbContext
{
    //DbSet is essentially a representation of a table in database
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();

    //we specify to use Sqlite, and the db file is called "app.db"
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // absolute path = the Web API, CLI and migrations all use the same file.
        // this is the app.db that sits inside the EfcRepositories project folder
        optionsBuilder.UseSqlite(@"Data Source=C:\Users\domin\RiderProjects\DNP_assignmnet\Server\EfcRepositories\app.db");
    }
}