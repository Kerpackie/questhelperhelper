using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHH.Common
{
    public static class Extensions
    {
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

        public static async Task<IMessage> SendSuccessAsync(this ISocketMessageChannel channel, string title, string description)
        {
            var embed = new EmbedBuilder()
                .WithColor(new Color(43, 182, 115))
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author
                    .WithIconUrl("")
                    .WithName(title);
                })
                .Build();

            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendErrorAsync(this ISocketMessageChannel channel, string title, string description)
        {
            var embed = new EmbedBuilder()
                .WithColor(new Color(43, 182, 115))
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author
                    .WithIconUrl("")
                    .WithName(title);
                })
                .Build();

            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendLogAsync(this ITextChannel channel, string title, string description)
        {
            var embed = new EmbedBuilder()
                .WithColor(new Color(26, 155, 226))
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author
                    .WithIconUrl("https://i.imgur.com/gLR4k7d.png")
                    .WithName(title);
                })
                .Build();

            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }
    }
}

