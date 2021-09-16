namespace QHH.Services
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Addons.Hosting;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using QHH.Data;
    using QHH.Data.Context;
    using QHH.Data.Models;
    using QHH.Utilities;

    /// <summary>
    /// The class responsible for handling the commands and various events.
    /// </summary>
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider provider;
        private readonly DiscordSocketClient client;
        private readonly CommandService service;
        private readonly IConfiguration configuration;
        private readonly ServerHelper serverHelper;
        //private readonly Servers servers;
        private readonly Images images;
        private readonly QHHDbContext context;
        private readonly DataAccessLayer dataAccessLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> that should be injected.</param>
        /// <param name="client">The <see cref="DiscordSocketClient"/> that should be injected.</param>
        /// <param name="service">The <see cref="CommandService"/> that should be injected.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> that should be injected.</param>
        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration configuration, DataAccessLayer dataAccessLayer, ServerHelper serverHelper, Images images, QHHDbContext context)
        {
            this.provider = provider;
            this.client = client;
            this.service = service;
            this.configuration = configuration;
            this.serverHelper = serverHelper;
            this.dataAccessLayer = dataAccessLayer;
            this.images = images;
            this.context = context;
        }

        /// <inheritdoc/>
        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            this.client.MessageReceived += this.OnMessageReceived;
            this.client.ReactionAdded += this.OnAchievementDiaryStatusReacted;
            this.client.UserJoined += this.OnUserJoined;
            this.service.CommandExecuted += this.OnCommandExecuted;
            await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }

        private async Task OnUserJoined(SocketGuildUser arg)
        {
            var newTask = new Task(async () => await HandleUserJoined(arg));
            newTask.Start();
        }

        private async Task HandleUserJoined(SocketGuildUser arg)
        {
            var roles = await this.serverHelper.GetAutoRolesAsync(arg.Guild);
            if (roles.Count > 0)
            {
                await arg.AddRoleAsync((IRole)roles);
            }
        }

        private async Task OnAchievementDiaryStatusReacted(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var thisMessageId = arg3.MessageId;
            var emote = arg3.Emote.Name;

            var messageId = await this.context.AchievementDiaries
                .AsQueryable()
                .Where(x => x.MessageId == thisMessageId)
                .FirstOrDefaultAsync();

            if (messageId == null)
            {
                return;
            }

            //No Progress - Red Circle
            if (emote == "\uD83D\uDD34")
            {
                messageId.Status = DevelopmentStatus.NoProgress;
            }

            //In Development - Yellow Circle
            if (emote == "\uD83D\uDFE1")
            {
                messageId.Status = DevelopmentStatus.InDevelopment;
            }

            //PR Submitted - Purple Circle
            if (emote == "\uD83D\uDFE3")
            {
                messageId.Status = DevelopmentStatus.PRSubmitted;
            }

            //Live - Green Circle
            if (emote == "\uD83D\uDFE2")
            {
                messageId.Status = DevelopmentStatus.Live;
            }
            else
            {
                return;
            }

            await this.context.SaveChangesAsync();
            //var editmessage = await arg1.GetOrDownloadAsync();
            //await editmessage.ModifyAsync(c => c.Content = $"***{messageId.DiaryName}*** ***{messageId.Status}***");
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            if (result.IsSuccess)
            {
                return;
            }

            await commandContext.Channel.SendMessageAsync(result.ErrorReason);
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message))
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            var argPos = 0;
            var prefix = await this.dataAccessLayer.GetGuildPrefix((message.Channel as SocketGuildChannel).Guild.Id) ?? "!";

            if (!message.HasStringPrefix(prefix, ref argPos) && !message.HasMentionPrefix(this.client.CurrentUser, ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(this.client, message);
            await this.service.ExecuteAsync(context, argPos, this.provider);
        }
    }
}
