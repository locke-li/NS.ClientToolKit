//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//[CustomEditor(typeof(ABManager))]
//public class MatLevelManagerUI : Editor
//{
//    SerializedProperty propertyCheckType = null;
//    SerializedProperty propertyMatCount = null;
//    SerializedObject sObj = null;

//    private void OnEnable()
//    {
//        sObj = serializedObject;
//        propertyCheckType = sObj.FindProperty("CheckType");
//        propertyMatCount = sObj.FindProperty("MatCount");
//    }

//    private void OnDisable()
//    {
//        propertyCheckType = null;
//        sObj = null;
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        M5EditorGUIUtility.Separator();
//        EditorGUILayout.LabelField("M5 Mat Level Manager V1.0");
//        M5EditorGUIUtility.Separator();
//        sObj.Update();
//        EditorGUILayout.PropertyField(propertyCheckType);
//        EditorGUILayout.PropertyField(propertyMatCount);
//        if (GUI.changed)
//        {
//            EditorUtility.SetDirty(target);
//        }
//        sObj.ApplyModifiedProperties();
//    }
//}
