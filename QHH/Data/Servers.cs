namespace QHH.Data
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using QHH.Data.Context;
    using QHH.Data.Models;

    /// <summary>
    /// Server - Data Access Layer.
    /// </summary>
    public class Servers
    {
        private readonly QHHDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Servers"/> class.
        /// Passes through the DbContext required.
        /// </summary>
        /// <param name="context">The database context passed through</param>
        public Servers(QHHDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Allows for modification of the guild prefix from the database.
        /// </summary>
        /// <param name="id">ID of the server recieved from Discord.</param>
        /// <param name="prefix">The prefix of the server that is to be stored in the database.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the change to the database as a success or fail.</returns>
        public async Task ModifyGuildPrefix(ulong id, string prefix)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            if (server == null)
            {
                this.context.Add(new Server { Id = id, Prefix = prefix });
            }
            else
            {
                server.Prefix = prefix;
            }

            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the guild prefix provided to the database.
        /// </summary>
        /// <param name="id">The id of the server recieved from discord.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the database query.</returns>
        public async Task<string> GetGuildPrefix(ulong id)
        {
            var prefix = await this.context.Servers
                .AsQueryable()
                .Where(x => x.Id == id)
                .Select(x => x.Prefix)
                .FirstOrDefaultAsync();

            return await Task.FromResult(prefix);
        }

        /// <summary>
        /// Modifies the Welcome Channel for the server that is set in the Database.
        /// </summary>
        /// <param name="id">ID of the server to be updated, recieved from Discord.</param>
        /// <param name="channelId">ID of the channel to be used for the Welcome Channel.</param>
        /// <returns>A <see cref="Task"/> which updates the database with the new Welome Channel DB.</returns>
        public async Task ModifyWelcomeChannelAsync(ulong id, ulong channelId)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            if (server == null)
            {
                this.context.Add(new Server { Id = id, WelcomeChannel = channelId });
            }
            else
            {
                server.WelcomeChannel = channelId;
            }

            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Clears the status of the Welcome Channel for the specified server in the Database.
        /// </summary>
        /// <param name="id">The ID of the server, provided by Discord.</param>
        /// <returns>A <see cref="Task"/> which clears the Welcome Channel for the server ID passed.</returns>
        public async Task ClearWelcomeChannelAsync(ulong id)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            server.WelcomeChannel = 0;
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the Welcome Channel ID for the specified server.
        /// </summary>
        /// <param name="id">The ID of the server, provided by Discord.</param>
        /// <returns>A <see cref="Task{TResult}"/> which returns the welcome channel ID of the server.</returns>
        public async Task<ulong> GetWelcomeChannelAsync(ulong id)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            return await Task.FromResult(server.WelcomeChannel);
        }

        /// <summary>
        /// Modifies the active Channel to be used for Logging.
        /// </summary>
        /// <param name="id">ID of the Server, provided by Discord.</param>
        /// <param name="channelId">Channel to be used for logging.</param>
        /// <returns>A <see cref="Task"/> representing the result of the Modification to the DB.</returns>
        public async Task ModifyLogsChannelAsync(ulong id, ulong channelId)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            if (server == null)
            {
                this.context.Add(new Server { Id = id, LogsChannel = channelId });
            }
            else
            {
                server.LogsChannel = channelId;
            }

            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Clears the channelId to be used for the logs channel.
        /// </summary>
        /// <param name="id">ID Of the server, recieved from Discord.</param>
        /// <returns>A <see cref="Task"/> representing the result of the deletion of the LogsChannelId from the Database.</returns>
        public async Task ClearLogsChannelAsync(ulong id)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            server.LogsChannel = 0;
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Get the Logs ChannelID to be used for the logs channel from the DB.
        /// </summary>
        /// <param name="id">ID of the server recieved from Discord.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the query.</returns>
        public async Task<ulong> GetLogsChannelAsync(ulong id)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            return await Task.FromResult(server.LogsChannel);
        }

        /// <summary>
        /// Modifies the 'Background Image' for the server that is used for the QuestCape Congratulatory Image.
        /// </summary>
        /// <param name="id">Server ID provided by Discord.</param>
        /// <param name="url">URL to the image that should be used in the QuestCape Congratulatory Image.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ModifyBackgroundImageAsync(ulong id, string url)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            if (server == null)
            {
                this.context.Add(new Server { Id = id, BackgroundImage = url });
            }
            else
            {
                server.BackgroundImage = url;
            }

            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Clears the Background Image URL from the Database. 
        /// </summary>
        /// <param name="id">ID of the server provided by Discord.</param>
        /// <returns>A <see cref="Task"/> representing the result of the Clear Operation from the Database.</returns>
        public async Task ClearBackgroundImageAsync(ulong id)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            server.BackgroundImage = null;
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the BackgroundImage that should be used for the QuestCape Congratulatory Image Generator.
        /// </summary>
        /// <param name="id">ID of the server provided by Discord.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the DB query.</returns>
        public async Task<string> GetBackgroundImageAsync(ulong id)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            return await Task.FromResult(server.BackgroundImage);
        }
    }
}
