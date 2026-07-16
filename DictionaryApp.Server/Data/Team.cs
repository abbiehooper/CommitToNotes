namespace DictionaryApp.Server.Data;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<DictionaryEntry> Entries { get; set; } = new();
}

public class DictionaryEntry
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}
