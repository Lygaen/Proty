using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Proty.db;

namespace Proty.utils
{
    public class PremiumOnly : CheckBaseAttribute
    {
        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return help || (await Db.Instance.FetchDbGuildAsync(ctx.Guild)).Premium;
        }

        public static bool IsPremium(Command command)
        {
            return command.ExecutionChecks.Any(c => c is PremiumOnly);
        }
    }
}