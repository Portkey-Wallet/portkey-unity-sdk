using Portkey.Core;
using UnityEditor;
using UnityEngine;

namespace Portkey.Utilities
{
    public class PortkeyUtilities
    {
        public static T GetConfig<T>(string name) where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets($"t: ScriptableObject {name}");
            if (guids.Length == 0)
            {
                throw new System.Exception($"No config with name: {name} found!");
            }
            else if (guids.Length > 0)
            {
                Debugger.LogWarning($"More than one config with name: {name} found, taking first one");
            }

            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}