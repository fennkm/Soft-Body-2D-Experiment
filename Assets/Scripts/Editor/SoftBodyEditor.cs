using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SoftBody))]
public class SoftBodyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SoftBody script = (SoftBody)target;
        SerializedObject obj = new SerializedObject(script);

        if(GUILayout.Button("Build Object"))
        {
            obj.FindProperty("width").intValue = script.m_width;
            obj.FindProperty("height").intValue = script.m_height;
            obj.FindProperty("springLength").floatValue = script.m_springLength;
            obj.FindProperty("springConst").floatValue = script.m_springConst;
            obj.FindProperty("dampingFactor").floatValue = script.m_dampingFactor;
            obj.FindProperty("displayStructure").boolValue = script.m_displayStructure;
            
            script.buildObject();

            SerializedProperty pointArr = obj.FindProperty("points");
            SerializedProperty springArr = obj.FindProperty("springs");

            pointArr.arraySize = script.points.Length;
            springArr.arraySize = script.springs.Length;

            for (int i = 0; i < script.points.Length; i++)
                pointArr.GetArrayElementAtIndex(i).objectReferenceValue = script.points[i];

            for (int i = 0; i < script.springs.Length; i++)
                springArr.GetArrayElementAtIndex(i).objectReferenceValue = script.springs[i];
        
            Debug.Log((obj.ApplyModifiedProperties() ? "Object updated" : "Object not updated"));
        }
    }
}
