using System.IO;
using Portkey.Core;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Portkey.Editor
{
    public class BuildPostProcessor
    {
        private static readonly string PORTKEYCONFIG_NAME = "PortkeyConfig";

        private static readonly string FACEID_USAGE_DESCRIPTION = "$(PRODUCT_NAME) wants to use TouchId or FaceID for authentication.";

#if UNITY_IOS
        [PostProcessBuild]
        public static void PostProcessing(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }
            
            UpdateBuildSettingsForAppleLogin(buildTarget, path);
            UpdateXcodePlist(path);
            UpdateXcodeBuildSettings(path);
        }

        private static void UpdateXcodeBuildSettings(string path)
        {
            var projPath = PBXProject.GetPBXProjectPath(path);
            
            var proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
            var mainTargetGuid = proj.GetUnityMainTargetGuid();
            
            UpdateRunpathSearchPaths(proj, mainTargetGuid);
            
            proj.WriteToFile (projPath);
        }

        private static void UpdateRunpathSearchPaths(PBXProject proj, string targetGuid)
        {
            proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS",
                "$(inherited) /usr/lib/swift @executable_path/Frameworks");
        }

        private static void UpdateXcodePlist(string path)
        {
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

            AddGoogleLoginRelatedInfo(rootDict, portkeyConfig);
            AddIOSBiometricInfo(rootDict);

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static void AddIOSBiometricInfo(PlistElementDict rootDict)
        {
            rootDict.SetString("NSFaceIDUsageDescription", FACEID_USAGE_DESCRIPTION);
        }

        private static void AddGoogleLoginRelatedInfo(PlistElementDict rootDict, PortkeyConfig portkeyConfig)
        {
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

#endif
    }
}