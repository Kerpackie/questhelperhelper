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
            this.WithColor(new Color(238, 62, 75));
            this.WithFooter("Quest Helper is only officially supported via runelite. Support will not be provided for any other third party clients using Quest Helper. This project is developed 100% by volunteers. Continiously asking for updates and ETAs, may result in a mute or a suspension from the server!");
        }
    }

    internal class AddDiaryEmbedBuilder : EmbedBuilder
    {
        public AddDiaryEmbedBuilder()
        {
            this.WithTitle("Achievement Diary Status Control");
            this.WithColor(new Color(238, 62, 75));
        }
    }

    internal class ErrorEmbedBuilder : EmbedBuilder
    {
        public ErrorEmbedBuilder()
        {
            this.WithTitle("ERROR!");
            this.WithColor(new Color(238, 62, 75));
        }
    }
}
