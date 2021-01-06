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
using CenturyGame.FilesDeferredDownloader.Runtime.Configs;
using UnityEngine;
using CenturyGame.LoggerModule.Runtime;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;
using Object = UnityEngine.Object;

namespace CenturyGame.FilesDeferredDownloader.Runtime
{
    public static class FilesDeferredDownloadManager
    {
        //--------------------------------------------------------------
        #region Inner Class & Enum ...
        //--------------------------------------------------------------
        internal enum InnerState
        {
            UnInitialize,

            Initializing,

            InitializeError,

            Initialized,
        }

        #endregion


        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private static ILogger s_mLogger = LoggerManager.GetLogger("AppUpdaterManager");

        private static InnerState s_mState = InnerState.UnInitialize;

        private static FilesDeferredDownloadService s_mService = null;

        private static Action<bool> mInitCallBack;

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        public static bool Initialized => s_mState == InnerState.Initialized;


        private static DeferredDownloadConfig s_mDeferredDownloadConfig = null;

        public static DeferredDownloadConfig DeferredDownloadConfig
        {
            get
            {
                if (s_mDeferredDownloadConfig == null)
                {
                    var text = Resources.Load<TextAsset>("deferreddownload");
                    if (text == null)
                    {
                        throw new FileNotFoundException("deferreddownload");
                    }

                    s_mDeferredDownloadConfig = JsonUtility.FromJson<DeferredDownloadConfig>(text.text);
                }
                return s_mDeferredDownloadConfig;
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

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init(Action<bool> callBack)
        {
            if (s_mState == InnerState.Initialized)
                return;
            s_mState = InnerState.Initializing;
            mInitCallBack = callBack;
            CreateService();
        }

        private static void CreateService()
        {
            const string serviceName = "FilesDeferredDownloader";
            var serviceGo = new GameObject();
            serviceGo.name = serviceName;
            Object.DontDestroyOnLoad(serviceGo);
            s_mService = serviceGo.AddComponent<FilesDeferredDownloadService>();
            s_mService.SetOnInitCompletedCallBack(result =>
            {
                s_mState = result ? InnerState.Initialized : InnerState.InitializeError;
                try
                {
                    mInitCallBack?.Invoke(result);
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    mInitCallBack = null;
                }
            });
        }

        /// <summary>
        /// 指定文件集是否准备好
        /// </summary>
        /// <param name="fileSetName">文件集名</param>
        /// <returns></returns>
        public static bool IsFileSetPrepared(string fileSetName)
        {
            CheckIsInitialize("IsFileSetPrepared");
            return s_mService.IsFileSetPrepared(fileSetName);
        }

        /// <summary>
        /// 文件集名是否合理
        /// </summary>
        /// <param name="fileSetName">文件集名</param>
        /// <returns></returns>
        public static bool IsFileSetNameValid(string fileSetName)
        {
            CheckIsInitialize("IsFileSetNameValid");
            return s_mService.IsFileSetNameValid(fileSetName);
        }

        /// <summary>
        /// 同步指定文件集的文件到本地
        /// </summary>
        /// <param name="fileSetName"></param>
        public static void SyncFiles(string fileSetName)
        {
            CheckIsInitialize("SyncFiles");
            if (string.IsNullOrEmpty(fileSetName))
            {
                throw new NullReferenceException($"The file set that name is \"{nameof(fileSetName)}\" is null!");
            }
            s_mService.SyncFiles(fileSetName);
        }

        /// <summary>
        /// 获取下载进度数据
        /// </summary>
        /// <returns></returns>
        public static ProgressData GetProgressData()
        {
            CheckIsInitialize("GetProgressData");
            return s_mService.ProgressData;
        }

        /// <summary>
        /// 设置文件下载完成回调
        /// </summary>
        /// <param name="callback">
        /// 参数含义分别为：是否成功，文件名，大小，md5
        /// </param>
        public static void SetOnFileDownloadCallBack(Action<bool, string, long, string> callback)
        {
            CheckIsInitialize("SetOnFileDownloadCallBack");
            s_mService.SetOnFileDownloadCallBack(callback);
        }

        /// <summary>
        /// 设置下载完成回调
        /// </summary>
        /// <param name="completed"></param>
        public static void SetDownloadCompletedCallBack(Action completed)
        {
            CheckIsInitialize("SetDownloadCompletedCallBack");
            s_mService.SetDownloadCompletedCallBack(completed);
        }

        /// <summary>
        /// 设置下载出错回调
        /// </summary>
        /// <param name="callback"></param>
        public static void SetDownloadErrorCallBack(Action<DeferredDownloadErrorType> callback)
        {
            CheckIsInitialize("SetDownloadErrorCallBack");
            s_mService.SetDownloadErrorCallBack(callback);
        }

        private static void CheckIsInitialize(string methodName)
        {
            if (!Initialized)
                throw new NullReferenceException($"Your want to use \"FilesDeferredDownloadManager\" that not initialized ! Call method :  \"{methodName}\" .");
        }

        #endregion

    }
}
