using System;
using System.Collections.Generic;
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
        
        public PackageData package;
    }

    public class PackageData
    {
        public string name;
        public string version;
    }
    
    public static class PublishVersionMenuItem
    {
        private const string PackageFileName = "package.json";
        
        [MenuItem("Assets/UPM Git Package/Publish Patch")]
        public static void PublishPatchVersion()
        {
            IEnumerable<PublishData> publishDataList = new List<PublishData>();
            
            if (Selection.activeObject is TextAsset textAsset)
            {
                var path = AssetDatabase.GetAssetPath(textAsset);
                if (path.EndsWith(PackageFileName))
                {
                    Debug.Log($"Exporting package from selection");
                    publishDataList = new List<PublishData>
                    {
                        GetPublishData(path, textAsset)
                    };
                }
            } else if (Selection.activeObject is PublishDataAsset publishDataAsset)
            {
                if (publishDataAsset.disabled)
                {
                    return;
                }

                publishDataList = publishDataAsset.packageFiles
                    .Select(p => GetPublishData(AssetDatabase.GetAssetPath(p), p)).ToList();
            }
            else
            {
                if (Preferences.AutoSearchPackages)
                {
                    // first search fo publish data assets and then export?
                    Debug.Log($"Exporting packages from all package.json using auto search");
                    publishDataList = GetAllPackagesPublishData();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error",
                        "Automatic package search is disabled, a package.json or a PackageDataAsset must be selected in order to export.", "OK");
                    return;
                }
            }
            
            PublishPatchVersion(publishDataList);
        }
        
        private static void PublishPatchVersion(IEnumerable<PublishData> publishDataList)
        {
            foreach (var publishData in publishDataList)  
            {
                Debug.Log($"Exporting Package {publishData.package.name}-{publishData.package.version}");
                try
                {
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
        }

        private static void CommitChanges(PublishData publishData)
        {
            if (!Preferences.automaticCommit)
                return;
            
            Debug.Log("Committing version change.");

            var commitMessage = Preferences.commitMessage;
            
            commitMessage = commitMessage.Replace("{PREVIOUS_VERSION}", publishData.version.ToString());
            commitMessage = commitMessage.Replace("{NEW_VERSION}", publishData.newVersion.ToString());
            
            var gitCommand = $"commit {publishData.pathToJson} -m \"{commitMessage}\"";
            
            Debug.Log($"Executing: git {gitCommand}");
            
            if (!Preferences.dryRun)
            {
                GitHelper.ExecuteCommand(gitCommand, GitHelper.Options.Default);
                GitHelper.ExecuteCommand($"tag master-{publishData.version}", GitHelper.Options.Default);
            }
        }

        private static void UpdatePackageVersion(PublishData publishData)
        {
            var version = publishData.version;
            
            publishData.newVersion = new Version(version.Major, version.Minor, version.Build + 1);
            Debug.Log($"Changing version from {version} to {publishData.newVersion}");

            var packageAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(publishData.pathToJson);
            var newText = packageAsset.text.Replace(
                $"\"{publishData.package.version}\"", 
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

            var branchName = publishData.package.name;

            try
            {
                GitHelper.ExecuteCommand($"branch -d {branchName}", new GitHelper.Options
                {
                    dryRun = Preferences.dryRun,
                    redirectOutput = false
                });
            }
            catch
            {
                Debug.Log($"Failed to delete previous branch {branchName}");
            }
            
            GitHelper.ExecuteCommand($"subtree split -P {publishData.path} -b {branchName}", new GitHelper.Options
            {
                dryRun = Preferences.dryRun,
                redirectOutput = false
            });
            
            var commitNumber = GitHelper.ExecuteCommand($"rev-parse {branchName}", new GitHelper.Options
            {
                dryRun = Preferences.dryRun,
                redirectOutput = true
            });

            GitHelper.ExecuteCommand($"tag {publishData.package.version} {commitNumber.Trim()}", new GitHelper.Options
            {
                dryRun = Preferences.dryRun,
                redirectOutput = false
            });

            GitHelper.ExecuteCommand($"push --tags -f -u {origin} {branchName}", new GitHelper.Options
            {
                dryRun = Preferences.dryRun,
                redirectOutput = false
            });
        }

        public static IEnumerable<PublishData> GetAllPackagesPublishData()
        {
            // warning, there could be multiple packages, even package.txt
            
            var guids = AssetDatabase.FindAssets("t:TextAsset package", new []
            {
                "Assets"
            });
            
            if (guids.Length == 0)
            {
               throw new Exception("Failed to get package.json");
            }

            var paths = guids
                .Where(g => AssetDatabase.GUIDToAssetPath(g).EndsWith(PackageFileName))
                .Select(AssetDatabase.GUIDToAssetPath).ToList();

            var publishDataList = new List<PublishData>();

            foreach (var path in paths)
            {
                var packageTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                publishDataList.Add(GetPublishData(path, packageTextAsset));
            }

            return publishDataList;
        }

        private static PublishData GetPublishData(string path, TextAsset textAsset)
        {
            var packageData = JsonUtility.FromJson<PackageData>(textAsset.text);
            return new PublishData
            {
                package = packageData,
                path = path.Replace(PackageFileName, ""),
                pathToJson = path,
                version = Version.Parse(packageData.version)
            };
        }

    }
}
