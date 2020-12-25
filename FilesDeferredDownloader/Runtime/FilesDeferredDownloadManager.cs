/***************************************************************

 *  类名称：        FilesDeferredDownloadManager

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/12/23 14:28:17

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using System.IO;
using System.Text;
using UnityEngine;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.FilesDeferredDownloader.Runtime.Configs;
using CenturyGame.LoggerModule.Runtime;
using Object = UnityEngine.Object;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;

namespace CenturyGame.FilesDeferredDownloader.Runtime
{
    public static class FilesDeferredDownloadManager
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        public const string DeferredDownloadManifestPattern = "res_deferred_{0}.json";

        public static string DeferredDownloadManifestName => string.Format(DeferredDownloadManifestPattern, AssetsFileSystem.GetPlatformStringForConfig());

        private static ILogger s_mLogger = LoggerManager.GetLogger("AppUpdaterManager");

        private static bool s_mInitialized = false;

        private static DeferredDownloadFileListConfig s_mConfig = null;

        private static FilesDeferredDownloadService s_mService = null;

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if (s_mInitialized)
                return;
            CreateService();
            var configPath= AssetsFileSystem.GetWritePath(DeferredDownloadManifestName);
            if (File.Exists(configPath))
            {
                var contents = File.ReadAllText(configPath, new UTF8Encoding(false, true));

                s_mConfig = JsonUtility.FromJson<DeferredDownloadFileListConfig>(contents);
            }

            s_mInitialized = true;
        }

        private static void CreateService()
        {
            const string serviceName = "FilesDeferredDownloader";
            var serviceGo = new GameObject();
            serviceGo.name = serviceName;
            s_mService = serviceGo.GetComponent<FilesDeferredDownloadService>();
        }

        /// <summary>
        /// 指定文件集是否已下载
        /// </summary>
        /// <param name="fileSetName">文件集名</param>
        /// <returns></returns>
        public static bool FileSetExist(string fileSetName)
        {
            if (s_mConfig == null)
            {
                return false;
            }

            return s_mConfig.Exist(fileSetName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSetName"></param>
        public static void SyncFiles(string fileSetName)
        {
            s_mService.SyncFiles(fileSetName);
        }

        #endregion

    }
}
