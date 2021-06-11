using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Proty.db;
using Proty.db.models;
using Proty.utils;

namespace Proty
{
    public class Events
    {
        private static ILogger Logger { get; set; }

        public static async Task Ready(DiscordClient sender, ReadyEventArgs e)
        {
            Logger = Program.LoggerFactory.CreateLogger("Events");
            Logger.LogInformation("Proty now connected and ready !");
            await sender.UpdateStatusAsync(new DiscordActivity()
            {
                ActivityType = ActivityType.Watching,
                Name = "that everything is okay"
            }, UserStatus.Online);
        }

        public static Task MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            Task.Run(async () =>
            {
                var cnext = sender.GetCommandsNext();
                var msg = e.Message;
                
                // Check if message has valid prefix.
                var dbGuild = await Db.Instance.FetchDbGuildAsync(e.Guild);
                if (dbGuild == null)
                {
                    await e.Channel.SendMessageAsync(MessageUtils.BuildLoading("Adding your guild to the DB..."));
                    await Db.Instance.CreateDbGuild(new DbGuild()
                    {
                        GuildId = e.Guild.Id,
                        Prefix = "!",
                        Premium = false
                    });
                    Logger.LogInformation($"Added guild \"{e.Guild.Name}\" ({e.Guild.Id}) to the DB !");
                    await e.Channel.SendMessageAsync(MessageUtils.BuildSuccess("Guild added to the DB !"));
                    dbGuild = await Db.Instance.FetchDbGuildAsync(e.Guild);
                }

                var cmdStart = msg.GetStringPrefixLength(dbGuild.Prefix);
                if (cmdStart == -1) return;
                
                // Retrieve prefix.
                var prefix = msg.Content[..cmdStart];
                
                // Retrieve full command string.
                var cmdString = msg.Content[cmdStart..];
                
                var command = cnext.FindCommand(cmdString, out var args);
                
                if(command is null) return;
                
                var ctx = cnext.CreateContext(msg, prefix, command, args);
                await cnext.ExecuteCommandAsync(ctx);
            });
            return Task.CompletedTask;
        }

        public static async Task CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Exception is not ChecksFailedException exception)
            {
                if (e.Exception is ArgumentException)
                {
                    await e.Context.Channel.SendMessageAsync(MessageUtils.BuildError($"Argument(s) are not valid !\nUse `help {e.Command.QualifiedName}` to get more informations about this command."));
                    return; 
                }
                await e.Context.Channel.SendMessageAsync(MessageUtils.BuildError($"Error was not handled !\n```{e.Exception}```"));
                return;
            }
            var failedChecks = exception.FailedChecks[0];
            
            switch (failedChecks)
            {
                case PremiumOnly:
                    await e.Context.Channel.SendMessageAsync(MessageUtils.BuildError("This server is not premium and this command is premium only !", "💎"));
                    break;
                case RequireUserPermissionsAttribute p:
                    await e.Context.Channel.SendMessageAsync(MessageUtils.BuildError($"You require the permission(s) `{string.Join(", ", p.Permissions)}` to use this command !", "🛡"));
                    break;
                case RequireBotPermissionsAttribute p:
                    await e.Context.Channel.SendMessageAsync(MessageUtils.BuildError($"The bot requires the permission(s) `{string.Join(", ", p.Permissions)}` to run this command !", "🛡"));
                    break;
            }
            await e.Context.Channel.SendMessageAsync(MessageUtils.BuildError($"Error was not handled !\n```{e.Exception}```"));
        }

        public static async Task GuildCreated(DiscordClient sender, GuildCreateEventArgs e)
        {
            await Task.Run(async () =>
            {
                await Db.Instance.CreateDbGuild(new DbGuild()
                {
                    GuildId = e.Guild.Id,
                    Prefix = "!",
                    Premium = false
                });
            });
        }

        public static async Task GuildDeleted(DiscordClient sender, GuildDeleteEventArgs e)
        {
            await Task.Run(async () =>
            {
                await Db.Instance.RemoveDbGuild(e.Guild.Id);
            });
        }
    }
}