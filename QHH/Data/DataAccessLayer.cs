namespace QHH.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using QHH.Data.Context;
    using QHH.Data.Models;

    /// <summary>
    /// The business layer of accessing the database.
    /// </summary>
    public class DataAccessLayer
    {
        private readonly QHHDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessLayer"/> class.
        /// </summary>
        /// <param name="dbContext">the database context required.</param>
        public DataAccessLayer(QHHDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Gets achievement diaries by name.
        /// </summary>
        /// <returns>a list of achievement diaries.</returns>
        public async Task<IEnumerable<AchievementDiary>> GetAchievementDiaries()
        {
            return await this.dbContext.AchievementDiaries
                .AsQueryable()
                .OrderBy(s => s.DiaryName)
                .ToListAsync();
        }

        /// <summary>
        /// Gets an Achievement Diary based on its ID.
        /// </summary>
        /// <param name="id">The ID of the Achievement Diary</param>
        /// <returns>A <see cref="AchievementDiary"/> Depending on if the ID exists in the Database.</returns>
        public async Task<AchievementDiary> GetAchievementDiaryMessageID(ulong id)
        {
            return await this.dbContext.AchievementDiaries
                .AsQueryable()
                .Where(x => x.MessageId == id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new achievement diary.
        /// </summary>
        /// <param name="initiator">User who created the diary.</param>
        /// <param name="messageId">Message used to control the diary status.</param>
        /// <param name="diaryName">Name of the diary.</param>
        /// <returns>Creates a new diary and writes it to the database.</returns>
        public async Task CreateAchievementDiary(ulong initiator, ulong messageId, string diaryName)
        {
            var entityEntry = this.dbContext.Add(new AchievementDiary
            {
                Initiator = initiator,
                MessageId = messageId,
                DiaryName = diaryName,
            });

            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the status of an achievement diary
        /// </summary>
        /// <param name="id">ID of the diary to be updated.</param>
        /// <param name="status">Status of the diary to be updated.</param>
        /// <returns>An updated diary.</returns>
        public async Task UpdateAchievementDiaryStatus(int id, DevelopmentStatus status)
        {
            var diary = await this.dbContext.AchievementDiaries
                .FindAsync(id);

            if (diary == null)
            {
                return;
            }

            diary.Status = status;
            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an Achievement Diary.
        /// </summary>
        /// <param name="name">Name of the diary to be deleted.</param>
        /// <returns>Nothing, its gone. Poof.</returns>
        public async Task DeleteAchievementDiary(string name)
        {
            var diary = await this.dbContext.AchievementDiaries
                .AsQueryable()
                .Where(x => x.DiaryName == name)
                .FirstOrDefaultAsync();

            if (diary == null)
            {
                return;
            }

            this.dbContext.Remove(diary);
            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Modifies the Achievement Diary Name, using the id of the diary.
        /// </summary>
        /// <param name="name">Name of the diary.</param>
        /// <param name="newName">Name that the diary should be changed to.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ModifyAchievementDiaryName(string name, string newName)
        {
            var diary = await this.dbContext.AchievementDiaries
                .AsQueryable()
                .Where(x => x.DiaryName == name)
                .FirstOrDefaultAsync();

            if (diary == null)
            {
                return;
            }
            else
            {
                diary.DiaryName = newName;
            }

            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Modifies the Achievement Diary Initiator ID, using the ID of the diary.
        /// </summary>
        /// <param name="id">ID of the diary.</param>
        /// <param name="initiator">The ID of the user who setup the diary.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ModifyAchievementDiaryInitiator(ulong id, ulong initiator)
        {
            var diary = await this.dbContext.AchievementDiaries
                .FindAsync(id);

            if (diary == null)
            {
                return;
            }
            else
            {
                diary.Initiator = initiator;
            }

            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Modifies the Achievement Diary Initiator ID, using the ID of the diary.
        /// </summary>
        /// <param name="id">ID of the diary.</param>
        /// <param name="messageId">The ID of the message that is used to control the Achievement Diary Status via reactions.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ModifyAchievementDiaryMessageId(ulong id, ulong messageId)
        {
            var diary = await this.dbContext.AchievementDiaries
                .FindAsync(id);

            if (diary == null)
            {
                return;
            }
            else
            {
                diary.MessageId = messageId;
            }

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<FAQ> GetFAQ(string faqName)
        {
            return await this.dbContext.FAQs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name == faqName);
        }

        public async Task<IEnumerable<FAQ>> GetFAQs()
        {
            return await this.dbContext.FAQs
                .AsQueryable()
                .ToListAsync();
        }

        public async Task CreateFaq(string name, ulong ownerId, string content)
        {
            var faq = await this.dbContext.FAQs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name == name);

            if (faq != null)
            {
                return;
            }

            this.dbContext.Add(new FAQ
            {
                Name = name,
                OwnerId = ownerId,
                Content = content,
            });

            await this.dbContext.SaveChangesAsync();
        }

        public async Task EditFAQContent(string faqName, string content)
        {
            var faq = await this.dbContext.FAQs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name == faqName);

            if (faqName == null)
            {
                return;
            }

            faq.Content = content;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task EditFAQOwner(string faqName, ulong ownerId)
        {
            var faq = await this.dbContext.FAQs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name == faqName);

            if (faq == null)
            {
                return;
            }

            faq.OwnerId = ownerId;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteFAQ(string faqName)
        {
            var faq = await this.dbContext.FAQs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name == faqName);

            if (faq == null)
            {
                return;
            }

            this.dbContext.Remove(faq);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<List<AutoRole>> GetAutoRolesAsync(ulong id)
        {
            var autoRoles = await this.dbContext.AutoRoles
                .AsQueryable()
                .Where(x => x.ServerId == id)
                .ToListAsync();

            return await Task.FromResult(autoRoles);
        }

        public async Task AddAutoRoleAsync(ulong id, ulong roleId)
        {
            var server = await this.dbContext.Servers
                .FindAsync(id);

            if (server == null)
            {
                this.dbContext.Add(new Server { Id = id });
            }

            this.dbContext.Add(new AutoRole { RoleId = roleId, ServerId = id });
            await this.dbContext.SaveChangesAsync();
        }

        public async Task RemoveAutoRoleAsync(ulong id, ulong roleId)
        {
            var autoRole = await this.dbContext.AutoRoles
                .AsQueryable()
                .Where(x => x.RoleId == roleId)
                .FirstOrDefaultAsync();

            this.dbContext.Remove(autoRole);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task ClearAutoRolesAsync(List<AutoRole> autoRoles)
        {
            this.dbContext.RemoveRange(autoRoles);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<List<Rank>> GetRanksAsync(ulong id)
        {
            var ranks = await this.dbContext.Ranks
                .AsQueryable()
                .Where(x => x.ServerId == id)
                .ToListAsync();

            return await Task.FromResult(ranks);
        }

        public async Task AddRankAsync(ulong id, ulong roleId)
        {
            var server = await this.dbContext.Servers
                .FindAsync(id);

            if (server == null)
                this.dbContext.Add(new Server { Id = id });

            this.dbContext.Add(new Rank { RoleId = roleId, ServerId = id });
            await this.dbContext.SaveChangesAsync();
        }

        public async Task RemoveRankAsync(ulong id, ulong roleId)
        {
            var rank = await this.dbContext.Ranks
                .AsQueryable()
                .Where(x => x.RoleId == roleId)
                .FirstOrDefaultAsync();

            this.dbContext.Remove(rank);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task ClearRanksAsync(List<Rank> ranks)
        {
            this.dbContext.RemoveRange(ranks);
            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Allows for modification of the guild prefix from the database.
        /// </summary>
        /// <param name="id">ID of the server recieved from Discord.</param>
        /// <param name="prefix">The prefix of the server that is to be stored in the database.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the change to the database as a success or fail.</returns>
        public async Task ModifyGuildPrefix(ulong id, string prefix)
        {
            var server = await this.dbContext.Servers
                .FindAsync(id);

            if (server == null)
            {
                this.dbContext.Add(new Server { Id = id, Prefix = prefix });
            }
            else
            {
                server.Prefix = prefix;
            }

            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the guild prefix provided to the database.
        /// </summary>
        /// <param name="id">The id of the server recieved from discord.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the database query.</returns>
        public async Task<string> GetGuildPrefix(ulong id)
        {
            var prefix = await this.dbContext.Servers
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
            var server = await this.dbContext.Servers
                .FindAsync(id);

            if (server == null)
            {
                this.dbContext.Add(new Server { Id = id, WelcomeChannel = channelId });
            }
            else
            {
                server.WelcomeChannel = channelId;
            }

            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Clears the status of the Welcome Channel for the specified server in the Database.
        /// </summary>
        /// <param name="id">The ID of the server, provided by Discord.</param>
        /// <returns>A <see cref="Task"/> which clears the Welcome Channel for the server ID passed.</returns>
        public async Task ClearWelcomeChannelAsync(ulong id)
        {
            var server = await this.dbContext.Servers
                .FindAsync(id);

            server.WelcomeChannel = 0;
            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the Welcome Channel ID for the specified server.
        /// </summary>
        /// <param name="id">The ID of the server, provided by Discord.</param>
        /// <returns>A <see cref="Task{TResult}"/> which returns the welcome channel ID of the server.</returns>
        public async Task<ulong> GetWelcomeChannelAsync(ulong id)
        {
            var server = await this.dbContext.Servers
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
            var server = await this.dbContext.Servers
                .FindAsync(id);

            if (server == null)
            {
                this.dbContext.Add(new Server { Id = id, LogsChannel = channelId });
            }
            else
            {
                server.LogsChannel = channelId;
            }

            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Clears the channelId to be used for the logs channel.
        /// </summary>
        /// <param name="id">ID Of the server, recieved from Discord.</param>
        /// <returns>A <see cref="Task"/> representing the result of the deletion of the LogsChannelId from the Database.</returns>
        public async Task ClearLogsChannelAsync(ulong id)
        {
            var server = await this.dbContext.Servers
                .FindAsync(id);

            server.LogsChannel = 0;
            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Get the Logs ChannelID to be used for the logs channel from the DB.
        /// </summary>
        /// <param name="id">ID of the server recieved from Discord.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the query.</returns>
        public async Task<ulong> GetLogsChannelAsync(ulong id)
        {
            var server = await this.dbContext.Servers
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
            var server = await this.dbContext.Servers
                .FindAsync(id);

            if (server == null)
            {
                this.dbContext.Add(new Server { Id = id, BackgroundImage = url });
            }
            else
            {
                server.BackgroundImage = url;
            }

            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Clears the Background Image URL from the Database. 
        /// </summary>
        /// <param name="id">ID of the server provided by Discord.</param>
        /// <returns>A <see cref="Task"/> representing the result of the Clear Operation from the Database.</returns>
        public async Task ClearBackgroundImageAsync(ulong id)
        {
            var server = await this.dbContext.Servers
                .FindAsync(id);

            server.BackgroundImage = null;
            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the BackgroundImage that should be used for the QuestCape Congratulatory Image Generator.
        /// </summary>
        /// <param name="id">ID of the server provided by Discord.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the DB query.</returns>
        public async Task<string> GetBackgroundImageAsync(ulong id)
        {
            var server = await this.dbContext.Servers
                .FindAsync(id);

            return await Task.FromResult(server.BackgroundImage);
        }
    }
}
