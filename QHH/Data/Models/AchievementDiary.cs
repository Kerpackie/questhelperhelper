using System.ComponentModel.DataAnnotations;

namespace QHH.Data.Models
{
    /// <summary>
    /// Enum to indicate the development status of an Achievement Status.
    /// </summary>
    public enum DevelopmentStatus
    {
        /// <summary>
        /// There is no progress on the development status, or the development has not started. 
        /// </summary>
        NoProgress,

        /// <summary>
        /// The Achievement Diary is currently in Development
        /// </summary>
        InDevelopment,

        /// <summary>
        /// A PR has been submitted to Github for the Achievement Diary.
        /// </summary>
        PRSubmitted,

        /// <summary>
        /// The Achievement Diary is released in the live version of the game, should only be set after the update is in the client.
        /// </summary>
        Live,
    }

    /// <summary>
    /// An achievement diary object to track development status.
    /// </summary>
    public class AchievementDiary
    {
        /// <summary>
        /// Gets or Sets the ID value of the Achievement Diary Object.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or Sets the MessageID of the message used to act as a key.
        /// </summary>
        public ulong MessageId { get; set; }

        /// <summary>
        /// Gets or Sets the ID of the person who initiated the development project.
        /// </summary>
        public ulong Initiator { get; set; }

        /// <summary>
        /// Gets or Sets the name of the Achievement Diary.
        /// </summary>
        public string DiaryName { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="DevelopmentStatus"/> for the Achievement Diary.
        /// </summary>
        public DevelopmentStatus Status { get; set; } = DevelopmentStatus.NoProgress;
    }
}
