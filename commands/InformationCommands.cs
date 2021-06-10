﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
    public class InformationCommands : BaseCommandModule
    {
        public Db Db { private get; set; }
        private DateTime _startTime = DateTime.Now;
        
        [Command("ping")]
        [Description("Get the ping of the bot to the Gateway and the DB.")]
        public async Task Ping(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
            {
                Title = "📶 Ping",
                Color = DiscordColor.Blue
            };
            embed.AddField("Gateway", $"`{ctx.Client.Ping} ms`");
            embed.AddField("Database", $"`{await Db.GetPingAsync()} ms`");
            await ctx.Channel.SendMessageAsync(embed);
        }
        
        [Command("uptime")]
        [Description("Get the ping of the bot to the Gateway and the DB.")]
        public async Task Uptime(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
            {
                Title = "⏲ Uptime",
                Description = $"Bot is up since `{MessageUtils.HumanReadable(DateTime.Now - _startTime)}` !",
                Color = DiscordColor.Blue
            };
            await ctx.Channel.SendMessageAsync(embed);
        }

        [Command("premium")]
        [Description("Get more information about Proty's premium and whether this guild is premium or not.")]
        public async Task Premium(CommandContext ctx)
        {
            var guild = await Db.FetchDbGuildAsync(ctx.Guild);
            var embed = new DiscordEmbedBuilder()
            {
                Title = "💎 Premium",
                Description = $"This guild is {(guild.Premium ? "" : "not")} premium !",
                Color = guild.Premium ? DiscordColor.Gold : DiscordColor.Red
            };
            await ctx.Channel.SendMessageAsync(embed);
        }

        [Command("botinfo")]
        [Description("Get to know the bot's owner, library, ...")]
        [Aliases("info", "bot")]
        public async Task BotInfo(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
            {
                Title = "🤖 Bot Info",
                Description = $"This are some informations about the bot !",
                Color = DiscordColor.Blue
            };

            embed.AddField("Creator", "Proty was made by [Lygaen#1501](https://github.com/Lygaen)");
            embed.AddField("Library", $"This bot is made with the [Dsharp Plus](https://dsharpplus.github.io/) library");
            
            await ctx.Channel.SendMessageAsync(embed);
        }

    }
}