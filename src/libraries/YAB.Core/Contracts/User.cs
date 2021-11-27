using YAB.Core.Annotations;

namespace YAB.Core.Contracts
{
    public class User
    {
        [PropertyDescription(false, "Normally the user name shown in chats and feeds.")]
        public string DisplayName { get; set; }

        [PropertyDescription(false, "Unique identifier for this plattform. This has to be some identifier which does not change for a given user.")]
        public string Id { get; set; }
    }
}