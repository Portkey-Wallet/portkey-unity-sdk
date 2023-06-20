using UnityEngine;
using System.Diagnostics;

namespace Portkey.Core
{
    /// <summary>
    /// Portkey specific debugger class for logging only if PORTKEY_DEVELOPMENT macro is set.
    /// This prevents the Portkey DID SDK to log in production builds for our third party partners.
    /// condition argument in each method is optional and defaults to true, this is for ease of use such that users do not need to wrap method calls.
    /// </summary>
    public static class Debugger
    {
        [Conditional("PORTKEY_DEVELOPMENT")]
        public static void ClearDeveloperConsole()
        {
            UnityEngine.Debug.ClearDeveloperConsole();
        }

        [Conditional("PORTKEY_DEVELOPMENT")]
        public static void Log(object message, bool condition = true)
        {
            if (condition)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        [Conditional("PORTKEY_DEVELOPMENT")]
        public static void Log(object message, Object context, bool condition = true)
        {
            if (condition)
            {
                UnityEngine.Debug.Log(message, context);
            }
        }
        
        [Conditional("PORTKEY_DEVELOPMENT")]
        public static void LogError(object message, bool condition = true)
        {
            if (condition)
            {
                UnityEngine.Debug.LogError(message);
            }
        }

        [Conditional("PORTKEY_DEVELOPMENT")]
        public static void LogError(object message, Object context, bool condition = true)
        {
            if (condition)
            {
                UnityEngine.Debug.LogError(message, context);
            }
        }

        [Conditional("PORTKEY_DEVELOPMENT")]
        public static void LogException(System.Exception exception, bool condition = true)
        {
            if (condition)
            {
                UnityEngine.Debug.LogException(exception);
            }
        }

        [Conditional("PORTKEY_DEVELOPMENT")]
        public static void LogException(System.Exception exception, Object context, bool condition = true)
        {
            if (condition)
            {
                UnityEngine.Debug.LogException(exception, context);
            }
        }

        [Conditional("PORTKEY_DEVELOPMENT")]
        public static void LogWarning(object message, bool condition = true)
        {
            if (condition)
            {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        [Conditional("PORTKEY_DEVELOPMENT")]
        public static void LogWarning(object message, Object context, bool condition = true)
        {
            if (condition)
            {
                UnityEngine.Debug.LogWarning(message, context);
            }
        }
    }
}