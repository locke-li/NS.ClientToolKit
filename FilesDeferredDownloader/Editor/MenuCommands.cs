
/***************************************************************

 *  类名称：        MenuCommands

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2021/1/4 10:28:01

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System.IO;
using System.Text;
using CenturyGame.FilesDeferredDownloader.Runtime.Configs;
using UnityEditor;
using UnityEngine;

namespace CenturyGame.FilesDeferredDownloader.Editor
{
    public static class MenuCommands
    {
        private const string CreateDeferredDownloadConfig = "CenturyGame/FilesDeferredDownloader/Create deferred download config";

        [MenuItem(CreateDeferredDownloadConfig)]
        static void MakeDefaultConfig()
        {
            //string targetFolderPath = Application.dataPath + "/CenturyGamePackageRes/FilesDeferredDownloader/Resources";
            //if (!Directory.Exists(targetFolderPath))
            //{
            //    Directory.CreateDirectory(targetFolderPath);

            //    AssetDatabase.Refresh();
            //}

            //var path = targetFolderPath + "/deferreddownload.txt";

            //if (File.Exists(path))
            //{
            //    Debug.LogWarning("The appupdater config that you want to make is exist in current project!");
            //    return;
            //}
            //DeferredDownloadConfig config = new DeferredDownloadConfig();
            //string jsonContents = JsonUtility.ToJson(config, true);
            //File.WriteAllText(path, jsonContents, new UTF8Encoding(false, true));

            //Debug.Log("Write default deferred download default config completd!");

            //AssetDatabase.Refresh();

        }
    }
}
