using System.Net.Http.Json;
using System.Text.Json;
using TradieMateWeb.Client.Models;
using Microsoft.JSInterop;

namespace TradieMateWeb.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public string? Token { get; private set; }
    public string? Email { get; private set; }
    public string? BusinessName { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var token = await _js.InvokeAsync<string>("sessionStorage.getItem", "auth_token");
            var email = await _js.InvokeAsync<string>("sessionStorage.getItem", "auth_email");
            var business = await _js.InvokeAsync<string>("sessionStorage.getItem", "auth_business");

            if (!string.IsNullOrEmpty(token))
            {
                Token = token;
                Email = email;
                BusinessName = business;
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }
        }
        catch { }
    }

    public async Task<bool> Login(LoginDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("Auth/login", dto);
            if (!response.IsSuccessStatusCode) return false;

            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (auth == null) return false;

            Token = auth.Token;
            Email = auth.Email;
            BusinessName = auth.BusinessName;

            await _js.InvokeVoidAsync("sessionStorage.setItem", "auth_token", Token);
            await _js.InvokeVoidAsync("sessionStorage.setItem", "auth_email", Email);
            await _js.InvokeVoidAsync("sessionStorage.setItem", "auth_business", BusinessName);

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

            return true;
        }
        catch { return false; }
    }

    public async Task<bool> Register(RegisterDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("Auth/register", dto);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task Logout()
    {
        Token = null;
        Email = null;
        BusinessName = null;
        _http.DefaultRequestHeaders.Authorization = null;
        await _js.InvokeVoidAsync("sessionStorage.removeItem", "auth_token");
        await _js.InvokeVoidAsync("sessionStorage.removeItem", "auth_email");
        await _js.InvokeVoidAsync("sessionStorage.removeItem", "auth_business");
    }
}