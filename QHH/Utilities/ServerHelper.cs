namespace QHH.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Discord;
    using Discord.WebSocket;
    using QHH.Common;
    using QHH.Data;
    using QHH.Data.Models;

    public class ServerHelper
    {
        private readonly Servers servers;
        private readonly Ranks ranks;
        private readonly AutoRoles autoroles;

        public ServerHelper(Servers servers, Ranks ranks, AutoRoles autoRoles)
        {
            this.servers = servers;
            this.ranks = ranks;
            this.autoroles = autoRoles;
        }

        public async Task SendLogAsync(IGuild guild, string title, string description)
        {
            var channelId = await this.servers.GetLogsChannelAsync(guild.Id);
            if (channelId == 0)
                return;

            var fetchedChannel = await guild.GetTextChannelAsync(channelId);
            if (fetchedChannel == null)
            {
                await this.servers.ClearLogsChannelAsync(guild.Id);
                return;
            }

            await fetchedChannel.SendLogAsync(title, description);
        }

        public async Task<List<IRole>> GetRanksAsync(IGuild guild)
        {
            var roles = new List<IRole>();
            var invalidRanks = new List<Rank>();

            var ranks = await this.ranks.GetRanksAsync(guild.Id);

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
                        invalidRanks.Add(rank);
                    else
                        roles.Add(role);
                }
            }

            if (invalidRanks.Count > 0)
                await this.ranks.ClearRanksAsync(invalidRanks);

            return roles;
        }

        public async Task<List<IRole>> GetAutoRolesAsync(IGuild guild)
        {
            var roles = new List<IRole>();
            var invalidAutoRoles = new List<AutoRole>();

            var autoRoles = await this.autoroles.GetAutoRolesAsync(guild.Id);

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
                        invalidAutoRoles.Add(autoRole);
                    else
                        roles.Add(role);
                }
            }

            if (invalidAutoRoles.Count > 0)
                await this.autoroles.ClearAutoRolesAsync(invalidAutoRoles);

            return roles;
        }
    }
}