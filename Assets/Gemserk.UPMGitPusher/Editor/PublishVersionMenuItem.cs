﻿using System;
using System.IO;
using System.Linq;
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
        [MenuItem("Assets/UPM Git Package/Publish Patch")]
        public static void PublishPatchVersion()
        {
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
            if (!Preferences.automaticCommit)
                return;
            
            Debug.Log("Committing version change.");

            var commitMessage = Preferences.commitmMessage;
            
            commitMessage = commitMessage.Replace("{PREVIOUS_VERSION}", publishData.version.ToString());
            commitMessage = commitMessage.Replace("{NEW_VERSION}", publishData.newVersion.ToString());
            
            var gitCommand = $"commit {publishData.pathToJson} -m \"{commitMessage}\"";
            
            Debug.Log($"Executing: git {gitCommand}");
            if (!Preferences.dryRun)
            {
                GitHelper.ExecuteCommand(gitCommand);
                GitHelper.ExecuteCommand($"tag master-{publishData.version}");
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

            if (!Preferences.dryRun)
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

        private static void PushSubTree(PublishData publishData)
        {
            const string origin = "origin";

            var branchName = publishData.pacakge.name;

            try
            {
                Debug.Log(GitHelper.ExecuteCommand($"branch -d {branchName}", Preferences.dryRun));
            }
            catch
            {
                Debug.Log($"Failed to delete previous branch {branchName}");
            }
            
            var output = GitHelper.ExecuteCommand($"subtree split -P {publishData.path} -b {branchName}", Preferences.dryRun);
            Debug.Log(output);
            Debug.Log(GitHelper.ExecuteCommand($"tag {publishData.pacakge.version} {output.Trim()}", Preferences.dryRun));
            Debug.Log(GitHelper.ExecuteCommand($"push --tags -f -u {origin} {branchName}", Preferences.dryRun));
        }
        
        public static PublishData GetPackageData()
        {
            // warning, there could be multiple packages, even package.txt
            
            var guids = AssetDatabase.FindAssets("t:TextAsset package");
            if (guids.Length == 0)
            {
               throw new Exception("Failed to get package.json");
            }

            var paths = guids
                .Where(g => AssetDatabase.GUIDToAssetPath(g).EndsWith("package.json"))
                .Select(AssetDatabase.GUIDToAssetPath).ToList();

            var path = paths[0];
            var packageTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            var packageData = JsonUtility.FromJson<PackageData>(packageTextAsset.text);

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
