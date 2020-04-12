using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gemserk.UPMGitPusher.Editor
{
    public class PublishData
    {
        public string path;
        public string pathToJson;
        
        public Version version;
        public Version newVersion;
        
        public PackageData pacakge;
    }

    public class PackageData
    {
        public string name;
        public string version;
    }
    
    public static class PublishVersionMenuItem
    {
        private static bool dryRun => EditorPrefs.GetBool(GitPusherSettingsProvider.PreferenceKeyDryRun, false);
        
        [MenuItem("Assets/UPM Git Package/Publish Patch")]
        public static void PublishPatchVersion()
        {
            // TODO: show some message while everything is being executed....
            // maybe some progress...

            try
            {
                EditorUtility.DisplayProgressBar("Publish Patch", "Starting publishing patch...", 0.0f);
                var publishData = GetPackageData();
                EditorUtility.DisplayProgressBar("Publish Patch", "Git Sub Tree", 0.33f);
                PushSubTree(publishData);
                EditorUtility.DisplayProgressBar("Publish Patch", "Updating package.json", 0.66f);
                UpdatePackageVersion(publishData);
                EditorUtility.DisplayProgressBar("Publish Patch", "Git Commit", 1.0f);
                CommitChanges(publishData);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", e.Message, "ok");
                throw e;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void CommitChanges(PublishData publishData)
        {
            // TODO: get the default commit message from EditorPreferences 
            // TODO: flag to have or not this automatic step or not.
            
            Debug.Log("Committing version change.");
            
            var gitCommand = $"commit {publishData.pathToJson} -m \"Updated version from {publishData.version} to {publishData.newVersion}\"";
            
            if (!dryRun)
            {
                GitHelper.ExecuteCommand(gitCommand);
            }
            else
            {
                Debug.Log($"Executing: git {gitCommand}");
            }
        }

        private static void UpdatePackageVersion(PublishData publishData)
        {
            var version = publishData.version;
            
            publishData.newVersion = new Version(version.Major, version.Minor, version.Build + 1);
            Debug.Log($"Changing version from {version} to {publishData.newVersion}");

            var packageAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(publishData.pathToJson);
            var newText = packageAsset.text.Replace(
                $"\"{publishData.pacakge.version}\"", 
                $"\"{publishData.newVersion}\"");

            if (!dryRun)
            {
                File.WriteAllText(publishData.pathToJson, newText);
            
                EditorUtility.SetDirty(packageAsset);
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log($"Writing new JSON:\n{newText}");
            }
        }

        public static void PushSubTree(PublishData publishData)
        {
            const string origin = "origin";

            var gitCommand =
                $"subtree push --prefix {publishData.path} {origin} {publishData.pacakge.name}-{publishData.pacakge.version}";
            
            if (!dryRun)
            {
                GitHelper.ExecuteCommand(gitCommand);
            }
            else
            {
                Debug.Log($"Executing: git {gitCommand}");
            }
        }
        
        private static PublishData GetPackageData()
        {
            // warning, there could be multiple packages, even package.txt
            
            var guids = AssetDatabase.FindAssets("t:TextAsset package");
            if (guids.Length == 0)
            {
               throw new Exception("Failed to get package.json");
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var packageTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            var packageData = JsonUtility.FromJson<PackageData>(packageTextAsset.text);
            
            // Testing we read everything without problem.
            Debug.Log(JsonUtility.ToJson(packageData, true));

            return new PublishData
            {
                pacakge = packageData,
                path = path.Replace("package.json", ""),
                pathToJson = path,
                version = Version.Parse(packageData.version)
            };
        }

    }
}
