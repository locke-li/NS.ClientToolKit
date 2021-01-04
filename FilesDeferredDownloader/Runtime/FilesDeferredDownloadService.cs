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

            DownloadFailed,

            DownloadSuccess,
        }

        public class InternalCallBacks
        {
            public Action<bool> initialize;
            public Action<bool, string, long, string> fileDownload;
            public Action completed;
            public Action<DeferredDownloadErrorType> error;

            public void Clear()
            {
                initialize = null;
                completed = null;
                fileDownload = null;
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

        private void OnStartInit()
        {
            this.mState = DownloadState.Initializing;
            var configPath = AssetsFileSystem.GetWritePath(DeferredDownloadManifestName);
            if (File.Exists(configPath))
            {
                var contents = File.ReadAllText(configPath, mEncoding);
                this.OnFileListConfigLoadCompleted(contents);
            }
            else
            {
                var url = $"{AssetsFileSystem.StreamingAssetsUrl}{DeferredDownloadManifestName}";
                this.mHttpRequest.Load(url, this.OnLoadFileListConfigCallBack);
            }
        }

        private void OnLoadFileListConfigCallBack(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                this.mState = DownloadState.InitializeError;
            }
            else
            {
                var contents = new UTF8Encoding(false, true).GetString(bytes);
                this.OnFileListConfigLoadCompleted(contents);
            }
        }

        private void OnFileListConfigLoadCompleted(string contents)
        {
            mTargetFileSetConfig = JsonUtility.FromJson<DeferredDownloadFileListConfig>(contents);

            var configPath = AssetsFileSystem.GetWritePath(LocalDownloadManifestName);
            if (File.Exists(configPath))
            {
                var localContetns = File.ReadAllText(configPath, mEncoding);
                this.mLocalFileSetConfig = JsonUtility.FromJson<DeferredDownloadFileListConfig>(localContetns);
            }
            else
            {
                this.mLocalFileSetConfig = new DeferredDownloadFileListConfig();
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

        private void OnStartInitFileSetList()
        {
            this.mState = DownloadState.InitializingFileSetList;
            string fileExternalPath = AssetsFileSystem.GetWritePath(this.mCurFileSetName);
            if (File.Exists(fileExternalPath))
            {
                this.LoadLocalFileSetManifest(fileExternalPath);
                this.mState = DownloadState.StartDownloadFiles;
            }
            else
            {
                var url = $"{AssetsFileSystem.StreamingAssetsUrl}{this.mCurFileSetName}";
                this.mHttpRequest.Load(url, this.OnLoadFileSetManifestCallBack);
            }
        }

        private void OnLoadFileSetManifestCallBack(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                this.mState = DownloadState.InitFileSetListFailure;
            }
            else
            {
                string fileExternalPath = AssetsFileSystem.GetWritePath(this.mCurFileSetName);
                File.WriteAllBytes(fileExternalPath, bytes);
                this.LoadLocalFileSetManifest(fileExternalPath);
                this.mState = DownloadState.StartDownloadFiles;
            }
        }

        private void LoadLocalFileSetManifest(string path)
        {
            var manifestContent = File.ReadAllText(path, mEncoding);
            this.mFileSetManifest = VersionManifestParser.Parse(manifestContent);
            this.ProgressData.TotalDownloadFileCount = this.mFileSetManifest.Count;
            this.ProgressData.TotalDownloadSize = this.mFileSetManifest.GetTotalSize();
        }

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

        private void OnDownloadFailure()
        {
            this.Clear();
            this.mCallbacks.error?.Invoke(DeferredDownloadErrorType.FileDownloadFailure);
        }

        private void OnDownloadSuccess()
        {
            this.SaveLocalFileSetConfig();
            this.Clear();
            this.mCallbacks.completed?.Invoke();
        }

        private void SaveLocalFileSetConfig()
        {
            var md5 = this.mTargetFileSetConfig.GetFileSetMD5(this.mCurFileSetName);
            this.mLocalFileSetConfig.AddOrUpdate(this.mCurFileSetName,md5);
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
                    this.mState = DownloadState.DownloadSuccess;
                }
                else
                {
                    this.StartDownloadFileInternal();
                }
            }
            else
            {
                this.mState = DownloadState.DownloadFailed;
            }
        }


        private DownloadState mState = DownloadState.StartInit;
        private string mCurFileSetName = string.Empty;
        private VersionManifest mFileSetManifest = null;
        private int mDownloadIdx = -1;
        private void Clear()
        {
            this.mState = DownloadState.Idle;
            this.mCurFileSetName = string.Empty;
            this.mFileSetManifest = null;
            this.mDownloadIdx = -1;
        }

        #endregion

        public void SyncFiles(string fileSetName)
        {
            if (this.mState != DownloadState.Idle)
            {
                return;
            }

            if (IsFileSetPrepared(fileSetName))
            {
                return;
            }

            this.mCurFileSetName = fileSetName;
            this.ProgressData.Clear();
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
