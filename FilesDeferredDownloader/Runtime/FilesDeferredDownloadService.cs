/***************************************************************

 *  类名称：        FilesDeferredDownloadService

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/12/23 20:46:48

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.AppUpdaterLib.Runtime.ResManifestParser;
using CenturyGame.FilesDeferredDownloader.Runtime.Configs;
using CenturyGame.LoggerModule.Runtime;
using UnityEngine;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;

namespace CenturyGame.FilesDeferredDownloader.Runtime
{
    internal class FilesDeferredDownloadService : MonoBehaviour
    {
        #region Inner Class & Enum ...

        public enum DownloadState
        {
            StartInit,

            Initializing,

            InitializeError,

            Initialized,

            Idle,

            StartInitFileSetList,

            InitializingFileSetList,

            InitFileSetListFailure,

            StartDownloadFiles,

            DownloadingFiles,

            DownloadFileSetSuccess,

            DownloadFailed,

            DownloadSuccess,
        }

        public class InternalCallBacks
        {
            public Action<bool> initialize;
            public Action<bool, string, long, string> fileDownload;
            public Action<bool, string, string> fileSetDownload;
            public Action completed;
            public Action<DeferredDownloadErrorType> error;

            public void Clear()
            {
                initialize = null;
                completed = null;
                fileDownload = null;
                fileSetDownload = null;
                error = null;
            }
        }


        #endregion

        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        public const string DeferredDownloadManifestPattern = "res_deferred_{0}.json";

        /// <summary>
        /// 本地延迟载入的文件列表
        /// </summary>
        public const string LocalDownloadManifestName = "res_deferred_local.json";

        private Encoding mEncoding = new UTF8Encoding(false,true);

        private DeferredDownloadFileListConfig mTargetFileSetConfig = null;

        private DeferredDownloadFileListConfig mLocalFileSetConfig = null;

        private static readonly Lazy<LoggerModule.Runtime.ILogger> s_mLogger = new Lazy<ILogger>(() =>
            LoggerManager.GetLogger("FilesDeferredDownloadService"));

        private InternalCallBacks mCallbacks = new InternalCallBacks();
        
        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        public static string DeferredDownloadManifestName => string.Format(DeferredDownloadManifestPattern, AssetsFileSystem.GetPlatformStringForConfig());

        private HttpRequest mHttpRequest = new HttpRequest();
        private FileDownloader mDownloader;

        public ProgressData ProgressData { private set; get; } = new ProgressData();

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        private void Start()
        {
            var httpCore = gameObject.AddComponent<HttpDownloadComponent>();
            this.mDownloader = new FileDownloader(httpCore);
            this.mDownloader.SetDownloadCallBack(this.OnDownloadFileCompleted);
            this.mState = DownloadState.StartInit;
        }

        #region Internal calls

        private void Update()
        {
            switch (this.mState)
            {
                case DownloadState.StartInit:
                    this.OnStartInit();
                    break;
                case DownloadState.InitializeError:
                    this.OnInitializeError();
                    break;
                case DownloadState.Initialized:
                    this.OnInitialized();
                    break;
                case DownloadState.StartInitFileSetList:
                    this.OnStartInitFileSetList();
                    break;
                case DownloadState.InitFileSetListFailure:
                    this.OnInitFileSetListFailure();
                    break;
                case DownloadState.StartDownloadFiles:
                    this.OnStartDownloadFiles();
                    break;
                case DownloadState.DownloadingFiles:
                    this.OnDownloadingFiles();
                    break;
                case DownloadState.DownloadFileSetSuccess:
                    this.OnDownloadFileSetSuccess();
                    break;
                case DownloadState.DownloadFailed:
                    this.OnDownloadFailure();
                    break;
                case DownloadState.DownloadSuccess:
                    this.OnDownloadSuccess();
                    break;
                default:
                    break;
            }

            this.mHttpRequest?.Update();
            this.mDownloader.Update();
        }

        #region 初始化
        private void OnStartInit()
        {
            this.mState = DownloadState.Initializing;
            this.LoadTargetFileSetListConfig();
        }

        private void LoadTargetFileSetListConfig()
        {
            var url = $"{AssetsFileSystem.StreamingAssetsUrl}{DeferredDownloadManifestName}";
            this.mHttpRequest.Load(url, this.OnTargetFileListConfigLoadedCallBack);
        }

        private void OnTargetFileListConfigLoadedCallBack(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                this.mState = DownloadState.InitializeError;
            }
            else
            {
                var contents = new UTF8Encoding(false, true).GetString(bytes);
                mTargetFileSetConfig = JsonUtility.FromJson<DeferredDownloadFileListConfig>(contents);
                this.LoadLocalFileSetListConfig();
            }
        }

        private void LoadLocalFileSetListConfig()
        {
            var configPath = AssetsFileSystem.GetWritePath(LocalDownloadManifestName);
            bool needCreateLocalFileSetConfig = true;
            if (File.Exists(configPath))
            {
                var localContetns = File.ReadAllText(configPath, mEncoding);
                var fileLocalFileSetConfig = JsonUtility.FromJson<DeferredDownloadFileListConfig>(localContetns);
                if (string.Equals(fileLocalFileSetConfig.md5, this.mTargetFileSetConfig.md5))
                {
                    this.mLocalFileSetConfig = fileLocalFileSetConfig;
                    needCreateLocalFileSetConfig = false;
                }
                else
                {
                    File.Delete(configPath);
                    s_mLogger?.Value.Info($"Delete local config that path is \"{configPath}\", because it is too old.");
                }
            }

            if (needCreateLocalFileSetConfig)
            {
                this.mLocalFileSetConfig = new DeferredDownloadFileListConfig();
                this.mLocalFileSetConfig.md5 = this.mTargetFileSetConfig.md5;
                this.SerializeLocalFileSetConfig();
            }
            this.mState = DownloadState.Initialized;
        }

        private void SerializeLocalFileSetConfig()
        {
            var path = AssetsFileSystem.GetWritePath(LocalDownloadManifestName);
            File.WriteAllText(path, JsonUtility.ToJson(this.mLocalFileSetConfig));
            s_mLogger.Value?.Debug("Serialize local file set config.");
        }

        private void OnInitializeError()
        {
            try
            {
                this.mCallbacks.initialize?.Invoke(false);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.enabled = false;
            }
        }

        private void OnInitialized()
        {
            try
            {
                this.mCallbacks.initialize?.Invoke(true);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.mState = DownloadState.Idle;
            }
        }

        #endregion

        private void OnStartInitFileSetList()
        {
            this.ProgressData.Clear();
            this.mCurSetName = this.mCurFileSetQueue.Dequeue();
            this.ProgressData.FileSetName = this.mCurSetName;
            this.mState = DownloadState.InitializingFileSetList;
            //string fileExternalPath = AssetsFileSystem.GetWritePath(this.mCurSetName);
            //if (File.Exists(fileExternalPath))
            //{
            //    this.LoadLocalFileSetManifest(fileExternalPath);
            //    this.mState = DownloadState.StartDownloadFiles;
            //}
            //else
            //{
                var url = $"{AssetsFileSystem.StreamingAssetsUrl}{this.mCurSetName}";
                this.mHttpRequest.Load(url, this.OnLoadFileSetManifestCallBack);
            //}
        }

        private void OnLoadFileSetManifestCallBack(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                this.mState = DownloadState.InitFileSetListFailure;
            }
            else
            {
                //string fileExternalPath = AssetsFileSystem.GetWritePath(this.mCurSetName);
                //File.WriteAllBytes(fileExternalPath, bytes);
                //this.LoadLocalFileSetManifest(fileExternalPath);
                //this.mState = DownloadState.StartDownloadFiles;

                var manifestContent = mEncoding.GetString(bytes);
                this.mFileSetManifest = VersionManifestParser.Parse(manifestContent);
                this.ProgressData.TotalDownloadFileCount = this.mFileSetManifest.Count;
                this.ProgressData.TotalDownloadSize = this.mFileSetManifest.GetTotalSize();
                this.mState = DownloadState.StartDownloadFiles;
            }
        }

        //private void LoadLocalFileSetManifest(string path)
        //{
        //    var manifestContent = File.ReadAllText(path, mEncoding);
        //    this.mFileSetManifest = VersionManifestParser.Parse(manifestContent);
        //    this.ProgressData.TotalDownloadFileCount = this.mFileSetManifest.Count;
        //    this.ProgressData.TotalDownloadSize = this.mFileSetManifest.GetTotalSize();
        //}

        private void OnInitFileSetListFailure()
        {
            this.mCallbacks.error?.Invoke(DeferredDownloadErrorType.FileSetNotFound);
            this.Clear();
        }

        private void OnStartDownloadFiles()
        {
            this.mDownloadIdx = 0;
            this.mState = DownloadState.DownloadingFiles;
            this.StartDownloadFileInternal();
        }

        private void OnDownloadingFiles()
        {
            this.ProgressData.CurrentDownloadingFileProgress = this.mDownloader.GetProgress();
        }

        private void OnDownloadFileSetSuccess()
        {
            this.SaveLocalFileSetConfig();
            this.mCallbacks.fileSetDownload?.Invoke(true, 
                this.mCurSetName,this.mTargetFileSetConfig.GetFileSetMD5(this.mCurSetName));
            if (this.mCurFileSetQueue.Count == 0)
            {
                this.mState = DownloadState.DownloadSuccess;
            }
            else
            {
                this.mState = DownloadState.StartInitFileSetList;
            }
        }

        private void OnDownloadFailure()
        {
            this.Clear();
            this.mCallbacks.error?.Invoke(DeferredDownloadErrorType.FileDownloadFailure);
        }

        private void OnDownloadSuccess()
        {
            this.Clear();
            this.mCallbacks.completed?.Invoke();
        }

        private void SaveLocalFileSetConfig()
        {
            var md5 = this.mTargetFileSetConfig.GetFileSetMD5(this.mCurSetName);
            this.mLocalFileSetConfig.AddOrUpdate(this.mCurSetName, md5);
            this.SerializeLocalFileSetConfig();
        }

        private void StartDownloadFileInternal()
        {
            var file = this.mFileSetManifest.Datas[this.mDownloadIdx];
            this.ProgressData.CurrentDownloadingFileSize = (ulong)file.S;
            this.mDownloader.Download(file);
        }

        private void OnDownloadFileCompleted(bool result)
        {
            var fileDesc = this.mFileSetManifest.Datas[this.mDownloadIdx];
            this.mCallbacks.fileDownload?.Invoke(result, fileDesc.N, fileDesc.S, fileDesc.H);
            if (result)
            {
                this.mDownloadIdx++;
                this.ProgressData.CurrentDownloadedFileCount = this.mDownloadIdx;
                this.ProgressData.CurrentDownloadedFileSize += (ulong)fileDesc.S;

                if (this.mDownloadIdx >= this.mFileSetManifest.Datas.Count)
                {
                    this.mState = DownloadState.DownloadFileSetSuccess;
                }
                else
                {
                    this.StartDownloadFileInternal();
                }
            }
            else
            {
                this.mCallbacks.fileSetDownload?.Invoke(false,this.mCurSetName,this.mTargetFileSetConfig.GetFileSetMD5(this.mCurSetName));
                this.mState = DownloadState.DownloadFailed;
            }
        }


        private DownloadState mState = DownloadState.StartInit;
        private Queue<string> mCurFileSetQueue = new Queue<string>();
        private string mCurSetName = string.Empty;
        private VersionManifest mFileSetManifest = null;
        private int mDownloadIdx = -1;
        private void Clear()
        {
            this.mState = DownloadState.Idle;
            this.mCurFileSetQueue.Clear();
            this.mFileSetManifest = null;
            this.mDownloadIdx = -1;
        }

        #endregion

        public void SyncFiles(out bool addedResult, params string[] fileSetNames)
        {
            if (this.mState != DownloadState.Idle)
            {
                s_mLogger.Value?.Warn("Downloader is working , Pleause try later!");
                addedResult = false;
                return;
            }

            if (fileSetNames == null || fileSetNames.Length == 0)
            {
                s_mLogger.Value?.Warn("The file Sets that you want to download has no element!");
                addedResult = false;
                return;
            }

            foreach (var fileSetName in fileSetNames)
            {
                if (!IsFileSetNameValid(fileSetName))
                {
                    s_mLogger.Value?.Fatal($"The file set that name is \"{fileSetName}\" is invalid .");
                    addedResult = false;
                    return;
                }
            }

            foreach (var fileSetName in fileSetNames)
            {
                if (!IsFileSetPrepared(fileSetName))
                {
                    this.mCurFileSetQueue.Enqueue(fileSetName);
                    s_mLogger.Value?.Debug($"Add file set that name is \"{fileSetName}\" to download queue .");
                }
                else
                {
                    s_mLogger.Value?.Fatal($"The file set that name is \"{fileSetName}\" is prepared .");
                }
            }

            addedResult = true;
            if (this.mCurFileSetQueue.Count == 0)
            {
                s_mLogger.Value?.Info("All off file set in the current download request is prepared!");
                addedResult = true;
                return;
            }
            
            this.mState = DownloadState.StartInitFileSetList;
        }

        public void SetOnInitCompletedCallBack(Action<bool> callback)
        {
            this.mCallbacks.initialize = callback;
        }

        public void SetOnFileDownloadCallBack(Action<bool, string, long, string> callback)
        {
            this.mCallbacks.fileDownload = callback;
        }

        public void SetOnFileSetDownloadCallBack(Action<bool, string, string> callback)
        {
            this.mCallbacks.fileSetDownload = callback;
        }

        public void SetDownloadCompletedCallBack(Action callback)
        {
            this.mCallbacks.completed = callback;
        }

        public void SetDownloadErrorCallBack(Action<DeferredDownloadErrorType> callback)
        {
            this.mCallbacks.error = callback;
        }

        public bool IsFileSetPrepared(string fileSetName)
        {
            if (string.IsNullOrEmpty(fileSetName))
            {
                throw new ArgumentNullException(nameof(fileSetName));
            }

            if (!this.mTargetFileSetConfig.Exist(fileSetName))
            {
                throw new InvalidOperationException($"Invalid file set , file name is \"{fileSetName}\" .");
            }

            if (!this.mLocalFileSetConfig.Exist(fileSetName))
            {
                return false;
            }

            var localMD5 = this.mLocalFileSetConfig.GetFileSetMD5(fileSetName);
            var targetMD5 = this.mTargetFileSetConfig.GetFileSetMD5(fileSetName);
            return string.Equals(localMD5,targetMD5);
        }

        public bool IsFileSetNameValid(string fileSetName)
        {
            if (string.IsNullOrEmpty(fileSetName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(fileSetName))
            {
                return false;
            }

            if (!this.mTargetFileSetConfig.Exist(fileSetName))
            {
                return false;
            }

            return true;
        }

        #endregion

    }
}
