using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ProxyCache
{
    internal class GenericProxyCache
    {
        private ObjectCache cache = MemoryCache.Default;
        public T Get<T>(string CacheItemName) where T : class
        {
            T fileContents = cache[CacheItemName] as T;

            if (fileContents == null)
            {             
                T objet = (T)Activator.CreateInstance(typeof(T));
                cache.Set(CacheItemName, objet, new CacheItemPolicy());
                Console.WriteLine(objet + " a été enregistré dans le cache");
                return objet;
            }

            Console.WriteLine(fileContents + " a été récupéré depuis le cache");

            return fileContents;
        }

        public T Get<T>(string CacheItemName, double dt_seconds) where T : class
        {
            T fileContents = cache[CacheItemName] as T;

            if (fileContents == null)
            {
                T objet = (T)Activator.CreateInstance(typeof(T));
                CacheItemPolicy policy = new CacheItemPolicy();
                cache.Set(CacheItemName, objet, DateTimeOffset.Now.AddSeconds(dt_seconds));
                return objet;
            }

            return fileContents;
        }

        public T Get<T>(string CacheItemName, DateTimeOffset dt) where T : class
        {
            T fileContents = cache[CacheItemName] as T;

            if (fileContents == null)
            {
                T objet = (T)Activator.CreateInstance(typeof(T));
                CacheItemPolicy policy = new CacheItemPolicy();
                cache.Set(CacheItemName, objet, dt);
                return objet;
            }

            return fileContents;
        }
    }
}
