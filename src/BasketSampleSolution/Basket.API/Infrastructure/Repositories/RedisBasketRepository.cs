using Basket.API.Model;
using Basket.API.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure.Repositories
{
    public class RedisBasketRepository : IBasketRepository
    {
        private readonly ILogger<RedisBasketRepository> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisBasketRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<RedisBasketRepository>();
            _redis = redis;
            _database = redis.GetDatabase();
        }

        public Task<bool> DeleteBasketAsync(string customerId) =>
            _database.KeyDeleteAsync(customerId);

        public async Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            var cachedBasket = await _database.StringGetAsync(customerId);

            if (cachedBasket.IsNullOrEmpty)
                return null;

            return JsonConvert.DeserializeObject<CustomerBasket>(cachedBasket);
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var keysInServer = server.Keys();

            return keysInServer?.Select(k => k.ToString());
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var created = await _database.StringSetAsync(basket.BuyerId, JsonConvert.SerializeObject(basket));

            if (!created)
            {
                _logger.LogInformation("Problem occur while persisting the basket in cache");
                return null;
            }

            return await GetBasketAsync(basket.BuyerId);
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }
    }
}
