using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Portkey.Core;
using UnityEditor;
using UnityEngine;

[assembly: InternalsVisibleTo("EditorMode"),
           InternalsVisibleTo("PlayMode")]
namespace Portkey.Test
{
    /// <summary>
    /// Utilities used for Unity unit tests.
    /// </summary>
    internal static class UnityTestUtilities
    {
        /// <summary>
        /// Use this method to run async methods in Unity unit tests.
        /// This method creates a new thread to run the async method, thereby allowing the unit test to run without blocking.
        /// This method should not be called anywhere else.
        /// </summary>
        /// <param name="asyncFunc">The async function to be called.</param>
        /// <typeparam name="T">The return type.</typeparam>
        /// <returns>The value with type T.</returns>
        public static T RunAsyncMethodToSync<T>(Func<Task<T>> asyncFunc)
        {
            return Task.Run(async () => await asyncFunc()).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Use this method to run async methods in Unity unit tests.
        /// This method creates a new thread to run the async method, thereby allowing the unit test to run without blocking.
        /// This method should not be called anywhere else.
        /// </summary>
        /// <param name="asyncFunc">The async function to be called.</param>
        public static void RunAsyncMethodToSync(Func<Task> asyncFunc)
        {
            Task.Run(async () => await asyncFunc()).GetAwaiter().GetResult();
        }
        
        public static PortkeyConfig GetPortkeyConfig(string name)
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(PortkeyConfig)} {name}");
            switch (guids.Length)
            {
                case 0:
                    Debug.LogError($"No {nameof(PortkeyConfig)} found!");
                    break;
                case > 0:
                    Debug.LogWarning($"More than one {nameof(PortkeyConfig)} found, taking first one");
                    break;
            }

            return (PortkeyConfig)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(PortkeyConfig));
        }
    }
}
