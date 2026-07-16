namespace DictionaryApp.Shared;

public record DictionaryEntryDto(int Id, string Key, string Value);
public record CreateEntryRequest(string Key, string Value);
public record UpdateEntryRequest(string Key, string Value);

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record UserInfo(string Email);
