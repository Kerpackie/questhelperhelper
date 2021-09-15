namespace QHH.Common
{
    using Discord;

    /// <summary>
    /// Class to generate embeds for Achievement diary status.
    /// </summary>
    internal class DiaryEmbedBuilder : EmbedBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiaryEmbedBuilder"/> class.
        /// The embed builder for Achievement Diary Status.
        /// </summary>
        public DiaryEmbedBuilder()
        {
            this.WithTitle("Achievement Diary Development Status");
            this.WithColor(Colors.Diary);
            this.WithFooter("Quest Helper is only officially supported via Runelite. ETA's not provided.");
        }
    }
}
