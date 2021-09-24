namespace QHH.Common
{
    using Discord;

    /// <summary>
    /// A hackey embed builder for adding achievement diaries. Do something better with this eventually.
    /// </summary>
    internal class AddDiaryEmbedBuilder : EmbedBuilder
    {
        public AddDiaryEmbedBuilder()
        {
            this.WithTitle("Achievement Diary Status Control");
            this.WithColor(Colors.Diary25Dark);
        }
    }
}
