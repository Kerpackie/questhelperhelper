namespace QHH.Common
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Discord;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Extension methods used throughout QHH.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Ensure a user is promoted or has administrator permissions.
        /// </summary>
        /// <param name="socketUser">The <see cref="SocketUser"/> to be checked.</param>
        /// <returns>A bool indicating whether or not to return.</returns>
        public static bool IsPromoted(this SocketUser socketUser)
        {
            if (socketUser is not SocketGuildUser socketGuildUser)
            {
                return false;
            }

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .Build();

                var cuntRoleId = configuration.GetSection("Roles").GetValue<ulong>("Cuntributor");
                var contributorRoleId = configuration.GetSection("Roles").GetValue<ulong>("Contributor");

                var cuntRole = socketGuildUser.Guild.GetRole(cuntRoleId);
                var contributorRole = socketGuildUser.Guild.GetRole(contributorRoleId);
                if (!socketGuildUser.Roles.Contains(cuntRole) &&
                    !socketGuildUser.Roles.Contains(contributorRole) &&
                    !socketGuildUser.GuildPermissions.Administrator)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Send a log of a command and its action to a channel.
        /// </summary>
        /// <param name="channel">Channel for  the log message to be sent to.</param>
        /// <param name="title">Title of the log message.</param>
        /// <param name="description">Description of the log message.</param>
        /// <returns></returns>
        public static async Task<IMessage> SendLogAsync(this ITextChannel channel, string title, string description)
        {
            var embed = new EmbedBuilder()
                .WithColor(new Color(26, 155, 226))
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author
                    .WithIconUrl(Icons.Error)
                    .WithName(title);
                })
                .Build();

            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }
    }
}

