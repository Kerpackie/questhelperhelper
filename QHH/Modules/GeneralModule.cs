namespace QHH.Modules
{
    using System.IO;
    using System.Threading.Tasks;
    using Discord.Commands;
    using Discord.WebSocket;
    using QHH.Common;
    using QHH.Data;
    using QHH.Utilities;

    /// <summary>
    /// The general module containing commands like ping.
    /// </summary>
    public class GeneralModule : ModuleBase<SocketCommandContext>
    {

        private readonly Servers servers;
        private readonly Ranks ranks;
        private readonly AutoRoles autoRoles;
        private readonly ServerHelper serverhelper;
        private readonly Images images;

        public GeneralModule(Servers servers, Ranks ranks, AutoRoles autoRoles, ServerHelper serverHelper, Images images)
        {
            this.servers = servers;
            this.ranks = ranks;
            this.autoRoles = autoRoles;
            this.serverhelper = serverHelper;
            this.images = images;
        }

        [Command("qpc", RunMode = RunMode.Async)]
        public async Task ImageAsync(SocketGuildUser user)
        {
            string path = await this.images.CreateQuestCapeImageAsync(user);
            await Context.Channel.SendFileAsync(path);
            File.Delete(path);
        }
    }
}
