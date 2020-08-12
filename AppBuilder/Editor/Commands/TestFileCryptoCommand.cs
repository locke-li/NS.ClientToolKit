///***************************************************************

// *  类名称：        TestFileCryptoCommand

// *  描述：				

// *  作者：          Chico(wuyuanbing)

// *  创建时间：      2020/4/23 21:08:07

// *  最后修改人：

// *  版权所有 （C）:   diandiangames

//***************************************************************/


//using System;
//using System.Text;
//using CenturyGame.AppUpdaterLib.Runtime;
//using UnityEditor;
//using UnityEngine;
//using File = CenturyGame.Core.IO.File;
//using StdFile = System.IO.File;

//namespace CenturyGame.AppBuilder.Editor.Commands
//{
//    public static class TestFileCryptoCommand
//    {


//        //[MenuItem("Commands/Cryptography/Print app config")]
//        static void PrintAppConfig()
//        {
//            string path = Application.dataPath + "/Resources/AppConfig.txt";
//            if (!StdFile.Exists(path))
//            {
//                Debug.LogError("The app config file is not exist!");
//                return;
//            }

//            for (int i = 0; i < 100; i++)
//            {
//                string configText = File.ReadAllText(path, new UTF8Encoding(false, true));

//                AppConfig config = JsonUtility.FromJson<AppConfig>(configText);
//                var version = config.appVersion;
//                StringBuilder sb = new StringBuilder();
//                sb.AppendLine($"GameVersion : {version.Major}.{version.Minor}.{version.Patch}{version.VersionSuffix}{version.BuildMetadata}");
//                Debug.Log(sb.ToString());
//            }

           
//        }

//        //[MenuItem("Commands/Cryptography/Create app config")]
//        static void CreateAppConfig()
//        {
//            AppConfig config = new AppConfig();
//            config.appVersion = new AppVersion();

//            var appVersion = config.appVersion;
//            appVersion.Major = "1";
//            appVersion.Minor = "1";
//            appVersion.Patch = "1";
//            string buildMetaData = $"-[{Application.unityVersion}-{DateTime.Now:yyyyMMddHHmmss}]";
//            appVersion.BuildMetadata = buildMetaData;

//            var jsonStr = JsonUtility.ToJson(config, true);
            
//            string path = Application.dataPath + "/Resources/AppConfig.txt";

//            File.WriteAllText(path, jsonStr, encoding: new UTF8Encoding(false, true));
//            AssetDatabase.Refresh();

//            Debug.Log("Write default appConfig completed!");
//        }




//    }
//}
