namespace YAB.Core.EventReactor
{
    /// <summary>
    /// Most pipeline handler should be configurable by the enduser/bot administrator.
    /// </summary>
    public interface IEventReactorConfiguration
    {
        /// <summary>
        /// If set, the execution of this handler is delayed by this amount of seconds.
        /// </summary>
        public int? DelayTaskForSeconds { get; set; }
    }
}