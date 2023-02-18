using UnityEditor;
using UnityEngine;

namespace Gemserk.UPMGitPusher.Editor
{
    [CustomEditor(typeof(PublishDataAsset), true)]
    public class PublishDataCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // var publishDataAsset = target as PublishDataAsset;
            
            if (GUILayout.Button("Publish"))
            {
                if (EditorUtility.DisplayDialog("Confirm", "This will run git to publish different branches with package code, continue?", 
                        "Yes", "Cancel"))
                {
                    PublishVersionMenuItem.PublishPatchVersion();
                }
            }
        }
    }
}