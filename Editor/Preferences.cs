using UnityEditor;

namespace Gemserk.UPMGitPusher.Editor
{
    public static class Preferences
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

    }
}