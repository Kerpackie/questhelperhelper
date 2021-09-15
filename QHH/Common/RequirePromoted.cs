using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHH.Common
{
    class RequirePromoted : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User is SocketGuildUser user)
            {
                var contributorRoleId = services.GetRequiredService<IConfiguration>().GetSection("Roles").GetValue<ulong>("Contributor");
                var cuntRoleId = services.GetRequiredService<IConfiguration>().GetSection("Roles").GetValue<ulong>("Cuntributor");

                var contributorRole = context.Guild.GetRole(contributorRoleId);
                var cuntRole = context.Guild.GetRole(cuntRoleId);

                if (user.Roles.Contains(cuntRole) || user.Roles.Contains(contributorRole))
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromError("User is not authorized to manage FAQ's"));
                }
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError("Command was run outide of a guild.")); //Shouldn't ever be called.
            }
        }
    }
}
