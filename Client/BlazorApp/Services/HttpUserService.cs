using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;
    
    //shared option: API sends camelCase JSON, my DTOs use PascalCase
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    //Post/Users
    public async Task<UserDto> CreateUserAsync(CreateUserDto request)
    {
        //PostAsJsonAsync = simplifies the process, otherwise we would have to manually serialize the CreateUserDto request param
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("users", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        //the ! here means = silences the warning that possible null reference returns
        // ! = null forgiving operator
        return JsonSerializer.Deserialize<UserDto>(response, JsonOptions)!;
    }

    //Put/Users/{id} = the API answers 204, no body to deserialize
    public async Task UpdateUserAsync(int id, UpdateUserDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"users/{id}", request);
        if (!httpResponse.IsSuccessStatusCode)
        {
            //on errors the controller sends a plain text message (404)
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }
    
    //Delete/users/{id} = also 204 no content
    public async Task DeleteUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"users/{id}");
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }
    
    //Get/user/{id}
    public async Task<UserDto> GetSingleUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<UserDto>(response, JsonOptions)!;
    }
    
   //Get/users?userName=dom = the query string is only added when a filter is given 
   public async Task<IEnumerable<UserDto>> GetManyUsersAsync(string? userName = null)
   {
       string url = "users"; //plain route
       if (!string.IsNullOrWhiteSpace(userName)) //can be also null, whole list is returned
       {
           //EscapeDataString makes spaces and special characters safe inside URL (like & or æ)
           url += $"?userName={Uri.EscapeDataString(userName)}"; //if user inputs concrete userName than the list is filtered
       }

       HttpResponseMessage httpResponse = await client.GetAsync(url);
       string response = await httpResponse.Content.ReadAsStringAsync();
       if (!httpResponse.IsSuccessStatusCode)
       {
           throw new Exception(response);
       }
       return JsonSerializer.Deserialize<List<UserDto>>(response, JsonOptions)!;
   }
}