using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

public class KviziramService
{
    private IDatabase _redis;    

    public KviziramService(KviziramContext _context) {
        _redis = _context.Redis;
    }

    public async Task addString(string key, string value) {
        await _redis.StringSetAsync(key, value);
    }

    public async Task<string?> getString(string key) {
        return await _redis.StringGetAsync(key);
    }
    
}