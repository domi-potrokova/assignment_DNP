using ApiContracts;

namespace BlazorApp.Services;

public interface IUserService
{
    public Task<UserDto> CreateUserAsync(CreateUserDto request);
    public Task UpdateUserAsync(int id, UpdateUserDto request);
    public Task DeleteUserAsync(int id);
    public Task<UserDto> GetSingleUserAsync(int id);
    //with IQueryable<UserDto> its synchronous, an HTTP call is async so we need to change it to async method, bc IQueryable can't be called directly
    public Task<IEnumerable<UserDto>> GetManyUsersAsync( string? userName = null);
}