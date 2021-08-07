namespace YAB.Plugins.Injectables.Options
{
    public class TwitchOptions : Options<TwitchOptions>
    {
        /// <summary>
        /// Navigate to https://dev.twitch.tv/console and register a new application for your bot
        /// </summary>
        public string TwitchBotClientId { get; set; }

        /// <summary>
        /// Navigate to https://dev.twitch.tv/console and register a new application for your bot
        /// </summary>
        public string TwitchBotSecret { get; set; }

        public string TwitchBotToken { get; set; }

        public string TwitchBotUsername { get; set; }

        public string TwitchChannelToJoin { get; set; }
    }
}