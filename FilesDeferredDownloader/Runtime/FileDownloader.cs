/***************************************************************

 *  类名称：        FileDownloader

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/12/24 16:55:10

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.AppUpdaterLib.Runtime.Managers;
using CenturyGame.LoggerModule.Runtime;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;

namespace CenturyGame.FilesDeferredDownloader.Runtime
{
    public class FileDownloader
    {
        //--------------------------------------------------------------
        #region Inner Class & Enum ...
        //--------------------------------------------------------------
        public enum InnerState
        {
            Idle,

            StartDownloadFromCDN,

            DownloadingFromCDN,

            StartDownloadFromOSS,

            DownloadingFromOSS,

            DownloadFailure,

            DownloadSuccess,
        }

        #endregion

        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private static readonly Lazy<LoggerModule.Runtime.ILogger> s_mLogger = new Lazy<ILogger>(() =>
            LoggerManager.GetLogger("FileDownloader"));

        private FileDownloadCallBack mCallBack;
        private HttpDownloadComponent mDownloadCore;

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------
        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        public FileDownloader(HttpDownloadComponent downloadCore)
        {
            this.mDownloadCore = downloadCore;
        }

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------


        #region Inner call
        public void Update()
        {
            switch (this.mState)
            {
                case InnerState.StartDownloadFromCDN:
                    this.OnStartDownloadFromCDN();
                    break;
                case InnerState.StartDownloadFromOSS:
                    this.OnStartDownloadFromOSS();
                    break;
                case InnerState.DownloadFailure:
                    this.OnDownloadFailure();
                    break;
                case InnerState.DownloadSuccess:
                    this.OnDownloadSuccess();
                    break;
                default:break;
            }    
        }

        private void OnStartDownloadFromCDN()
        {
            this.mState = InnerState.DownloadingFromCDN;
            this.StartDownloadInternal(FileServerType.CDN);
        }

        private void OnStartDownloadFromOSS()
        {
            s_mLogger.Value?.Debug("Start download file form oss .");
            this.mState = InnerState.DownloadingFromOSS;
            this.StartDownloadInternal(FileServerType.OSS);
        }

        private void OnDownloadFailure()
        {
            this.Clear();
            this.mCallBack?.Invoke(false);
        }

        private void OnDownloadSuccess()
        {
            this.Clear();
            this.mCallBack?.Invoke(true);
        }

        private string GetRemoteResFileUrl(string fileName, FileServerType fileServerType = FileServerType.CDN)
        {
            var config = AppUpdaterManager.AppUpdaterConfig;
            string serverUrl = (fileServerType == FileServerType.CDN) ? config.cdnUrl : config.ossUrl;
            string url = $"{serverUrl}/{fileName}";
            return url;
        }

        private void StartDownloadInternal(FileServerType serverType)
        {
            var url = GetRemoteResFileUrl(this.mCurDownloadInfo.GetRNUTF8(), serverType);
            string filePath = AssetsFileSystem.GetWritePath(this.mCurDownloadInfo.N);
            this.mDownloadCore.Download(url,filePath,this.mCurDownloadInfo.H,this.OnDownloadCompleted);
        }

        private void OnDownloadCompleted(bool result)
        {
            if (!result && this.mState == InnerState.DownloadingFromCDN)
            {
                this.mState = InnerState.StartDownloadFromOSS;
            }
            else
            {
                this.mState = result ? InnerState.DownloadSuccess : InnerState.DownloadFailure;
            }
        }

        #endregion

        public void SetDownloadCallBack(FileDownloadCallBack callback)
        {
            this.mCallBack = callback;
        }

        public bool IsWorking()
        {
            return this.mState != InnerState.Idle;
        }

        public void Download(FileDesc fileDesc)
        {
            if (IsWorking())
            {
                return;
            }
            this.Collect(fileDesc);
            this.mState = InnerState.StartDownloadFromCDN;
        }

        public float GetProgress()
        {
            return this.mDownloadCore.Progress;
        }


        private void Collect(FileDesc fileDesc)
        {
            this.mCurDownloadInfo = fileDesc;
        }

        private InnerState mState = InnerState.Idle;
        private FileDesc mCurDownloadInfo;
        

        private void Clear()
        {
            this.mState = InnerState.Idle;
            this.mCurDownloadInfo = null;
        }

        #endregion
    }
}
