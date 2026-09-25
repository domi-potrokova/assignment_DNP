using CLI.UI;
using FileRepositories;
using RepositoryContracts;

//creates the repos and CliApp
//program.cs initiates whatever needs to be created, probably primary repos + passes them to CliApp
Console.WriteLine("Starting CLI app...");
IUserRepository userRepository = new UserFileRepository();
ICommentRepository commentRepository = new CommentFileRepository();
IPostRepository postRepository = new PostFileRepository(); //after finishing the FileRepository, we have to delete dependency to InMemoryRepo
//all the references must change from InMemoryRepo to FileRepo

CliApp cliApp = new CliApp(userRepository, commentRepository, postRepository);
await cliApp.StartAsync();