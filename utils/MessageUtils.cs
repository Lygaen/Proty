using System;
using DSharpPlus.Entities;

namespace Proty.utils
{
    public static class MessageUtils
    {
        public static DiscordEmbed BuildLoading(string message)
        {
            return new DiscordEmbedBuilder()
            {
                Title = "⌛ Wait...",
                Description = message,
                Color = DiscordColor.Blue
            }.Build();
        }

        public static DiscordEmbed BuildSuccess(string message)
        {
            return new DiscordEmbedBuilder()
            {
                Title = "✔ Success",
                Description = message,
                Color = DiscordColor.Green
            }.Build();
        }
        
        public static DiscordEmbed BuildError(string message, string emoji = "❌")
        {
            return new DiscordEmbedBuilder()
            {
                Title = emoji+" Error",
                Description = message,
                Color = DiscordColor.Red
            }.Build();
        }

        public static string HumanReadable(TimeSpan time)
        {
            if (time.Days >= 1)
            {
                return $"{time.Days} day{(time.Days > 1 ? "s" : "")}";
            }
            if (time.Hours >= 1)
            {
                return $"{time.Hours} hour{(time.Hours > 1 ? "s" : "")}";
            }
            
            return time.Minutes >= 1 ? $"{time.Minutes} minute{(time.Minutes > 1 ? "s" : "")}" : $"{time.Seconds} second{(time.Seconds > 1 ? "s" : "")}";
        }
    }
}