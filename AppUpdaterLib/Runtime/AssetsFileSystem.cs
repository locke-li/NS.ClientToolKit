/***************************************************************

 *  类名称：        Assets

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/6 18:32:00

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CenturyGame.ClientToolKit.AppSetting.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CenturyGame.AppUpdaterLib.Runtime
{
    public class AssetsFileSystem
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private static StringBuilder TmpSB = new StringBuilder();

        private static readonly Dictionary<string, ABTableItemInfoClient> configList = new Dictionary<string, ABTableItemInfoClient>(StringComparer.OrdinalIgnoreCase);

        public const string AppInfoFileName = "appInfo.x";

        public const string AppDataResLocalRoot = "Conf";
        public const string AppDataResRemoteRoot = "data";

        public const string AppUnityResLocalRoot = "resource";
        public const string AppUnityResRemoteRoot = "resource";

        public const string AppDataResManifestName = "res_data.json";

        public const string UnityABFileName = "FileList.x";
        public const string RemoteUnityResManifestPattern = "res_{0}-{1}.json";

        public const string UnityResManifestNamePattern = "res_{0}.json";
        public static string UnityResManifestName => string.Format(UnityResManifestNamePattern, GetPlatformStringForConfig());

        public static readonly string RootFolderName = "Assets";
        private static string s_mRootFolder = string.Empty;
        public static string RootFolder
        {
            get
            {
                if(string.IsNullOrEmpty(s_mRootFolder))
                    s_mRootFolder = $"{PersistentDataPath}/{RootFolderName}";
                return s_mRootFolder;
            }
        }

        public static readonly string AbConfigInfoName = "FileListInfo.x";
        public static readonly string AbConfigInfoClientName = "FileListClientInfo.x";

        public static ABConfigInfoClient ConfigInfoClient = null;

        public static Version RemoteConfigInfoVer = null;

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        private static string mPersistentDataPath = string.Empty;
        public static string PersistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(mPersistentDataPath))
                {
#if UNITY_EDITOR
                    mPersistentDataPath = System.IO.Path.GetFullPath(Application.dataPath + "/../SandBox");
                    mPersistentDataPath = mPersistentDataPath.Replace(@"\", @"/");
#else
                mPersistentDataPath = Application.persistentDataPath;
#endif
                }
                return mPersistentDataPath;
            }
        }

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public static string GetStreamingAssetsPath(string path, string ext = null, bool loadAB = true)
        {
            return GetStreamingAssetsPathInternal(path,ext,loadAB);
        }

        private static string GetStreamingAssetsPathInternal(string path, string ext, bool loadAB)
        {
            if (loadAB && configList.ContainsKey(path))
            {
                ABTableItemInfoClient item = configList[path];
                if (item.R)
                    return GetWritePath(path, true, ext);
            }
            TmpSB.Length = 0;

#if UNITY_ANDROID && !UNITY_EDITOR
            if(loadAB)
            {
                TmpSB.Append(Application.dataPath);
                TmpSB.Append("!assets/");
            }
            else
            {
                TmpSB.Append(Application.streamingAssetsPath);
                TmpSB.Append("/");
            }
#elif UNITY_IPHONE && !UNITY_EDITOR
            if(loadAB)
            {
                TmpSB.Append(Application.dataPath);
                TmpSB.Append("/Raw/");
            }
            else
            {
                TmpSB.Append("file://");
                TmpSB.Append(Application.streamingAssetsPath);
                TmpSB.Append("/");
            }
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (!loadAB)
            {
                TmpSB.Append("file://");
            }
            TmpSB.Append(Application.streamingAssetsPath);
            TmpSB.Append("/");
#endif

#if INCLUDE_PLATFORM_NAME_TO_STREAMINGASSETS

            TmpSB.Append(Utility.GetPlatformName());
            TmpSB.Append("/");
#endif

            TmpSB.Append(path);
            if (ext != null)
                TmpSB.Append(ext);
            return TmpSB.ToString();
        }

        public static string GetWritePath(string path, bool createFolder = false, string ext = null)
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (!Directory.Exists(RootFolder))
            {
                Directory.CreateDirectory(RootFolder);
            }
            string result = null;
            if (createFolder)
            {
                TmpSB.Length = 0;
                TmpSB.Append(PersistentDataPath);
                TmpSB.Append("/");
                TmpSB.Append(RootFolderName);
                string[] ps = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (ps.Length > 1)
                {
                    for (int i = 0; i < ps.Length; i++)
                    {
                        TmpSB.Append("/");
                        TmpSB.Append(ps[i]);
                        result = TmpSB.ToString();
                        if (i < ps.Length - 1)
                        {
                            DirectoryInfo folder = new DirectoryInfo(result);
                            if (!folder.Exists)
                                folder.Create();
                        }
                    }
                }
                else
                {
                    TmpSB.Append("/");
                    TmpSB.Append(path);
                }
            }
            else
            {
                TmpSB.Length = 0;
                TmpSB.Append(PersistentDataPath);
                TmpSB.Append("/");
                TmpSB.Append(RootFolderName);
                TmpSB.Append("/");
                TmpSB.Append(path);
            }
            if (ext != null)
                TmpSB.Append(ext);
            result = TmpSB.ToString();
            return result;
        }

        /*
        public static void ABConfigInfoClientCopyFrom(VersionManifest config, string clientPath)
        {
            configList.Clear();
            ABConfigInfoClient clientInfo = new ABConfigInfoClient();
            //clientInfo.Ver = config.Ver;
            clientInfo.List = new ABTableItemInfoClient[config.Datas.Count];
            for (int i = 0; i < config.Datas.Count; i++)
            {
                ABTableItemInfoClient infoClient = new ABTableItemInfoClient();
                var info = config.Datas[i];
                infoClient.N = info.N;
                infoClient.H = info.H;
                infoClient.S = info.S;
                infoClient.R = false;//因为使用的是内置的清单，所以当前默认文件都需要读内置的文件
                clientInfo.List[i] = infoClient;
                configList[infoClient.N] = infoClient;
            }
            ConfigInfoClient = null;
            ConfigInfoClient = clientInfo;
            string json = JsonUtility.ToJson(ConfigInfoClient);
            File.WriteAllBytes(clientPath, System.Text.Encoding.UTF8.GetBytes(json));
        }
        
        public static void ABConfigInfoClientCopyFrom(ABConfigInfoClient config)
        {
            configList.Clear();
            for (int i = 0; i < config.List.Length; i++)
            {
                ABTableItemInfoClient info = config.List[i];
                configList[info.N] = info;
            }
        }
        */
        public static string GetPlatformStr()
        {
#if UNITY_EDITOR
            switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
            {
                case UnityEditor.BuildTarget.Android:
                    {
                        return "/Android/";
                    }
                case UnityEditor.BuildTarget.iOS:
                    {
                        return "/IOS/";
                    }
                default:
                    {
                        return "/Standlone/";
                    }
            }
#else
#if UNITY_ANDROID && !UNITY_EDITOR
                return "/Android/";
#elif UNITY_IPHONE && !UNITY_EDITOR
                return "/IOS/";
#else
                return "/Standlone/";
#endif
        
#endif
        }

        public static FileDesc GetTask(FileDesc item, ref bool add)
        {
            add = false;
            FileDesc result = null;
            if (configList.ContainsKey(item.N))
            {
                result = configList[item.N];
                if (result.H.CompareTo(item.H) != 0)
                {
                    add = true;
                    result = item;
                }
            }
            else
            {
                add = true;
                result = item;
            }
            return result;
        }


        public static void UpdateTask(FileDesc task)
        {
            if (configList.ContainsKey(task.N))
            {
                ABTableItemInfoClient item = configList[task.N];
                item.H = task.H;
                item.S = task.S;
                item.R = true;
            }
            else
            {
                ABTableItemInfoClient item = new ABTableItemInfoClient();
                item.N = task.N;
                item.H = task.H;
                item.S = task.S;
                item.R = true;
                configList[task.N] = item;
            }
        }


        /// <summary>
        /// 保存本地热更的配置到手机
        /// </summary>
        /// <param name="saveVer"></param>
        public static void SaveConfigInfoClient(bool saveVer)
        {
            List<ABTableItemInfoClient> tmpList = new List<ABTableItemInfoClient>();
            foreach (KeyValuePair<string, ABTableItemInfoClient> kv in configList)
            {
                tmpList.Add(kv.Value);
            }
            if (ConfigInfoClient != null)
            {
                if (saveVer)
                {
                    ConfigInfoClient.Ver = RemoteConfigInfoVer.GetVersionString();
                }
                ConfigInfoClient.List = tmpList.ToArray();
                string json = JsonUtility.ToJson(ConfigInfoClient);
                File.WriteAllBytes(string.Concat(GetWritePath(AbConfigInfoClientName)), System.Text.Encoding.UTF8.GetBytes(json));
                ConfigInfoClient = null;
            }
            else
            {
                Debug.Log("Save ConfigInfoClient fail!");
            }
        }


        public static string GetPlatformStringForConfig()
        {
#if UNITY_EDITOR
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    {
                        return "android";
                    }
                case BuildTarget.iOS:
                    {
                        return "ios";
                    }
                default:
                    {
                        return "android";
                    }
            }
#else
#if UNITY_ANDROID && !UNITY_EDITOR
                        return "android";
#elif UNITY_IPHONE && !UNITY_EDITOR
                        return "ios";
#else
                        return "android";
#endif

#endif
        }

#endregion

    }
}
