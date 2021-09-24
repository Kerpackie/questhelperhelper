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
        private readonly QHHDbContext context;
        private readonly DataAccessLayer dataAccessLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> that should be injected.</param>
        /// <param name="client">The <see cref="DiscordSocketClient"/> that should be injected.</param>
        /// <param name="service">The <see cref="CommandService"/> that should be injected.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> that should be injected.</param>
        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration configuration, DataAccessLayer dataAccessLayer, ServerHelper serverHelper, QHHDbContext context)
        {
            this.provider = provider;
            this.client = client;
            this.service = service;
            this.configuration = configuration;
            this.serverHelper = serverHelper;
            this.dataAccessLayer = dataAccessLayer;
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

        /// <summary>
        /// Event to run when a user joins the guild.
        /// </summary>
        /// <param name="arg">the user that joined</param>
        /// <returns>A new task handling user joined events.</returns>
        private async Task OnUserJoined(SocketGuildUser arg)
        {
            var newTask = new Task(async () => await this.HandleUserJoined(arg));
            newTask.Start();
        }

        /// <summary>
        /// Handles the user joined event.
        /// </summary>
        /// <param name="arg">The user who joined the guild.</param>
        /// <returns>Adds a role to a user who joined server.</returns>
        private async Task HandleUserJoined(SocketGuildUser arg)
        {
            var roles = await this.serverHelper.GetAutoRolesAsync(arg.Guild);
            if (roles.Count > 0)
            {
                await arg.AddRoleAsync((IRole)roles);
            }
        }

        /// <summary>
        /// Updates status of achievement diary development on reaction to a certain comment.
        /// </summary>
        /// <param name="arg1">The message that was reacted.</param>
        /// <param name="arg2">The channel the message was reacted in.</param>
        /// <param name="arg3">The reaction that was used.</param>
        /// <returns>Updated development status written to the database.</returns>
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

            // No Progress - Red Circle
            if (emote == "\uD83D\uDD34")
            {
                messageId.Status = DevelopmentStatus.NoProgress;
            }

            // In Development - Yellow Circle
            if (emote == "\uD83D\uDFE1")
            {
                messageId.Status = DevelopmentStatus.InDevelopment;
            }

            // PR Submitted - Purple Circle
            if (emote == "\uD83D\uDFE3")
            {
                messageId.Status = DevelopmentStatus.PRSubmitted;
            }

            // Live - Green Circle
            if (emote == "\uD83D\uDFE2")
            {
                messageId.Status = DevelopmentStatus.Live;
            }
            else
            {
                return;
            }

            await this.context.SaveChangesAsync();

            // Edit Message after the fact, updating the status. Doesn't work 100% of the time and sometimes skips over. 
            //var editmessage = await arg1.GetOrDownloadAsync();
            //await editmessage.ModifyAsync(c => c.Content = $"***{messageId.DiaryName}*** ***{messageId.Status}***");
        }

        /// <summary>
        /// What do do when the command is run.
        /// </summary>
        /// <param name="commandInfo">Information about the command</param>
        /// <param name="commandContext">Where the command was run.</param>
        /// <param name="result">The result of the event, if it was successful etc.</param>
        /// <returns>Runs the command that was passed through.</returns>
        private async Task OnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            if (result.IsSuccess)
            {
                return;
            }

            await commandContext.Channel.SendMessageAsync(result.ErrorReason);
        }

        /// <summary>
        /// How to handle the the bot receiving a message.
        /// </summary>
        /// <param name="arg">Message passed to the bot.</param>
        /// <returns>Runs the command if it is valid.</returns>
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
