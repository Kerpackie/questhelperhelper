namespace QHH.Common
{
    using Discord;

    /// <summary>
    /// An error embed builder, will move over whatever uses this to QHHEmbedBuilder eventually.
    /// </summary>
    internal class ErrorEmbedBuilder : EmbedBuilder
    {
        public ErrorEmbedBuilder()
        {
            this.WithTitle("ERROR!");
            this.WithColor(Colors.Error);
        }
    }
}
