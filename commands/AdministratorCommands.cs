using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using Proty.db;
using Proty.utils;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Proty.commands
{
    public class AdministratorCommands : BaseCommandModule
    {
        public Db Db { private get; set; }
        
        [Command("prefix")]
        [Description("Sets the prefix of the bot.")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task Prefix(CommandContext ctx, [Description("The new prefix.")] string prefix)
        {
            if (prefix.Length > 3)
            {
                await ctx.Channel.SendMessageAsync(MessageUtils.BuildError("Prefix length caps to 3 ! This prefix is too long !"));
                return;
            }
            
            var guild = await Db.FetchDbGuildAsync(ctx.Guild);
            guild.Prefix = prefix;
            await Db.UpdateDbGuild(guild);
            await ctx.Channel.SendMessageAsync(MessageUtils.BuildSuccess($"Prefix was successfully changed to `{prefix}` !"));
        }

        [Command("announce")]
        [Description("Announce a message with the bot in a certain channel.")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task Announce(CommandContext ctx, [Description("The channel to announce in.")] DiscordChannel channel, [Description("The message to announce.")] string message)
        {
            if (!channel.PermissionsFor(ctx.Member).HasFlag(Permissions.SendMessages))
                throw new ChecksFailedException(ctx.Command, ctx,
                    new[] {new RequireUserPermissionsAttribute(Permissions.SendMessages)});
            
            if (!channel.PermissionsFor(ctx.Guild.Members[ctx.Client.CurrentUser.Id]).HasFlag(Permissions.SendMessages))
                throw new ChecksFailedException(ctx.Command, ctx,
                    new[] {new RequireBotPermissionsAttribute(Permissions.SendMessages)});
            
            await channel.SendMessageAsync(new DiscordEmbedBuilder()
            {
                Title = "📢 Announcement",
                Description = message,
                Color = DiscordColor.Blue,
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    IconUrl = ctx.User.AvatarUrl,
                    Name = ctx.Member.Nickname
                }
            });
        }
    }
}