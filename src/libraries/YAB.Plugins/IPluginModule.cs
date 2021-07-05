using System;

namespace YAB.Plugins
{
    public interface IPluginModule
    {
        /// <summary>
        /// Please pass in the type of your background tasks that should be run in the background of the bot.
        /// </summary>
        /// <param name="registerer">Call this action for each background task you want to run.</param>
        public void RegisterBackgroundTasks(Action<Type> registerer);

        /// <summary>
        /// Please register all eventReactors your plugin provides.
        /// </summary>
        /// <param name="registerer">Call this action for each of your Event Reactors.</param>
        public void RegisterEventReactors(Action<Type> registerer);

        /// <summary>
        /// Please register all the events your plugin provides such that the user can register event reators on those.
        /// </summary>
        /// <param name="registerer">Call this action for each of your events.</param>
        public void RegisterPluginEvents(Action<Type> registerer);
    }
}