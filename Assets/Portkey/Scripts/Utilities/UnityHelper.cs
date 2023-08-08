using Portkey.Core;
using UnityEditor;
using UnityEngine;

namespace Portkey.Utilities
{
#if UNITY_EDITOR
    /// <summary>
    /// Helper class to provide some unity specific functionality.
    /// </summary>
    public static class UnityHelper
    {
        /// <summary>
        /// Gets a ScriptableObject config from the project.
        /// </summary>
        /// <param name="name">Name of the config.</param>
        /// <typeparam name="T">Object type inherited from ScriptableObject.</typeparam>
        /// <returns>The config object.</returns>
        /// <exception cref="Exception">Throws if no matching config was found.</exception>
        public static T GetConfig<T>(string name) where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets($"t: ScriptableObject {name}");
            switch (guids.Length)
            {
                case 0:
                    throw new System.Exception($"No config with name: {name} found!");
                case > 0:
                    Debugger.LogWarning($"More than one config with name: {name} found, taking first one");
                    break;
            }

            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
#endif
}