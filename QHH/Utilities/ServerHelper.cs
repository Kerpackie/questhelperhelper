namespace QHH.Utilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Discord;
    using Discord.WebSocket;
    using QHH.Common;
    using QHH.Data;
    using QHH.Data.Models;

    /// <summary>
    /// Basic class to handle some server functionality.
    /// </summary>
    public class ServerHelper
    {
        private readonly DataAccessLayer dataAccessLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHelper"/> class
        /// </summary>
        /// <param name="dataAccessLayer">DB layer</param>
        public ServerHelper(DataAccessLayer dataAccessLayer)
        {
            this.dataAccessLayer = dataAccessLayer;
        }

        /// <summary>
        /// Sends logs to the specified channel.
        /// </summary>
        /// <param name="guild">The Guild that the command was run in.</param>
        /// <param name="title">The title of the log</param>
        /// <param name="description">The body of the log</param>
        /// <returns>A log embed gets sent to a specified channel.</returns>
        public async Task SendLogAsync(IGuild guild, string title, string description)
        {
            var channelId = await this.dataAccessLayer.GetLogsChannelAsync(guild.Id);
            if (channelId == 0)
            {
                return;
            }

            var fetchedChannel = await guild.GetTextChannelAsync(channelId);
            if (fetchedChannel == null)
            {
                await this.dataAccessLayer.ClearLogsChannelAsync(guild.Id);
                return;
            }

            await fetchedChannel.SendLogAsync(title, description);
        }

        /// <summary>
        /// Checks for ranks that should be applied.
        /// </summary>
        /// <param name="guild">Context for GuildID</param>
        /// <returns>A list of ranks that should use the autoranks feature.</returns>
        public async Task<List<IRole>> GetRanksAsync(IGuild guild)
        {
            var roles = new List<IRole>();
            var invalidRanks = new List<Rank>();

            var ranks = await this.dataAccessLayer.GetRanksAsync(guild.Id);

            foreach (var rank in ranks)
            {
                var role = guild.Roles.FirstOrDefault(x => x.Id == rank.RoleId);
                if (role == null)
                {
                    invalidRanks.Add(rank);
                }
                else
                {
                    var currentUser = await guild.GetCurrentUserAsync();
                    var hierarchy = (currentUser as SocketGuildUser).Hierarchy;

                    if (role.Position > hierarchy)
                    {
                        invalidRanks.Add(rank);
                    }
                    else
                    {
                        roles.Add(role);
                    }
                }
            }

            if (invalidRanks.Count > 0)
            {
                await this.dataAccessLayer.ClearRanksAsync(invalidRanks);
            }

            return roles;
        }

        /// <summary>
        /// Gets the autoroles for the server.
        /// </summary>
        /// <param name="guild">Context for the Guild.</param>
        /// <returns>A list of AutoRoles.</returns>
        public async Task<List<IRole>> GetAutoRolesAsync(IGuild guild)
        {
            var roles = new List<IRole>();
            var invalidAutoRoles = new List<AutoRole>();

            var autoRoles = await this.dataAccessLayer.GetAutoRolesAsync(guild.Id);

            foreach (var autoRole in autoRoles)
            {
                var role = guild.Roles.FirstOrDefault(x => x.Id == autoRole.RoleId);
                if (role == null)
                {
                    invalidAutoRoles.Add(autoRole);
                }
                else
                {
                    var currentUser = await guild.GetCurrentUserAsync();
                    var hierarchy = (currentUser as SocketGuildUser).Hierarchy;

                    if (role.Position > hierarchy)
                    {
                        invalidAutoRoles.Add(autoRole);
                    }
                    else
                    {
                        roles.Add(role);
                    }
                }
            }

            if (invalidAutoRoles.Count > 0)
            {
                await this.dataAccessLayer.ClearAutoRolesAsync(invalidAutoRoles);
            }

            return roles;
        }
    }
}