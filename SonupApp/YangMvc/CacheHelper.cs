using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace YangMvc
{
    public class CacheHelper
    {
        private HttpContext Context;
        private IDistributedCache Cache;

        public CacheHelper(HttpContext httpContext)
        {
            this.Context = httpContext;
            this.Cache = httpContext.RequestServices.GetService(typeof(IDistributedCache)) as IDistributedCache;
        }


        public void Set<T>(int userId, string key, T obj, int minutes = 30)
        {
            key += $"_UID{userId}";
            Cache.Set(key, obj.ToBytes(), new DistributedCacheEntryOptions { AbsoluteExpiration=DateTime.Now.AddMinutes(minutes) });
        }

        public T Get<T>(int userId, string key)
        {
            key += $"_UID{userId}";
            return (T)(Cache.Get(key).ToObject());
        }

        public void Clear(int userId, string key)
        {
            key += $"_UID{userId}";
            Cache.Remove(key);
        }
    }
}
