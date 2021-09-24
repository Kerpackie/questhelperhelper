namespace QHH.Common
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Attribute used to ensure the user is promoted (a Contributor or higher).
    /// </summary>
    public class RequirePromoted : PreconditionAttribute
    {
        /// <summary>
        /// Checks if the attribute will pass or fail.
        /// </summary>
        /// <param name="context">The underlying context.</param>
        /// <param name="command">The command that this attribute was placed on.</param>
        /// <param name="services">The services inside our service pool.</param>
        /// <returns>Returns if a user is authorized to manage tags.</returns>
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
                return Task.FromResult(PreconditionResult.FromError("Command was run outide of a guild.")); // Shouldn't ever be called.
            }
        }
    }
}
