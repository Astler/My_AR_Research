using ExternalTools.ImagesLoader.Elements;
using UnityEditor;
using UnityEngine;

namespace ExternalTools.ImagesLoader.Editor
{
    [CustomEditor(typeof(WebImage))]
    public class WebImageEditor : UnityEditor.UI.ImageEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            WebImage image = (WebImage)target;

            using EditorGUI.ChangeCheckScope check = new();
            
            string urlField = EditorGUILayout.TextField("Url", image.GetUrl());

            if (!check.changed) return;
            
            Undo.RecordObject(target, "Changed web image url");
            image.SetImageUrl(urlField);
        }
    }
}