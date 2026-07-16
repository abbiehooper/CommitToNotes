namespace DictionaryApp.Shared;

public record TeamDto(int Id, string Name);
public record DictionaryEntryDto(int Id, int TeamId, string Key, string Value);
public record CreateTeamRequest(string Name);
public record CreateEntryRequest(int TeamId, string Key, string Value);
public record UpdateEntryRequest(string Key, string Value);

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record UserInfo(string Email);
