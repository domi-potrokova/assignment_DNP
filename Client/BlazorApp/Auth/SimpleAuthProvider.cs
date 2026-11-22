using System.Security.Claims;
using System.Text.Json;
using ApiContracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorApp.Auth;

//class is responsible for providing the claims representing user, whenever the Blazor framework needs to authorize anything
public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient; //used to contact the web api
    private ClaimsPrincipal? currentClaimsPrincipal; //stores current auth state

    //httpClient is injected through constructor
    public SimpleAuthProvider(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    // 1.Call web APi with LoginReq + its relevant info
    // 2.Read the content of the response (error/list of claims)
    // 3.If the request was unsuccessful throw exception, otherwise deserialize into UserDto
    // 4.UserDto is converted to a list of claims and packaged into ClaimsIdentity & the identity is packaged into ClaimsPrincipal, which is saved
    // in the field variable
    // 5.finally we notify the app about change in auth state, blazor will update and change accordingly
    public async Task Login(string userName, string password)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "auth/login",
            new LoginRequest{UserName = userName,Password = password});

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userDto.UserName),
            new Claim("Id", userDto.Id.ToString())
        };
        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        currentClaimsPrincipal = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentClaimsPrincipal)));
    }
    
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        //the ?? is null coalescing operator, checks if the current Claims Principal is null, and if so, than return the part after the ??
        return Task.FromResult(new AuthenticationState(currentClaimsPrincipal ?? new ()));
    }
    
    //Logout = reset ClaimsPrincipal to empty one and notify framework about change in auth state
    
    public void Logout()
    {
        currentClaimsPrincipal = new();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(currentClaimsPrincipal)));
    }
}