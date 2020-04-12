using System.Collections.Generic;
using UnityEditor;

namespace Gemserk.UPMGitPusher.Editor
{
    public static class GitPusherSettingsProvider
    {
        public const string PreferenceKeyDryRun = "UPMGitPusher.DryRun";
        
        // TODO: add force push option
        // TODO: allow configure remote (default is origin)
        
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            
            var provider = new SettingsProvider("Preferences/UPM Git Pusher", SettingsScope.User)
            {
                label = "UPM Git Pusher",
                guiHandler = (searchContext) =>
                {
                    var dryRun = EditorPrefs.GetBool(PreferenceKeyDryRun, false);
                    dryRun = EditorGUILayout.Toggle("Dry Run", dryRun);
                    EditorPrefs.SetBool(PreferenceKeyDryRun, dryRun);
                },
                keywords = new HashSet<string>(new[] { "Git", "UPM" })
            };

            return provider;
        }
    }
}