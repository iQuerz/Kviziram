using StackExchange.Redis;

public interface IGuestService
{
    public Task addString(string key, string value);
    public Task<string?> getString(string key);
}