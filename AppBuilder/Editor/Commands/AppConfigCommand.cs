/***************************************************************

 *  类名称：        AppConfigCommand

 *  描述：		    与游戏版本相关的命令

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/23 11:05:52

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.AppUpdaterLib.Runtime.Manifests;
using UnityEditor;
using UnityEngine;
using YamlDotNet;

namespace CenturyGame.AppBuilder.Editor.Commands
{
    public static class AppConfigCommand
    {

        //[MenuItem("Commands/AssetBundleManager/Print app config")]
        static void PrintAppConfig()
        {
            string path = Application.dataPath + "/Resources/AppConfig.txt";
            if (!File.Exists(path))
            {
                Debug.LogError("The app config file is not exist!");
                return;
            }
            string configText = File.ReadAllText(path, encoding: new UTF8Encoding(false, true));

            AppConfig config = JsonUtility.FromJson<AppConfig>(configText);
            var version = config.appVersion;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"GameVersion : {version.Major}.{version.Minor}.{version.Patch}{version.VersionSuffix}{version.BuildMetadata}");
            Debug.Log(sb.ToString());
        }


        //[MenuItem("Commands/AssetBundleManager/Crate or update app config")]
        //static void CreateAppConfig()
        //{
        //    AppConfig config = new AppConfig();
        //    config.appVersion = new AppVersion();

        //    var appVersion = config.appVersion;
        //    appVersion.Major = "1";
        //    appVersion.Minor = "1";
        //    appVersion.Patch = "1";
        //    //EditorUserBuildSettings.development 
        //    string buildMetaData = $"-[{Application.unityVersion}-{DateTime.Now:yyyyMMddHHmmss}]";
        //    appVersion.BuildMetadata = buildMetaData;

        //    var jsonStr = JsonUtility.ToJson(config, true);
        //    Debug.Log(jsonStr);
        //    string path = Application.dataPath + "/Resources/AppConfig.txt";

        //    File.WriteAllText(path, jsonStr, encoding: new UTF8Encoding(false, true));

        //    AssetDatabase.Refresh();

        //    Debug.Log("Write default appConfig completed!");
        //}

        //[MenuItem("CenturyGame/AppBuild/Commands/AppConfig/Crate app info config")]
        static void CreateAppConfig()
        {
            //AppInfoManifest manifest = new AppInfoManifest();
            //manifest.version = "1.0.0";
            //manifest.dataResVersion = "f23r2rf3g34f3434rtftg3";
            //manifest.unityDataResVersion = "f2r323f3f23f23f";
            
            //var jsonStr = JsonUtility.ToJson(manifest, true);
            //Debug.Log(jsonStr);
            //string path = Application.streamingAssetsPath + $"/{AssetsFileSystem.AppInfoFileName}";

            //Debug.Log(path);

            //File.WriteAllText(path, jsonStr, encoding: new UTF8Encoding(false, true));

            //AssetDatabase.Refresh();

            //Debug.Log($"Write {AssetsFileSystem.AppInfoFileName} completed!");
        }
    }
}
