namespace YAB.Core.Contracts
{
    public class User
    {
        /// <summary>
        /// Normally the user name shown in chats and feeds.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Unique identifier for this plattform.
        /// This has to be some identifier which does not change for a given user.
        /// </summary>
        public string Id { get; set; }

        public PluginPlattform Plattform { get; set; }
    }
}