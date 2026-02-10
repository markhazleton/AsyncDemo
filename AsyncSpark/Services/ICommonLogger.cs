namespace AsyncSpark.Services
{
    /// <summary>
    /// Common logging interface for tracking events and exceptions.
    /// </summary>
    public interface ICommonLogger
    {
        /// <summary>
        /// Tracks an event with the specified message.
        /// </summary>
        /// <param name="message">The event message to track.</param>
        void TrackEvent(string message);
        
        /// <summary>
        /// Tracks an exception with an associated message.
        /// </summary>
        /// <param name="exception">The exception to track.</param>
        /// <param name="message">Additional context message for the exception.</param>
        void TrackException(Exception exception, string message);
    }
}

