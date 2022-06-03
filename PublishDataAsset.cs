using System.Collections.Generic;
using UnityEngine;

namespace Gemserk.UPMGitPusher.Editor
{
    [CreateAssetMenu(menuName = "Assets/UPM Git Package/Create Publish Data")]
    public class PublishDataAsset : ScriptableObject
    {
        [Tooltip("Ignores this asset when auto searching assets to publish.")]
        public bool disabled;
        
        // Editor window to run publish button

        [Tooltip("A list of paths to package.json files to be published.")]
        public List<TextAsset> packageFiles;
    }
}