using System;
using System.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Proty.db.models
{
    public class DbGuild
    {
        public ulong GuildId { get; set; }
        public string Prefix { get; set; } = "!";
        public bool Premium { get; set; } = false;
        
        public DbGuild(IDataRecord record)
        {
            GuildId = (ulong) record.GetInt64(0);
            Prefix = record.GetString(1);
            Premium = record.GetBoolean(2);
        }
        
        public DbGuild() {}
        
        public static class GuildCache
        {
            private static readonly MemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());
            
            public static DbGuild GetCached(ulong id)
            {
                MemoryCache.TryGetValue(id, out DbGuild guild);
                return guild;
            }

            public static DbGuild AddToCache(DbGuild guild)
            {
                RemoveCached(guild.GuildId);
                
                return (DbGuild) MemoryCache.CreateEntry(guild.GuildId)
                    .SetValue(guild)
                    .SetAbsoluteExpiration(TimeSpan.FromHours(2))
                    .SetSlidingExpiration(TimeSpan.FromHours(1)).Value;
            }

            public static void RemoveCached(ulong id)
            {
                MemoryCache.Remove(id);
            }
        }
    }
}