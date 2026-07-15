using Blog.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Blog.Infrastructure.Service
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<T?> GetAsync<T>(string key)
         {
            try
            {
                var cachedData = await _distributedCache.GetStringAsync(key);
                if (string.IsNullOrEmpty(cachedData))
                {
                    return default;
                }
                return JsonSerializer.Deserialize<T>(cachedData);
            }
            catch (Exception ex)
            {
                // Ghi log cảnh báo lỗi Redis ở đây nhưng không ném ra ngoại lệ để tránh sập ứng dụng
                Console.WriteLine($"[Redis Error] GetAsync failed: {ex.Message}");
                return default;
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis Error] RemoveAsync failed: {ex.Message}");
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
              try
            {
                var options = new DistributedCacheEntryOptions();
                options.AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5);
                var valueToJson = JsonSerializer.Serialize(value);
                await _distributedCache.SetStringAsync(key, valueToJson, options);
            }
            catch (Exception ex)
            {
                // Ghi log cảnh báo lỗi Redis ở đây
                Console.WriteLine($"[Redis Error] SetAsync failed: {ex.Message}");
                // Không ném ra ngoại lệ để không ảnh hưởng đến nghiệp vụ
            }
        }
    }
}
