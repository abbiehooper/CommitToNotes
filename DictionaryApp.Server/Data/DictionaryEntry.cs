namespace DictionaryApp.Server.Data;

public class DictionaryEntry
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}
