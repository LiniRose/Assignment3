using ApiContracts;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("posts", request);
        return await HttpHelper.ReadAsync<PostDto>(httpResponse);
    }

    public async Task<List<PostDto>> GetPostsAsync()
    {
        HttpResponseMessage httpResponse = await client.GetAsync("posts");
        return await HttpHelper.ReadAsync<List<PostDto>>(httpResponse);
    }

    public async Task<PostDto> GetPostAsync(int id, bool includeComments = true)
    {
        string include = includeComments ? "true" : "false";
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{id}?includeComments={include}");
        return await HttpHelper.ReadAsync<PostDto>(httpResponse);
    }
}
