using System.Net.NetworkInformation;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using MySql.Data.MySqlClient;
using Proty.db.models;

namespace Proty.db
{
    public class Db
    {
        public static Db Instance { get; private set; }
        private static string Ip { get; } = "37.59.156.138";
        private MySqlConnection _baseConnection;

        public async Task<DbGuild> FetchDbGuildAsync(DiscordGuild guild)
        {
            return await FetchDbGuildAsync(guild.Id);
        }
        
        public async Task<DbGuild> FetchDbGuildAsync(ulong id)
        {
            var cached = DbGuild.GuildCache.GetCached(id);

            if (cached != null) return cached;
            
            var con = (MySqlConnection) _baseConnection.Clone();
            await con.OpenAsync();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Guilds WHERE guildId=@Id;";
            cmd.Parameters.AddWithValue("Id", id);

            var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) return null;
            await reader.ReadAsync();
            return DbGuild.GuildCache.AddToCache(new DbGuild(reader));

        }

        public async Task UpdateDbGuild(DbGuild guild)
        {
            
            var con = (MySqlConnection) _baseConnection.Clone();
            await con.OpenAsync();
            var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE Guilds SET prefix=@Prefix, premium=@Premium WHERE guildId=@Id;";
            cmd.Parameters.AddWithValue("Id", guild.GuildId);
            cmd.Parameters.AddWithValue("Prefix", guild.Prefix);
            cmd.Parameters.AddWithValue("Premium", guild.Premium);
            
            await cmd.ExecuteNonQueryAsync();
            DbGuild.GuildCache.AddToCache(guild);
        }

        public async Task CreateDbGuild(DbGuild guild)
        {
            var con = (MySqlConnection) _baseConnection.Clone();
            await con.OpenAsync();
            var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO Guilds(guildId, prefix, premium) VALUES (@Id, @Prefix, @Premium);";
            cmd.Parameters.AddWithValue("Id", guild.GuildId);
            cmd.Parameters.AddWithValue("Prefix", guild.Prefix);
            cmd.Parameters.AddWithValue("Premium", guild.Premium);

            await cmd.ExecuteNonQueryAsync();
            DbGuild.GuildCache.AddToCache(guild);
        }

        public async Task<long> GetPingAsync()
        {
            return (await new Ping().SendPingAsync(Ip, 1000)).RoundtripTime;
        }

        public void Init()
        {
            _baseConnection = new MySqlConnection($"SERVER={Ip};DATABASE=proty;UID=proty;PASSWORD=fy!m53!5ciGEpc");
            Instance = this;
        }
    }
}