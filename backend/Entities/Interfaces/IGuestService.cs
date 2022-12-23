using StackExchange.Redis;

public interface IGuestService
{
    public Task AddString(string key, string value);
    public Task<string?> GetString(string key);
}