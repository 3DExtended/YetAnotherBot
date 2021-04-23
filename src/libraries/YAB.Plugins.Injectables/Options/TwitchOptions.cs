namespace YAB.Plugins.Injectables.Options
{
    public class TwitchOptions : Options<TwitchOptions>
    {
        public string TwitchBotToken { get; set; }

        public string TwitchBotUsername { get; set; }

        public string TwitchChannelToJoin { get; set; }
    }
}