using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    //Post/comment
    public async Task<CommentDto> CreateCommentAsync(CreateCommentDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("comments",request); 
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<CommentDto>(response, JsonOptions)!;
    }
    
    //Put/comment/{id}
    //API answers 204, nothing to deserialize
    public async Task UpdateCommentAsync(int id, UpdateCommentDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"comments/{id}", request);
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }
    
    //Delete/comment/{id}
    public async Task DeleteCommentAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"comments/{id}");
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }
    
    //Get/comment/{id}
    public async Task<CommentDto> GetSingleCommentAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"comments/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<CommentDto>(response, JsonOptions)!;
    }
    
    //Get/comment?UserName
    public async Task<IEnumerable<CommentDto>> GetManyCommentsAsync(int? postId = null, int? userId = null,
        string? userName = null)
    {
        //collect only the filters the caller actually provided
        List<string> queryParts = new();

        if (postId.HasValue)
        {
            queryParts.Add($"postId={postId.Value}");
        }

        if (userId.HasValue)
        {
            queryParts.Add($"userId={userId.Value}"); //numbers are URL safe
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            queryParts.Add($"userName={Uri.EscapeDataString(userName)}");
        }

        string url = "comments";
        if (queryParts.Count > 0)
        {
            url += "?" + string.Join("&", queryParts);
        }

        HttpResponseMessage httpResponse = await client.GetAsync(url);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<CommentDto>>(response, JsonOptions)!;
    }
}