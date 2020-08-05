using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace ngApp_netCoreApi_kestrel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _redisDatabase;

        public RedisController(IDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        [HttpGet("{key}")]
        [Produces(typeof(string))]
        public async Task<IActionResult> GetAsync(string key)
        {
            if (!_redisDatabase.KeyExists(key))
                return NotFound();

            var value = await _redisDatabase.StringGetAsync(key);

            return Ok(value.ToString());
        }

        [HttpPost]
        public async Task PostAsync([FromBody] KeyValuePair<string, string> keyValue)
        {
            await _redisDatabase.StringSetAsync(keyValue.Key, keyValue.Value);
        }

        [HttpPost(template: "expiry")]
        public async Task SaveExpiry([FromBody] KeyValuePair<string, string> keyValue)
        {
            await _redisDatabase.StringSetAsync(keyValue.Key, keyValue.Value, expiry: TimeSpan.FromSeconds(10));
        }
    }
}
