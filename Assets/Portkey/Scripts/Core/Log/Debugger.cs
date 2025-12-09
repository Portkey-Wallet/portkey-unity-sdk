using System;

namespace Portkey.Core
{

    /// <summary>
    /// Portkey debugger class for logging.
    /// </summary>
    public static class Debugger
    {
        public static IPortkeyLogger Logger;

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(object message)
        {
            Logger?.Log(message);
        }
        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(object message)
        {
            Logger?.LogWarning(message);
        }
        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogError(object message)
        {
            Logger?.LogError(message);
        }
        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public static void LogException(Exception exception)
        {
            Logger?.LogException(exception);
        }
    }
}
