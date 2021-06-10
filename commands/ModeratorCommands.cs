using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Proty.db;
using Proty.utils;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Proty.commands
{
    public class ModeratorCommands : BaseCommandModule
    {
        public Db Db { private get; set; }
        
        [Command("ban")]
        [Description("Bans a specific member with a reason (optional)")]
        [RequireUserPermissions(Permissions.BanMembers)]
        [RequireBotPermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, [Description("The member to Ban.")] DiscordMember member, [Description("The reason on why to ban this member.")] string reason = "No reason given")
        {
            await member.BanAsync(7, reason:"Proty Ban : "+reason);
            await ctx.Channel.SendMessageAsync(MessageUtils.BuildSuccess($"Member {member.Mention} ({member.Id}) was banned from the server !"));
        }
        
        [Command("kick")]
        [Description("Kicks a specific member with a reason (optional)")]
        [RequireUserPermissions(Permissions.KickMembers)]
        [RequireBotPermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx, [Description("The member to Kick.")] DiscordMember member, [Description("The reason on why to kick this member.")] string reason = "No reason given")
        {
            await member.RemoveAsync("Proty Kick : "+reason);
            await ctx.Channel.SendMessageAsync(MessageUtils.BuildSuccess($"Member {member.Mention} ({member.Id}) was kicked from the server !"));
        }
        
        [Command("softban")]
        [Description("Bans then Unbans a member to delete his messages and kick him")]
        [RequireUserPermissions(Permissions.BanMembers)]
        [RequireBotPermissions(Permissions.BanMembers)]
        public async Task SoftBan(CommandContext ctx, [Description("The member to Soft Ban.")] DiscordMember member, [Description("The reason on why to soft ban this member.")] string reason = "No reason given")
        {
            await member.BanAsync(7, reason:"Proty Soft Ban : "+reason);
            await member.UnbanAsync("Proty Soft Ban : "+reason);
            await ctx.Channel.SendMessageAsync(MessageUtils.BuildSuccess($"Member {member.Mention} ({member.Id}) was soft banned from the server !"));
        }

        [Command("clear")]
        [Description("Clears the channel for a certain amount of message.")]
        public async Task Clear(CommandContext ctx, [Description("The amount of message to delete, will cap at 200.")] int amount = 100)
        {
            try
            {
                await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync(Math.Max(amount, 200)));
            }
            catch (BadRequestException e)
            {
                await ctx.Channel.SendMessageAsync(MessageUtils.BuildError($"Couldn't delete all messages, some are older than 14 days (Discord limit when purging)."));
                return;
            }

            await ctx.Channel.SendMessageAsync(MessageUtils.BuildSuccess($"Successfully deleted `{amount}` messages !"));
        }
    }
}