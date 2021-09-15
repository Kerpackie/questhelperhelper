namespace QHH.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using QHH.Data.Context;
    using QHH.Data.Models;

    /// <summary>
    /// AchievementDiary - Data Access Layer.
    /// </summary>
    public class AchievementDiaries
    {
        private readonly QHHDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Servers"/> class.
        /// Passes through the DbContext required.
        /// </summary>
        /// <param name="context">The database context passed through.</param>
        public AchievementDiaries(QHHDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<AchievementDiary>> GetAchievementDiaries()
        {
            return await this.context.AchievementDiaries
                .AsQueryable()
                .OrderBy(s => s.DiaryName)
                .ToListAsync();
        }

        /// <summary>
        /// Gets an Achievement Diary based on its ID.
        /// </summary>
        /// <param name="id">The ID of the Achievement Diary</param>
        /// <returns>A <see cref="AchievementDiary"/> Depending on if the ID exists in the Database.</returns>
        public async Task<AchievementDiary> GetAchievementDiaryByID(int id)
        {
            return await this.context.AchievementDiaries
                .FindAsync(id);
        }

        public async Task CreateAchievementDiary(ulong initiator, ulong messageId, string diaryName)
        {
            var entityEntry = this.context.Add(new AchievementDiary
            {
                Initiator = initiator,
                MessageId = messageId,
                DiaryName = diaryName,
            });

            await this.context.SaveChangesAsync();
        }

        public async Task UpdateAchievementDiaryStatus(int id, DevelopmentStatus status)
        {
            var diary = await this.context.AchievementDiaries
                .FindAsync(id);

            if (diary == null)
            {
                return;
            }

            diary.Status = status;
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteAchievementDiary(string name)
        {
            var diary = await this.context.AchievementDiaries
                .AsQueryable()
                .Where(x => x.DiaryName == name)
                .FirstOrDefaultAsync();

            if (diary == null)
            {
                return;
            }

            this.context.Remove(diary);
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Modifies the Achievement Diary Name, using the id of the diary.
        /// </summary>
        /// <param name="name">Name of the diary.</param>
        /// <param name="newName">Name that the diary should be changed to.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ModifyAchievementDiaryName(string name, string newName)
        {
            var diary = await this.context.AchievementDiaries
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

            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Modifies the Achievement Diary Initiator ID, using the ID of the diary.
        /// </summary>
        /// <param name="id">ID of the diary.</param>
        /// <param name="initiator">The ID of the user who setup the diary.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ModifyAchievementDiaryInitiator(ulong id, ulong initiator)
        {
            var diary = await this.context.AchievementDiaries
                .FindAsync(id);

            if (diary == null)
            {
                return;
            }
            else
            {
                diary.Initiator = initiator;
            }

            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Modifies the Achievement Diary Initiator ID, using the ID of the diary.
        /// </summary>
        /// <param name="id">ID of the diary.</param>
        /// <param name="messageId">The ID of the message that is used to control the Achievement Diary Status via reactions.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ModifyAchievementDiaryMessageId(ulong id, ulong messageId)
        {
            var diary = await this.context.AchievementDiaries
                .FindAsync(id);

            if (diary == null)
            {
                return;
            }
            else
            {
                diary.MessageId = messageId;
            }

            await this.context.SaveChangesAsync();
        }
    }
}
