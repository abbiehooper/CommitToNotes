using CommitToNotes.Shared;
using System.Net.Http.Json;

namespace CommitToNotes.Client;

public class AuthService(HttpClient http, CookieAuthStateProvider state)
{
    private readonly HttpClient _http = http;
    private readonly CookieAuthStateProvider _state = state;

    public async Task<bool> RegisterAsync(string email, string password)
    {
        var res = await _http.PostAsJsonAsync(
            "auth/register", new RegisterRequest(email, password));
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var res = await _http.PostAsJsonAsync(
            "auth/login", new LoginRequest(email, password));
        if (res.IsSuccessStatusCode) _state.NotifyStateChanged();
        return res.IsSuccessStatusCode;
    }

    public async Task LogoutAsync()
    {
        await _http.PostAsync("auth/logout", null);
        _state.NotifyStateChanged();
    }
}