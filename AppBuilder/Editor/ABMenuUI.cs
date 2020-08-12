using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System;
using Object = System.Object;

public sealed class ABMenuUI : EditorWindow
{
    public static readonly int WindowWidth = 400;
    public static readonly int WindowHeight = 300;
    private static bool initState = false;
    private static ABMenuUI assetSettingWindow;
    public static void Init()
    {
        if (initState) return;
        initState = true;
        assetSettingWindow = (ABMenuUI)EditorWindow.GetWindow(typeof(ABMenuUI));
        assetSettingWindow.titleContent = new GUIContent("资源打包系统V1.0");
        assetSettingWindow.position = new Rect((Screen.currentResolution.width - WindowWidth) / 2, (Screen.currentResolution.height - WindowHeight) / 2, WindowWidth, WindowHeight);
    }
    public static void ShowWindow()
    {
        Init();
        assetSettingWindow.Show();
    }
    void OnDisable()
    {
        //assetSettingWindow = null;
        //initState = false;
    }
    void Update()
    {
        //Init();
    }
    
    private bool updateServer = false;
    void OnGUI()
    {
        if (!initState) return;
        updateServer = GUILayout.Toggle(updateServer, "上传服务器");
        if (GUILayout.Button("制作基础AB版本（每一个大版本的基础版本）"))
        {
            //ABMenu.BuildAb(updateServer);
            //AbHelp.BackBuildSceneSetting();
            //ABMenu.SaveBulidVersionInfo();
        }
        if (GUILayout.Button("PatchAB包"))
        {
            //ABMenu.CreatePatch(updateServer);
        }
    }
}

