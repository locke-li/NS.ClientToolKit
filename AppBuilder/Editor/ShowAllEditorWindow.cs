using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class ShowAllEditorWindow : EditorWindow
{
    static List<Type> windowTypes = new List<Type>();
    static List<Type> timelineTypes = new List<Type>();
    //[MenuItem("Tools/ShowAllWindow")]
    static void ShowWindow()
    {
        current = EditorWindow.GetWindow<ShowAllEditorWindow>("AllEditorWindow");
        current.position = new Rect(100, 100, 900, 800);
        Type[] types = typeof(EditorWindow).Assembly.GetTypes();
        windowTypes = GetEditorWindow(types);
        Debug.Log(typeof(EditorWindow).Assembly.FullName);
        hWindos.position = current.position;
        hWindos.Show();
        //hWindos.position

        hWindos.ShowPopup();
        //这里是为了找TimeLine窗口
//        Type[] timeTypes = typeof(TimelineEditor).Assembly.GetTypes();
//        timelineTypes = GetEditorWindow(timeTypes);
//        Debug.Log(typeof(TimelineEditor).Assembly.FullName);

    }
    private static EditorWindow hWindos = null;
    private static ShowAllEditorWindow current = null;
    /// <summary>
    /// 这里是循环遍历当前类型的基类，判定是否为EditorWindow，最多找1000次
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    private static List<Type> GetEditorWindow(Type[] types)
    {
        List<Type> editorType = new List<Type>();
        foreach (var item in types)
        {
            int i = 1;
            Type temp = item;
            while(temp != null && i < 1000)
            {
                if(temp.BaseType == typeof(EditorWindow))
                {
                    editorType.Add(item);
                    Debug.Log(temp.Name);
                    if (temp.Name.Contains("SceneHierarchyWindow"))
                    {
                        Debug.Log(temp.ToString());
                        hWindos = EditorWindow.GetWindow(temp) as EditorWindow;
                    }
                    break;
                }

                temp = temp.BaseType;
                i++;
            }
        }

        editorType.Sort((a, b) =>
        {
            return string.Compare(a.Name, b.Name);
        });
        return editorType;
    }


    Vector2 scroll = Vector2.zero;
    private void OnGUI()
    {
        if (hWindos != null)
        {
            hWindos.position = current.position;
            
        }

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("打开所有窗口"))
        {
            foreach (var item in windowTypes)
            {
                try
                {
                    EditorWindow window = EditorWindow.GetWindow(item);
                    window.Show();
                }
                catch
                {

                }
            }
        }

        if (GUILayout.Button("关闭所有窗口"))
        {
            foreach (var item in windowTypes)
            {
                EditorWindow window = EditorWindow.GetWindow(item);
                window.Close();
            }
        }

        if (GUILayout.Button("TimeLine窗口"))
        {
            foreach (var item in timelineTypes)
            {
                try
                {
                    EditorWindow ww = EditorWindow.GetWindow(item);
                    if (ww != null)
                    {
                        ww.Show();
                        Debug.Log(item.FullName);
                    }
                }
                catch
                {

                }
               
            }
        }
        scroll =  EditorGUILayout.BeginScrollView(scroll);
        foreach (var item in windowTypes)
        {
            if (GUILayout.Button(item.Name))
            {
                Debug.Log(item.Name);

                EditorWindow window =  EditorWindow.GetWindow(item);
                window.position = new Rect(100, 100, 500, 800);
                window.Show();
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginVertical();
    }

}
public class ExamplePopup : EditorWindow
{
    //[MenuItem("Window/Example Popup %e")]
    static void InitPopup()
    {
        ExamplePopup popup = EditorWindow.CreateInstance<ExamplePopup>();
        Rect rect = new Rect(100, 100, 250, 250);
        popup.position = rect;
        popup.ShowAsDropDown(rect, new Vector2(250, 250));
    }
 
    private void OnGUI()
    {
        GUI.Label(new Rect(75, 75, 200, 100), "Hello World");
    }
}
