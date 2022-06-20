using UnityEngine;
using UnityEditor;
using Heartfield.Roputseug;

namespace HeartfieldEditor.Roputseug
{
    [CustomEditor(typeof(RoputseugDictionary))]
    sealed class RoputseugEditor : Editor
    {
        string output;
        bool showResult;

        public override void OnInspectorGUI()
        {
            var currTarget = (RoputseugDictionary)target;

            EditorGUILayout.Separator();

            EditorGUI.BeginChangeCheck();
            currTarget.inputText = EditorGUILayout.TextArea(currTarget.inputText);
            if (EditorGUI.EndChangeCheck() && showResult)
            {
                output = string.Empty;
                showResult = false;
            }

            EditorGUILayout.Separator();

            EditorGUI.BeginDisabledGroup(currTarget.inputText.Length <= 2);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Separate Syllables"))
            {
                output = RoputseugDictionary.SeparateSyllables(currTarget.inputText);
                showResult = true;
            }

            if (GUILayout.Button("Translate"))
            {
                output = RoputseugDictionary.Translate(currTarget.inputText);
                showResult = true;
            }

            if (GUILayout.Button("Translate and Separate Syllables"))
            {
                output = RoputseugDictionary.TranslateAndSeparateSyllables(currTarget.inputText);
                showResult = true;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var style = new GUIStyle(EditorStyles.label)
            {
                richText = true
            };
            
            EditorGUILayout.LabelField("Output", $"<b>{output}</b>", style);
            EditorGUILayout.EndVertical();
        }
    }
}