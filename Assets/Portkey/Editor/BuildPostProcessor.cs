using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Portkey.Editor
{
    public class BuildPostProcessor
    {
        private static readonly string PORTKEYCONFIG_NAME = "PortkeyConfig";

        [PostProcessBuild]
        public static void UpdateXcodePlist(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }
            
            UpdateBuildSettingsForAppleLogin(buildTarget, path);

            var plistPath = path + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            var rootDict = plist.root;
            var portkeyConfig = EditorHelper.GetPortkeyConfig(PORTKEYCONFIG_NAME);
            if (portkeyConfig == null)
            {
                return;
            }

            Debug.Log("Adding required fields to Info.plist...");

            rootDict.SetString("GIDClientID", portkeyConfig.GoogleIOSClientId);

            const string urlKey = "CFBundleURLTypes";
            var urlElement = rootDict.values[urlKey] ?? rootDict.CreateArray(urlKey);
            var urlArray = urlElement.AsArray();
            if (urlArray.values.Count == 0)
            {
                urlArray.AddDict();
            }

            var urlDict = urlArray.values[0].AsDict();
            const string schemesKey = "CFBundleURLSchemes";
            var schemesElement = urlDict.values[schemesKey] ?? urlDict.CreateArray(schemesKey);
            var schemesArray = schemesElement.AsArray();
            schemesArray.AddString(portkeyConfig.GoogleIOSDotReverseClientId);

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static void UpdateBuildSettingsForAppleLogin(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            var projectPath = PBXProject.GetPBXProjectPath(path);
        
            // Adds entitlement depending on the Unity version used
            var project = new PBXProject();
            project.ReadFromString(System.IO.File.ReadAllText(projectPath));
            var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
            manager.AddSignInWithApple();
            manager.WriteToFile();
        }

    }
}