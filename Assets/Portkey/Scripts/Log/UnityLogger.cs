using System;
using Portkey.Core;
using UnityEngine;

namespace Portkey.Log
{
    /// <summary>
    /// Singleton Unity logger class for logging.
    /// </summary>
    public class UnityLogger : IPortkeyLogger
    {
        protected const string Tag = "Portkey";
        protected static UnityLogger instance;

        public static UnityLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UnityLogger();
                }

                return instance;
            }
        }

        protected Logger logger;

        protected UnityLogger()
        {
            logger = new Logger(Debug.unityLogger);
#if PORTKEY_DEVELOPMENT
            logger.filterLogType = LogType.Log;
#else
            logger.filterLogType = LogType.Error;
#endif
            //TODO: Add a config to set the log level instead of using macro
        }

        /// <summary>
        /// To accomodate scriptable object config load order, we need to initialize the logger on AfterAssembliesLoaded.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        protected static void Initialize()
        {
            Debugger.Logger = Instance;
        }

        /// <summary>
        /// Logs a Unity message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(object message)
        {
            logger.Log(Tag, message);
        }
        
        /// <summary>
        /// Logs a Unity warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogWarning(object message)
        {
            logger.LogWarning(Tag, message);
        }

        /// <summary>
        /// Logs a Unity error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogError(object message)
        {
            logger.LogError(Tag, message);
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public void LogException(Exception exception)
        {
            logger.LogException(exception);
        }
    }
}