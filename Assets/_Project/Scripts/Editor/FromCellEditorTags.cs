#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FromCell.Editor
{
    static class FromCellEditorTags
    {
        public static void EnsureProjectTagsAndLayers()
        {
            EnsureTag("Player");
            EnsureTag("Ground");
            EnsureTag("Enemy");
            EnsureLayer("Player");
            EnsureLayer("Ground");
            EnsureLayer("Hazard");
            EnsureLayer("Trigger");
            EnsureLayer("Enemy");
        }

        static void EnsureTag(string tag)
        {
            if (UnityEditorInternal.InternalEditorUtility.tags.Contains(tag))
                return;

            var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
            var tagManager = new SerializedObject(asset);
            var tags = tagManager.FindProperty("tags");

            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
            tagManager.ApplyModifiedProperties();
            Debug.Log($"From Cell: added tag '{tag}'");
        }

        static void EnsureLayer(string layer)
        {
            if (LayerMask.NameToLayer(layer) >= 0)
                return;

            var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
            var tagManager = new SerializedObject(asset);
            var layers = tagManager.FindProperty("layers");

            for (int i = 8; i < layers.arraySize; i++)
            {
                if (!string.IsNullOrEmpty(layers.GetArrayElementAtIndex(i).stringValue))
                    continue;

                layers.GetArrayElementAtIndex(i).stringValue = layer;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"From Cell: added layer '{layer}' at index {i}");
                return;
            }

            Debug.LogWarning($"From Cell: no free layer slot for '{layer}'");
        }
    }
}
#endif
