using System;
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

    public class AuthorData
    {
        public string name;
        public string email;
        public string url;
    }
    
    public class PackageData
    {
        public string name;
        public string version;
        public string displayName;
        public string description;
        public string unity;
        public string unityRelease;
        // TODO: dependencies...
        public string[] keywords;
        public AuthorData author;
    }
    
    public static class PublishVersionMenuItem
    {
        [MenuItem("Assets/UPM Git Package/Publish Patch")]
        public static void PublishPatchVersion()
        {
            try
            {
                var publishData = GetPackageData();
                PushSubTree(publishData);
                UpdatePackageVersion(publishData);
                CommitChanges(publishData);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", e.Message, "ok");
                throw e;
            }
        }

        private static void CommitChanges(PublishData publishData)
        {
            Debug.Log("Commit changes... (not implemented yet)");
            
            // TODO: get the default commit message from EditorPreferences 
            
            var gitCommand = $"commit {publishData.pathToJson} -m 'Updated version from {publishData.version} to {publishData.newVersion}'";
            
            Debug.Log($"Executing: git {gitCommand}");
        }

        private static void UpdatePackageVersion(PublishData publishData)
        {
            Debug.Log("Updating package version... (not implemented yet)");

            var version = publishData.version;
            
            var newVersion = new Version(version.Major, version.Minor, version.Build + 1);
            Debug.Log($"Changing version to {newVersion}");

            publishData.pacakge.version = newVersion.ToString();
            publishData.newVersion = newVersion;
            
            // TODO: write new pacakge.json file...
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
        
        public static void PushSubTree(PublishData publishData)
        {
            // TODO: configure origin in editor preferences
            const string origin = "origin";
            Debug.Log($"git subtree push --prefix {publishData.path} {origin} {publishData.pacakge.name}-{publishData.pacakge.version}");
        }
    }
}
