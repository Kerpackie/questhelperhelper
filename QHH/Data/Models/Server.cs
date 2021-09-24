namespace QHH.Data.Models
{
    /// <summary>
    /// An Object of the Type Server - Contains information such as Server ID, Prefix, Channel Id's etc.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Gets or Sets the ID of the Server.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Gets or Sets the prefix for bot to execute commands.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or Sets the ID of the channel that should be used as the welcome channel.
        /// </summary>
        public ulong WelcomeChannel { get; set; }

        /// <summary>
        /// Gets or Sets the ID of the channel that should be used as the logs channel.
        /// </summary>
        public ulong LogsChannel { get; set; }

        /// <summary>
        /// Gets or sets the url to the 'Background Image' used to create congratulations images.
        /// </summary>
        public string BackgroundImage { get; set; }
    }
}
