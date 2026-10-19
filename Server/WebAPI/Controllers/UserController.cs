using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

//job of controller is to coordinate, no business logic sits here
//more like validation rules live here
//controllers job: translate between HTTP world (requests, routes, status, JSON) and my app's world (entities, repos)

//5 endpoints for the User entity (Create, Update, GetSingle, GetMany, Delete) = 3 entities with 5 endpoints
[ApiController]
[Route("users")] //base route; all actions here start with /users
public class UserController : ControllerBase
{
    //can't use DTOs bc they are just temporary obj that exist for the duration of one request, don't store anything
    //repo is where the actual data lives
    //controllers job: take CreateUserDto -> build actual User entity from it -> hand that to userRepo.AddAsync to save it
    //than get back to saved User -> map it to UserDto to send back
    private readonly IUserRepository userRepository;
    
    //ASP.NET's dependency injection creates this and injects the repo automatically
    //the same idea as CliApp's constructor, just wired by framework
    public UserController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    
    //POST/users
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        //business rule: user name must be unique
        //bool taken = does at least one element in this sequence satisfy the condition i gave? true/false
        bool taken = userRepository.GetMany() 
            //.Any = calls the method on the thing to left, so userRepo.GetMany() -> .any is called on whatever is returned from that bool call
            //the lambda is condition, it knows that u means User bc .any is being called on IQueryable<User>, so it knows the lambda param must be type of User
            //Does any existing user have the same username (ignoring the case) as the one being created?
            .Any(u => u.UserName.Equals(dto.UserName, StringComparison.OrdinalIgnoreCase));
        if (taken)
        {
            return Conflict($"Username '{dto.UserName}' is already taken."); //error 409
        }
        
        //map DTO = entity id is 0 here -> AddAsync assigns the real one here
        User user = new User(dto.UserName, dto.Password, 0);
        User created = await userRepository.AddAsync(user);

        UserDto resultDto = new UserDto()
        {
            Id = created.Id,
            UserName = created.UserName
        };
        //201 created with a location header pointing to getSingle for this new user
        return CreatedAtAction(nameof(GetSingle), new { id = resultDto.Id }, resultDto);
    }
    
    //PUT/users/{id}
    [HttpPut("{id}")]
    //async = pause this method until the repo finishes its work and than continue
    //two inputs from two sources = url: id, dto: json request body
    public async Task<ActionResult> Update(int id, UpdateUserDto dto)
    {
        //map DTO -> entity, using the id from the route not from the body
        User user = new User(dto.UserName, dto.Password, id); //combines route id, with body's username/password into one complete entity
        try
        {
            //finds the existing user by id, removes it and updates the version, rewrites json file
            await userRepository.UpdateAsync(user); 
        }
        //user with that id doesnt exist - nothing to update, need to throw exception otherwise server would crash
        catch (InvalidOperationException)
        {
            return NotFound($"User with id {id} not found.");
        }

        return NoContent(); //204 = success, nothing to return
    }
    
    //GET/users/{id}
    //receive Id from URL -> ask repo for that user -> if missing say 404 -> otherwise repackage and send back safe DTO as JSON
    //we are just reading instead of writing = only Id comes from the URL
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetSingle(int id)
    {
        User user;
        try
        {
            //same as update, ask repo for the user, if it doesn't exist throw exception so the server doesnt crash
            user = await userRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with id {id} not found");
        }

        //we only copy the id and userName into UserDto, leaving password out (thats why we have dto), so the client never sees the password
        UserDto dto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName
        };
        return Ok(dto); //response will be 200 ok with dto serialized to json automatically
    }
    
    //GET/users?userName=dom
    //return list of users, either everyone or a filtered subset if the caller asked for one
    [HttpGet] //no id here, different from GetSingle, matches plain GET/users (ASP.NET tells them apart by the route pattern
    //IEnumerable = sequence of UserDto obj i can loop over
    //reads optional ?userName part from the request URL and puts the value into userName variable for me to use, if client didnt include it, the userName is just null
    public ActionResult<IEnumerable<UserDto>> GetMany([FromQuery] string? userName)
    {
        //calls existing repo method and stores the result in a local variable
        //local variable: variable declared inside a method, that only exists while that method is running + disappears once the method finishes (local = nothing outside can see or use it)
        //users (local) created when GetMany() starts runing, changed in if block
        IQueryable<User> users = userRepository.GetMany();
        
        //runs only if client sent userName value
        //optional filter: "users with some specific word in their username" (from my org requirement)
        if (!string.IsNullOrWhiteSpace(userName))
        {
            users = users.Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase));
            //u lambda knows its user bc it comes from IQueryable<User>
        }
        
        //each matched user converts to UserDto and .ToList collects it into List<UserDto> and sends back to client
        //select strips out passwords and reshapes every user into a DTO and sends the list back to client
        List<UserDto> dtos = users
            .Select(u => new UserDto {Id = u.Id, UserName = u.UserName})
        .ToList();

        return Ok(dtos); //200 ok, list serialized as JSON array
    }
    
    //DELETE/users/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await userRepository.DeleteAsync(id);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with id {id} not found");
        }

        return NoContent(); //204
    }
}