using UnityEditor;

namespace Gemserk.UPMGitPusher.Editor
{
    public static class Preferences
    {
        public const string PreferenceKeyDryRun = "UPMGitPusher.DryRun";
        public const string PreferenceKeyAutoCommit = "UPMGitPusher.AutoCommit";
        public const string PreferenceKeyCommitMessage = "UPMGitPusher.CommitMessage";
        public const string PreferenceKeyAutoSearchPackages = "UPMGitPusher.AutoSearchPackages";
        
        // TODO: allow configure remote (default is origin)
        // TODO: optional configure path to package.json or reference to text asset itself

        public static bool dryRun => EditorPrefs.GetBool(PreferenceKeyDryRun, false);
        public static bool automaticCommit => EditorPrefs.GetBool(PreferenceKeyAutoCommit, true);

        public static string commitMessage => EditorPrefs.GetString(PreferenceKeyCommitMessage, 
            "Updated version from {PREVIOUS_VERSION} to {NEW_VERSION}");
        
        public static bool AutoSearchPackages => EditorPrefs.GetBool(PreferenceKeyAutoSearchPackages, false);

    }
}