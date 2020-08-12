using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;

public partial class ABMenu
{

    /// <summary>
    /// 拷贝场景文件到打包目录
    /// </summary>
    /// <returns></returns>
    public static List<FileInfo> CopyScene()
    {
        List<FileInfo> sceneList = new List<FileInfo>();
        
        DirectoryInfo sceneDirectory = new DirectoryInfo(string.Concat(System.Environment.CurrentDirectory, Path.DirectorySeparatorChar, AbHelp.SceneFolder));
        if (!sceneDirectory.Exists)
        {
            sceneDirectory.Create();
        }
        List<string> sourcePaths = new List<string>();
        List<string> targetPaths = new List<string>();
        
        EditorBuildSettingsScene[] lastScenes = AbHelp.GetEditorBuildSettingsScene(sourcePaths, targetPaths);

        ABBuildCache.Open("SceneCache.x");
        for(int i = 0; i < sourcePaths.Count; i++)
        {
            copyScene(sourcePaths[i], targetPaths[i], sceneList);
        }
        ABBuildCache.Close();
        EditorBuildSettings.scenes = lastScenes;
        return sceneList;
    }

    /// <summary>
    /// 拷贝场景
    /// </summary>
    /// <param name="fromAssetPath"></param>
    /// <param name="toAssetPath"></param>
    /// <param name="sceneList"></param>
    private static void copyScene(string fromAssetPath, string toAssetPath, List<FileInfo> sceneList)
    {
        //为了加速，判断新老文件sha1
        
        string targetPathTmp = string.Concat(System.Environment.CurrentDirectory
            , Path.DirectorySeparatorChar
            , toAssetPath);
            
            
        string sourcePathTmp = string.Concat(System.Environment.CurrentDirectory
            , Path.DirectorySeparatorChar
            , fromAssetPath);
        
        string sha1 = null;
        if (ABBuildCache.ExistsBegin(sourcePathTmp, ref sha1))
        {
            FileInfo sceneFile = new FileInfo(targetPathTmp);
            if (sceneFile.Exists)
            {
                sceneFile.Attributes = FileAttributes.Normal;
                sceneFile.Delete();
            }

            File.Copy(sourcePathTmp, targetPathTmp);
            //File.Copy(string.Concat(sourcePathTmp,  AbHelp.MetaFileExt), string.Concat(targetPathTmp,  AbHelp.MetaFileExt));
            AssetDatabase.ImportAsset(toAssetPath);

            //case 2:52秒
            //AssetDatabase.CopyAsset(sourcePaths[i], targetPaths[i]);
        }
        ABBuildCache.ExistsEnd(sourcePathTmp, sha1);
        
        //为了加速，不删除
        //FileInfo sceneFile = new FileInfo(toAssetPath);
        //sceneFile.Attributes = FileAttributes.Normal;
        //sceneList.Add(sceneFile);
    }
    
    /// <summary>
    /// 删除场景
    /// </summary>
    /// <param name="list"></param>
    public static void DeleteScene(List<FileInfo> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            FileInfo tmpFile = list[i];
            if (tmpFile.Exists)
            {
                FileInfo metaFile = new FileInfo(string.Concat(tmpFile.FullName, AbHelp.FileExt[0]));
                if (metaFile.Exists)
                {
                    metaFile.Attributes = FileAttributes.Normal;
                    metaFile.Delete();
                }
                tmpFile.Delete();
            }
            tmpFile = null;
        }
        list.Clear();
    }
}

