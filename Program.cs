using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proty.commands;
using Proty.db;
using Serilog;

namespace Proty
{
    class Program
    {
        private static Db Db { get; } = new Db();
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory().AddSerilog();
        public static IServiceProvider Services = new ServiceCollection().AddSingleton(Db).BuildServiceProvider();
        
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            Init();
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = args[0],
                TokenType = TokenType.Bot,
                LoggerFactory = LoggerFactory
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                UseDefaultCommandHandler = false,
                Services = Services
            });

            commands.RegisterCommands(Assembly.GetExecutingAssembly());
            commands.SetHelpFormatter<HelpCommand>();
            commands.CommandErrored += Events.CommandErrored;

            discord.Ready += Events.Ready;
            discord.MessageCreated += Events.MessageCreated;
            discord.GuildCreated += Events.GuildCreated;
            discord.GuildDeleted += Events.GuildDeleted;

            await discord.ConnectAsync(new DiscordActivity()
            {
                ActivityType = ActivityType.Playing,
                Name = "with a loading bar"
            });
            await Task.Delay(-1);
        }

        private static void Init()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Db.Init();
        }
    }
}