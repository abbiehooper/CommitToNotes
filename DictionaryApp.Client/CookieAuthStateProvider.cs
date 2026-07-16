using DictionaryApp.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace DictionaryApp.Client;

public class CookieAuthStateProvider(HttpClient http) : AuthenticationStateProvider
{
    private readonly HttpClient _http = http;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var res = await _http.GetAsync("auth/me");
            if (!res.IsSuccessStatusCode)
                return new AuthenticationState(_anonymous);

            var me = await res.Content.ReadFromJsonAsync<UserInfo>();
            if (me is null || string.IsNullOrEmpty(me.Email))
                return new AuthenticationState(_anonymous);

            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, me.Email)], "cookieauth");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}