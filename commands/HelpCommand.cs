using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using Proty.utils;

namespace Proty.commands
{
    public class HelpCommand : BaseHelpFormatter
    {
        private readonly CommandContext _ctx;
        private readonly DiscordEmbedBuilder _embed;

        public HelpCommand(CommandContext ctx) : base(ctx)
        {
            _ctx = ctx;
            _embed = new DiscordEmbedBuilder();
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            _embed.Color = DiscordColor.Blue;
            _embed.Title = $"\u2139 {command.Module.ModuleType.Name[..^8]} - {command.QualifiedName}{(PremiumOnly.IsPremium(command) ? "💎" : "")}";
            _embed.Description = command.Description;
            
            var str = new StringBuilder();
            
            if (command.Aliases.Count != 0)
            {
                foreach (var alias in command.Aliases)
                {
                    str.Append('`').Append(alias).Append("`,");
                }

                str.Remove(str.Length - 1, 1); // Removes the last ','

                _embed.AddField("Aliases", str.ToString());
            }

            str.Clear();

            var overload = command.Overloads[0];

            if (overload.Arguments.Count > 0)
            {
                foreach (var argument in overload.Arguments)
                {
                    str.Append('`')
                        .Append(argument.IsOptional ? '(' : "")
                        .Append(_ctx.CommandsNext.GetUserFriendlyTypeName(argument.Type))
                        .Append(argument.IsCatchAll ? "..." : "")
                        .Append(argument.IsOptional ? ')' : "")
                        .Append('`')
                        .Append(" "+argument.Name+" : ")
                        .Append(argument.Description ?? "No description provided.")
                        .Append('\n');
                }
            }
            else
            {
                str.Append("No arguments required.");
            }

            _embed.AddField("Arguments", str.ToString());

            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            _embed.Title = "\u2139 Help";
            _embed.Color = DiscordColor.Blue;
            _embed.Description = "Here are all of the Proty's available commands !";
            
            var infoString = new StringBuilder();
            var modString = new StringBuilder();
            var adminString = new StringBuilder();
            var others = new StringBuilder();
            
            foreach (var subcommand in subcommands)
            {
                switch (subcommand.Module.GetInstance(Program.Services))
                {
                    case InformationCommands:
                        infoString
                            .Append('`')
                            .Append(subcommand.Name)
                            .Append(PremiumOnly.IsPremium(subcommand) ? "💎" : "")
                            .Append("`, ");
                            
                        break;
                    case ModeratorCommands:
                        modString
                            .Append('`')
                            .Append(subcommand.Name)
                            .Append(PremiumOnly.IsPremium(subcommand) ? "💎" : "")
                            .Append("`, ");
                            
                        break;
                    case AdministratorCommands:
                        adminString
                            .Append('`')
                            .Append(subcommand.Name)
                            .Append(PremiumOnly.IsPremium(subcommand) ? "💎" : "")
                            .Append("`, ");
                            
                        break;
                    default:
                        others
                            .Append('`')
                            .Append(subcommand.Name)
                            .Append(PremiumOnly.IsPremium(subcommand) ? "💎" : "")
                            .Append("`, ");
                            
                        break;
                }
            }
            
            if (infoString.Length > 2)
            {
                infoString.Remove(infoString.Length - 2, 2);
                _embed.AddField("Moderator", infoString.ToString());
            }

            if (modString.Length > 2)
            {
                modString.Remove(modString.Length - 2, 2);
                _embed.AddField("Moderator", modString.ToString());
            }
            
            if (adminString.Length > 2)
            {
                adminString.Remove(adminString.Length - 2, 2);
                _embed.AddField("Moderator", adminString.ToString());
            }

            if (others.Length <= 0) return this;
            
            others.Remove(others.Length - 2, 2);
            _embed.AddField("Others", others.ToString());

            return this;
        }

        public override CommandHelpMessage Build()
        {
            return new(embed: _embed);
        }
    }
}