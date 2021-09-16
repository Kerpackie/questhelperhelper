namespace QHH.Modules
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using QHH.Data;
    using QHH.Utilities;

    /// <summary>
    /// The general module containing commands like ping.
    /// </summary>
    public class ConfigModule : ModuleBase<SocketCommandContext>
    {

        private readonly ServerHelper serverHelper;
        private readonly DataAccessLayer dataAccessLayer;

        public ConfigModule(DataAccessLayer dataAccessLayer, ServerHelper serverHelper)
        {
            this.dataAccessLayer = dataAccessLayer;
            this.serverHelper = serverHelper;
        }

        [Command("prefix", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task Prefix(string prefix = null)
        {
            if (prefix == null)
            {
                var guildPrefix = await this.dataAccessLayer.GetGuildPrefix(this.Context.Guild.Id) ?? "!";
                await this.ReplyAsync($"The current prefix of this bot is `{guildPrefix}`.");
                return;
            }

            if (prefix.Length > 8)
            {
                await this.ReplyAsync("The length of the new prefix is too long!");
                return;
            }

            await this.dataAccessLayer.ModifyGuildPrefix(this.Context.Guild.Id, prefix);
            await this.ReplyAsync($"The prefix has been adjusted to `{prefix}`.");
            await this.serverHelper.SendLogAsync(this.Context.Guild, "Prefix adjusted", $"{this.Context.User.Mention} modified the prefix to `{prefix}`.");
        }

        [Command("ranks", RunMode = RunMode.Async)]
        public async Task Ranks()
        {
            var ranks = await this.serverHelper.GetRanksAsync(Context.Guild);
            if (ranks.Count == 0)
            {
                await ReplyAsync("This server does not yet have any ranks!");
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            string description = "This message lists all available ranks.\nIn order to add a rank, you can use the name or ID of the rank.";
            foreach (var rank in ranks)
            {
                description += $"\n{rank.Mention} ({rank.Id})";
            }

            await ReplyAsync(description);
        }

        [Command("addrank", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddRank([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await this.serverHelper.GetRanksAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("That role does not exist!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("That role has a higher position than the bot!");
                return;
            }

            if (ranks.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("That role is already a rank!");
                return;
            }

            await this.dataAccessLayer.AddRankAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"The role {role.Mention} has been added to the ranks!");
        }

        [Command("delrank", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task DelRank([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await this.serverHelper.GetRanksAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("That role does not exist!");
                return;
            }

            if (ranks.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("That role is not a rank yet!");
                return;
            }

            await this.dataAccessLayer.RemoveRankAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"The role {role.Mention} has been removed from the ranks!");
        }

        [Command("autoroles", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoRoles()
        {
            var autoRoles = await this.serverHelper.GetAutoRolesAsync(Context.Guild);
            if (autoRoles.Count == 0)
            {
                await ReplyAsync("This server does not yet have any autoroles!");
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            string description = "This message lists all autoroles.\nIn order to remove an autorole, use the name or ID.";
            foreach (var autoRole in autoRoles)
            {
                description += $"\n{autoRole.Mention} ({autoRole.Id})";
            }

            await ReplyAsync(description);
        }

        [Command("addautorole", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddAutoRole([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var autoRoles = await this.serverHelper.GetAutoRolesAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("That role does not exist!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("That role has a higher position than the bot!");
                return;
            }

            if (autoRoles.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("That role is already an autorole!");
                return;
            }

            await this.dataAccessLayer.AddAutoRoleAsync(this.Context.Guild.Id, role.Id);
            await ReplyAsync($"The role {role.Mention} has been added to the autoroles!");
        }

        [Command("delautorole", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task DelAutoRole([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var autoRoles = await this.serverHelper.GetAutoRolesAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("That role does not exist!");
                return;
            }

            if (autoRoles.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("That role is not an autorole yet!");
                return;
            }

            await this.dataAccessLayer.RemoveAutoRoleAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"The role {role.Mention} has been removed from the autoroles!");
        }

        [Command("qpcimage", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task QPCImage(string args = null)
        {
            if (args == "clear")
            {
                await this.dataAccessLayer.ClearBackgroundImageAsync(Context.Guild.Id);
                await ReplyAsync("Successfully cleared the Quest Cape Image for this server, the backup image will be used instead.");
                return;
            }

            if (args != null && args != "clear")
            {
                await this.dataAccessLayer.ModifyBackgroundImageAsync(Context.Guild.Id, args);
                await ReplyAsync($"Successfully modified the Quest Cape Image to {args}.");
                return;
            }
        }

        [Command("welcome", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Welcome(string option = null, string value = null)
        {
            if (option == null && value == null)
            {
                var fetchedChannelId = await this.dataAccessLayer.GetWelcomeChannelAsync(Context.Guild.Id);
                if (fetchedChannelId == 0)
                {
                    await ReplyAsync("There has not been set a welcome channel yet!");
                    return;
                }

                var fetchedChannel = Context.Guild.GetTextChannel(fetchedChannelId);
                if (fetchedChannel == null)
                {
                    await ReplyAsync("There has not been set a welcome channel yet!");
                    await this.dataAccessLayer.ClearWelcomeChannelAsync(Context.Guild.Id);
                    return;
                }

                var fetchedBackground = await this.dataAccessLayer.GetBackgroundImageAsync(Context.Guild.Id);

                if (fetchedBackground != null)
                    await ReplyAsync($"The channel used for the welcome module is {fetchedChannel.Mention}.\nThe background is set to {fetchedBackground}.");
                else
                    await ReplyAsync($"The channel used for the welcome module is {fetchedChannel.Mention}.");

                return;
            }

            if (option == "channel" && value != null)
            {
                if (!MentionUtils.TryParseChannel(value, out ulong parsedId))
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                var parsedChannel = Context.Guild.GetTextChannel(parsedId);
                if (parsedChannel == null)
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                await this.dataAccessLayer.ModifyWelcomeChannelAsync(Context.Guild.Id, parsedId);
                await ReplyAsync($"Successfully modified the welcome channel to {parsedChannel.Mention}.");
                return;
            }

            await ReplyAsync("You did not use this command properly.");
        }

        [Command("logs")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Logs(string value = null)
        {
            if (value == null)
            {
                var fetchedChannelId = await this.dataAccessLayer.GetLogsChannelAsync(Context.Guild.Id);
                if (fetchedChannelId == 0)
                {
                    await ReplyAsync("There has not been set a logs channel yet!");
                    return;
                }

                var fetchedChannel = Context.Guild.GetTextChannel(fetchedChannelId);
                if (fetchedChannel == null)
                {
                    await ReplyAsync("There has not been set a logs channel yet!");
                    await this.dataAccessLayer.ClearLogsChannelAsync(Context.Guild.Id);
                    return;
                }

                await ReplyAsync($"The channel used for the logs is set to {fetchedChannel.Mention}.");

                return;
            }

            if (value != "clear")
            {
                if (!MentionUtils.TryParseChannel(value, out ulong parsedId))
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                var parsedChannel = Context.Guild.GetTextChannel(parsedId);
                if (parsedChannel == null)
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                await this.dataAccessLayer.ModifyLogsChannelAsync(Context.Guild.Id, parsedId);
                await ReplyAsync($"Successfully modified the logs channel to {parsedChannel.Mention}.");
                return;
            }

            if (value == "clear")
            {
                await this.dataAccessLayer.ClearLogsChannelAsync(Context.Guild.Id);
                await ReplyAsync("Successfully cleared the logs channel.");
                return;
            }

            await ReplyAsync("You did not use this command properly.");
        }
    }
}
