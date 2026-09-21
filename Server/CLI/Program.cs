using CLI.UI;
using InMemoryRepositories;
using RepositoryContracts;

//creates the repos and CliApp
//program.cs initiates whatever needs to be created, probably primary repos + passes them to CliApp
Console.WriteLine("Starting CLI app...");
IUserRepository userRepository = new UserInMemoryRepository();
ICommentRepository commentRepository = new CommentInMemoryRepository();
IPostRepository postRepository = new PostInMemoryRepository();

CliApp cliApp = new CliApp(userRepository, commentRepository, postRepository);
await cliApp.StartAsync();