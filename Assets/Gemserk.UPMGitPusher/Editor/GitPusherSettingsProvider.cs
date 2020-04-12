using System.Collections.Generic;
using UnityEditor;

namespace Gemserk.UPMGitPusher.Editor
{
    public static class GitPusherSettingsProvider
    {
        public const string PreferenceKeyDryRun = "UPMGitPusher.DryRun";
        public const string PreferenceKeyAutoCommit = "UPMGitPusher.AutoCommit";
        
        // TODO: allow configure remote (default is origin)
        // TODO: get the default commit message from EditorPreferences 

        public static bool dryRun => EditorPrefs.GetBool(GitPusherSettingsProvider.PreferenceKeyDryRun, false);
        public static bool automaticCommit => EditorPrefs.GetBool(GitPusherSettingsProvider.PreferenceKeyAutoCommit, true);

        private static void TogglePreference(string label, string preference, bool defaultValue)
        {
            var previousValue = EditorPrefs.GetBool(preference, defaultValue);
            var newValue = EditorGUILayout.Toggle(label, previousValue);
            EditorPrefs.SetBool(preference, newValue);
        }

        
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/UPM Git Pusher", SettingsScope.User)
            {
                label = "UPM Git Pusher",
                guiHandler = (searchContext) =>
                {
                    TogglePreference(PreferenceKeyDryRun, "Dry run", false);
                    TogglePreference(PreferenceKeyAutoCommit, "Automatically commit new version update", true);
                },
                keywords = new HashSet<string>(new[] { "Git", "UPM" })
            };

            return provider;
        }
    }
}