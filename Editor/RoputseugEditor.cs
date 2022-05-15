using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Roputseug
{
    [CustomEditor(typeof(RoputseugDictionary))]
    public class RoputseugEditor : Editor
    {

        string output;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            RoputseugDictionary currTarget = (RoputseugDictionary)target;

            currTarget.inputText = EditorGUILayout.TextArea(currTarget.inputText);

            if (currTarget.inputText == string.Empty)
                return;
           
            if (GUILayout.Button("Separate Syllables"))
            {
                output = RoputseugDictionary.Translate(currTarget.inputText);
            }

            EditorGUILayout.TextArea(output);
        }
    }
}