/***************************************************************

 *  类名称：        RemoteFileDownloader

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/19 11:14:13

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.LoggerModule.Runtime;
using System;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    internal class RemoteFileDownloader : IDisposable
    {
        #region Enums

        public enum InnerState
        {
            Idle,

            DownloadResFileFromCdn,

            DownloadingCdn,

            DownloadResFileFromOss,

            DownloadingOss,

            DownloadResFileFailure,
        }

        #endregion

        //--------------------------------------------------------------

        #region Fields

        //--------------------------------------------------------------

        private InnerState mState = InnerState.Idle;

        private Action<bool, byte[]> mLoadResFileEvent = null;

        private string mFileName = null;

        private AppUpdaterContext mContext;

        /// <summary>
        /// http下载组件
        /// </summary>
        private HttpRequest mHttpComponnent;

        private static readonly ILogger s_mLogger = LoggerManager.GetLogger("RemoteFileDownloader");
        #endregion

        //--------------------------------------------------------------

        #region Properties & Events

        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------

        #region Creation & Cleanup

        //--------------------------------------------------------------

        public RemoteFileDownloader(AppUpdaterContext context , HttpRequest httpComponnent)
        {
            this.mContext = context;
            this.mHttpComponnent = httpComponnent;
        }

        #endregion

        //--------------------------------------------------------------

        #region Methods

        //--------------------------------------------------------------

        public void Update()
        {
            this.mHttpComponnent.Update();

            switch (mState)
            {
                case InnerState.DownloadResFileFromCdn:
                    this.RequestRemoteFile(FileServerType.CDN);
                    break;
                case InnerState.DownloadResFileFromOss:
                    this.RequestRemoteFile(FileServerType.OSS);
                    break;
                case InnerState.DownloadResFileFailure:
                    break;
            }
        }

        public void StartLoadResFileFromRemote(Action<bool, byte[]> callback , string fileName)
        {
            this.Clear();

            this.mLoadResFileEvent = callback;

            this.mFileName = fileName;

            this.mState = InnerState.DownloadResFileFromCdn;
        }

        private void RequestRemoteFile(FileServerType fileServerType)
        {
            string fileUrl = this.mContext.GetRemoteResFileUrl(this.mFileName,fileServerType);

            if (fileServerType == FileServerType.CDN)
            {
                this.mState = InnerState.DownloadingCdn;
            }
            else
            {
                this.mState = InnerState.DownloadingOss;
            }
            s_mLogger.Info($"Downloading {fileUrl}");
            this.mHttpComponnent.Load(fileUrl, OnRemoteFileDataRet);
        }

        private void OnRemoteFileDataRet(byte[] netData)
        {
            if (netData == null || netData.Length == 0)
            {
                switch (mState)
                {
                    case InnerState.DownloadingCdn:
                        this.mState = InnerState.DownloadResFileFromOss;
                        break;
                    case InnerState.DownloadingOss:
                        this.mState = InnerState.DownloadResFileFailure;
                        this.mLoadResFileEvent?.Invoke(false, null);
                        break;
                }
            }
            else
            {
                this.mLoadResFileEvent?.Invoke(true, netData);
            }
        }


        public void Clear()
        {
            this.mState = InnerState.Idle;
            this.mLoadResFileEvent = null;
            this.mFileName = null;
        }

        public void Dispose()
        {
            this.mState = InnerState.Idle;
            this.mLoadResFileEvent = null;
            this.mContext = null;
            this.mHttpComponnent = null;
        }

        #endregion
    }
}
