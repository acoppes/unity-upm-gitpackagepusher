using System.Collections.Generic;
using UnityEditor;

namespace Gemserk.UPMGitPusher.Editor
{
    public static class GitPusherSettingsProvider
    {
        public const string PreferenceKeyDryRun = "UPMGitPusher.DryRun";
        public const string PreferenceKeyAutoCommit = "UPMGitPusher.AutoCommit";
        public const string PreferenceKeyCommitMessage = "UPMGitPusher.CommitMessage";
        
        // TODO: allow configure remote (default is origin)
        // TODO: optional configure path to package.json or reference to text asset itself

        public static bool dryRun => EditorPrefs.GetBool(PreferenceKeyDryRun, false);
        public static bool automaticCommit => EditorPrefs.GetBool(PreferenceKeyAutoCommit, true);

        public static string commitmMessage => EditorPrefs.GetString(PreferenceKeyCommitMessage, 
            "Updated version from {PREVIOUS_VERSION} to {NEW_VERSION}");
        
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
                    TogglePreference("Dry run", PreferenceKeyDryRun, GitPusherSettingsProvider.dryRun);
                    TogglePreference("Automatically commit new version update", PreferenceKeyAutoCommit, 
                        GitPusherSettingsProvider.automaticCommit);
                    StringPreference("Commit Message", PreferenceKeyCommitMessage, 
                        GitPusherSettingsProvider.commitmMessage);
                },
                keywords = new HashSet<string>(new[] { "Git", "UPM" })
            };

            return provider;
        }
    }
}