using System;
using Portkey.Core;
using UnityEditor;
using UnityEngine;

namespace Portkey.Editor
{
    /// <summary>
    /// Helper class to provide some unity specific functionality.
    /// </summary>
    public static class EditorHelper
    {
        /// <summary>
        /// Gets a ScriptableObject config from the project.
        /// </summary>
        /// <param name="name">Name of the config.</param>
        /// <returns>The config Portkey object.</returns>
        /// <exception cref="Exception">Throws if no matching config was found.</exception>
        public static PortkeyConfig GetPortkeyConfig(string name)
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(PortkeyConfig)} {name}");
            if (guids.Length == 0)
            {
                throw new Exception($"No {nameof(PortkeyConfig)} found!");
            }
            else if (guids.Length > 0)
            {
                Debug.LogWarning($"More than one {nameof(PortkeyConfig)} found, taking first one");
            }

            return (PortkeyConfig)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]),
                typeof(PortkeyConfig));
        }
    }
}