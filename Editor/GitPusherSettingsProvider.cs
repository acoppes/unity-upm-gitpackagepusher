using System.Collections.Generic;
using UnityEditor;

namespace Gemserk.UPMGitPusher.Editor
{
    public static class GitPusherSettingsProvider
    {
     
        private static void TogglePreference(string label, string preference, bool defaultValue)
        {
            var previousValue = EditorPrefs.GetBool(preference, defaultValue);
            var newValue = EditorGUILayout.Toggle(label, previousValue);
            EditorPrefs.SetBool(preference, newValue);
        }
        
        private static void StringPreference(string label, string preference, string defaultValue)
        {
            var previousValue = EditorPrefs.GetString(preference, defaultValue);
            var newValue = EditorGUILayout.TextField(label, previousValue);
            EditorPrefs.SetString(preference, newValue);
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/UPM Git Pusher", SettingsScope.User)
            {
                label = "UPM Git Pusher",
                guiHandler = (searchContext) =>
                {
                    TogglePreference("Dry run", Preferences.PreferenceKeyDryRun, Preferences.dryRun);
                    TogglePreference("Automatically commit new version update", Preferences.PreferenceKeyAutoCommit, 
                        Preferences.automaticCommit);
                    StringPreference("Commit Message", Preferences.PreferenceKeyCommitMessage, 
                        Preferences.commitMessage);
                    TogglePreference("Automatic package search", Preferences.PreferenceKeyAutoSearchPackages, 
                        Preferences.AutoSearchPackages);
                },
                keywords = new HashSet<string>(new[] { "Git", "UPM" })
            };

            return provider;
        }
    }
}