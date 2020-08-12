using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System;

public partial class ABMenu
{
    private static readonly string dataFolderName = "Assets/csv/";
    private static readonly string dataExt = ".csv";
    //private static readonly string assetExt = ".asset";
    public static List<FileInfo> CopyData()
    {
        List<FileInfo> dataList = new List<FileInfo>();
        DirectoryInfo dataFolder = new DirectoryInfo(string.Concat(System.Environment.CurrentDirectory, Path.DirectorySeparatorChar, dataFolderName));
        string currentPath = string.Concat(System.Environment.CurrentDirectory, Path.DirectorySeparatorChar);
        if (dataFolder.Exists)
        {
            FileInfo[] dataFile = dataFolder.GetFiles("*", SearchOption.AllDirectories);
            ABBuildCache.Open("TableCache.x");
            for(int i = 0; i < dataFile.Length; i++)
            {
                FileInfo tmpFile = dataFile[i];
                string sha1 = null;
                if (dataExt.CompareTo(tmpFile.Extension.ToLower()) == 0 && ABBuildCache.ExistsBegin(tmpFile.FullName, ref sha1))
                {
                    string itenName = tmpFile.FullName.Substring(currentPath.Length);
                    itenName = Path.GetFileNameWithoutExtension(itenName);

                    //string scriptableScriptName = ClientDataBaseManager.Instance.Config.GetScriptableScriptName(itenName, true);
                    //string scriptableAssetName = string.Concat(itenName, assetExt);
                    ////string scriptableAssetName = ClientDataBaseManager.Instance.Config.GetScriptableAssetName(itenName, true);
                    //if (ClientDataBase.ClientDataBaseParse.Instance.CreateScriptableAssets(scriptableScriptName, scriptableAssetName, false, dataFolderName))
                    //{
                    //    //Debug.LogFormat("Update Asset table (.csv) name :{0} index:{1}", itenName, i + 1);
                    //}
                    
                    //加速
                    //string newAssetName = string.Concat(dataFolder.FullName, scriptableAssetName);
                    //FileInfo tmp = new FileInfo(newAssetName);
                    //tmp.Attributes = FileAttributes.Normal;
                    //dataList.Add(tmp);
                }
                ABBuildCache.ExistsEnd(tmpFile.FullName, sha1);
            }
            ABBuildCache.Close();
        }
        return dataList;
    }
}
