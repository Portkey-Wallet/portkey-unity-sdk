using System.Collections.Generic;
using System.IO;
using System.Linq;
using Portkey.Core;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Portkey.Editor
{
    public class BuildPostProcessor
    {
#if UNITY_IOS
        private static readonly string PORTKEYCONFIG_NAME = "PortkeyConfig";
        private static readonly string FACEID_USAGE_DESCRIPTION = "$(PRODUCT_NAME) wants to use TouchId or FaceID for authentication.";
        private static readonly string[] ResourceExts = { ".html" };
        
        private static readonly string UNITY_ASSET_PATH = "Assets/";
        private static readonly string XCODE_LIBRARIES_PATH = "Libraries/";
 
        internal static void CopyAndReplaceDirectory(string srcPath, string dstPath, string[] enableExts)
        {
            var isExist = false;
            
            if (Directory.Exists(dstPath))
            {
                isExist = true;
            }

            if (File.Exists(dstPath))
            {
                isExist = true;
            }

            if (!isExist)
            {
                Directory.CreateDirectory(dstPath);
            }

            foreach (var file in Directory.GetFiles(srcPath))
            {
                if (enableExts.Contains(Path.GetExtension(file)))
                {
                    File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
                }
            }
 
            foreach (var dir in Directory.GetDirectories(srcPath))
            {
                CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)), enableExts);
            }
        }
        
        public static void GetDirFileList(string dirPath, ref List<string> dirs, string[] enableExts, string subPathFrom="")
        {
            foreach (var path in Directory.GetFiles(dirPath))
            {
                if (enableExts.Contains(System.IO.Path.GetExtension(path)))
                {
                    if(subPathFrom != "")
                    {
                        dirs.Add(path.Substring(path.IndexOf(subPathFrom)));
                    }
                    else
                    {
                        dirs.Add(path);
                    }
                }
            }
 
            if (Directory.GetDirectories(dirPath).Length > 0)
            {
                foreach (var path in Directory.GetDirectories(dirPath))
                {
                    GetDirFileList(path, ref dirs, enableExts, subPathFrom);
                }
            }
        }
        
        [PostProcessBuild]
        public static void PostProcessing(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }
            
            UpdateXcodePlist(path);
            
            var projPath = PBXProject.GetPBXProjectPath(path);
            var proj = new PBXProject();
            
            UpdateXcodeBuildSettings(proj, projPath);
            UpdateXcodeBuildPhase(proj, path);
            
            proj.WriteToFile (projPath);
        }

        private static void UpdateXcodeBuildSettings(PBXProject proj, string projPath)
        {
            proj.ReadFromString(File.ReadAllText(projPath));
            var mainTargetGuid = proj.GetUnityMainTargetGuid();
            
            UpdateRunpathSearchPaths(proj, mainTargetGuid);
        }
        
        private static void UpdateXcodeBuildPhase(PBXProject proj, string path)
        {
            var projPath = PBXProject.GetPBXProjectPath(path);
            
            const string RECAPTCHA_FOLDER = "Plugins/iOS/IOSRecaptchaPlugin";
            var targetGuid = proj.GetUnityMainTargetGuid();
            var destPath = Path.Combine(path, XCODE_LIBRARIES_PATH + RECAPTCHA_FOLDER);
            
            var resources = new List<string>();
            CopyAndReplaceDirectory(UNITY_ASSET_PATH + RECAPTCHA_FOLDER, destPath, ResourceExts);
            GetDirFileList(destPath, ref resources, ResourceExts, XCODE_LIBRARIES_PATH + RECAPTCHA_FOLDER);
            
            foreach (var resource in resources)
            {
                var resourcesBuildPhase = proj.GetResourcesBuildPhaseByTarget(targetGuid);
                var resourcesFilesGuid = proj.AddFile(resource, resource, PBXSourceTree.Source);
                proj.AddFileToBuildSection(targetGuid, resourcesBuildPhase, resourcesFilesGuid);
            }
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
            AddQuerySchema(rootDict);

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static void AddQuerySchema(PlistElementDict rootDict)
        {
            var iosTransport = EditorHelper.GetIOSTransportConfig("IOSTransport");
            
            rootDict.CreateArray("LSApplicationQueriesSchemes").AddString(iosTransport.URLScheme);
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
#endif
    }
}