using System;
using UnityEditor;
using UnityEngine;

namespace Gemserk.UPMGitPusher.Editor
{
    public class PackageData
    {
        public string path;
        
        public string name;
        public string version;
    }
    
    public static class PublishVersionMenuItem
    {
        [MenuItem("Assets/Git/Publish UPM Package")]
        public static void PublishVersion()
        {
            try
            {
                var packageData = GetPackageData();
                PushSubTree(packageData);
                UpdatePackageVersion(packageData);
                CommitChanges(packageData);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", e.Message, "ok");
                throw e;
            }
        }

        private static void CommitChanges(PackageData packageData)
        {
            Debug.Log("Commit changes... (not implemented yet)");
        }

        private static void UpdatePackageVersion(PackageData packageData)
        {
            Debug.Log("Updating package version... (not implemented yet)");
        }

        private static PackageData GetPackageData()
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

            packageData.path = path.Replace("package.json", "");

            return packageData;
        }
        
        public static void PushSubTree(PackageData packageData)
        {
            // TODO: configure origin in editor preferences
            const string origin = "origin";
            Debug.Log($"git subtree push --prefix {packageData.path} {origin} {packageData.name}-{packageData.version}");
        }
    }
}
