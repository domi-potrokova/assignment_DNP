using System.Net;
using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }
    
    //Post/post
    public async Task<PostDto> CreatePostAsync(CreatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("posts",request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<PostDto>(response, JsonOptions)!;
    }
    
    //Put/post/{id}
    //API answers 204, nothing to deserialize
    public async Task UpdatePostAsync(int id, UpdatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"posts/{id}", request);
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }
    
    //Delete/post/{id}
    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"posts/{id}");
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }
    
    //Get/post/{id}
    public async Task<PostDto> GetSinglePostAsync(int id, bool includeComments = false)
    {
        string url = $"posts/{id}";
        if (includeComments) //we add the bool to the url, this parameter is sent only when its true (default is false)
        {
            url += "?includeComments=true"; //only one parameter, so its always the first one
        }
        
        HttpResponseMessage httpResponse = await client.GetAsync(url);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<PostDto>(response, JsonOptions)!;
    }
    
    //Get/post?UserName
    public async Task<IEnumerable<PostDto>> GetManyPostsAsync(string? title = null, int? userId = null,
        string? userName = null)
    {
        //collect only the filters the caller actually provided
        List<string> queryParts = new();

        if (!string.IsNullOrWhiteSpace(title))
        {
            queryParts.Add($"title={Uri.EscapeDataString(title)}");
        }

        if (userId.HasValue)
        {
            queryParts.Add($"userId={userId.Value}"); //numbers are URL safe
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            queryParts.Add($"userName={Uri.EscapeDataString(userName)}");
        }

        string url = "posts";
        if (queryParts.Count > 0)
        {
            //Join puts & between the parts = title=...&userId=...
            url += "?" + string.Join("&", queryParts);
        }

        HttpResponseMessage httpResponse = await client.GetAsync(url);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<PostDto>>(response, JsonOptions)!;
    }
}