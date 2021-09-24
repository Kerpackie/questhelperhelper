using Discord;

namespace QHH.Modules
{
    using System.IO;
    using System.Threading.Tasks;
    using Discord.Commands;
    using Discord.WebSocket;
    using QHH.Utilities;

    /// <summary>
    /// The general module containing commands like ping.
    /// </summary>
    public class GeneralModule : ModuleBase<SocketCommandContext>
    {

        private readonly Images images;

        public GeneralModule(Images images)
        {
            this.images = images;
        }

        [Command("qpc", RunMode = RunMode.Async)]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        public async Task ImageAsync(SocketGuildUser user)
        {
            string path = await this.images.CreateQuestCapeImageAsync(user);
            await Context.Channel.SendFileAsync(path);
            File.Delete(path);
        }
    }
}
