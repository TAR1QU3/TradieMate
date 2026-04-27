using System.Net.Http.Json;
using System.Text.Json;
using TradieMateWeb.Client.Models;

namespace TradieMateWeb.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private const string TokenKey = "auth_token";
    private const string EmailKey = "auth_email";
    private const string BusinessKey = "auth_business";

    public string? Token { get; private set; }
    public string? Email { get; private set; }
    public string? BusinessName { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    public AuthService(HttpClient http)
    {
        _http = http;
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

    public void Logout()
    {
        Token = null;
        Email = null;
        BusinessName = null;
        _http.DefaultRequestHeaders.Authorization = null;
    }
}