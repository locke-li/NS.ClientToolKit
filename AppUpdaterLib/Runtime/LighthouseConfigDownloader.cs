/***************************************************************

 *  类名称：        LighthouseConfigDownloader

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/14 16:31:49

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.LoggerModule.Runtime;
using System;
using System.Text;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    internal sealed class LighthouseConfigDownloader : IDisposable
    {
        #region Enums

        public enum InnerState
        {
            Idle,

            ReqLighthouseConfigFromCdn,

            RequestingCdn,

            RequestLighthouseConfigFromOss,

            RequestingOss,

            ReqLighthouseConfigFailure,
        }

        #endregion

        //--------------------------------------------------------------

        #region Fields

        //--------------------------------------------------------------

        private static readonly ILogger s_mLogger = LoggerManager.GetLogger("LighthouseConfigDownloader");

        private static Encoding m_Encoding = new UTF8Encoding(false, true);

        private InnerState mState = InnerState.Idle;

        /// <summary>
        /// 需要请求的lighthouse配置的id
        /// </summary>
        private string mTargetLighthouseId = string.Empty;

        /// <summary>
        /// 请求结果返回
        /// </summary>
        private Action<bool, string> mReqLighthouseConfigEvent = null;

        private AppUpdaterContext mContext;

        /// <summary>
        /// http下载组件
        /// </summary>
        private HttpRequest mHttpComponnent;

        #endregion

        //--------------------------------------------------------------

        #region Properties & Events

        //--------------------------------------------------------------

        public FileServerType CurRequestFileServerType { private set; get; } = FileServerType.CDN;

        #endregion

        //--------------------------------------------------------------

        #region Creation & Cleanup

        //--------------------------------------------------------------

        public LighthouseConfigDownloader(AppUpdaterContext context, HttpRequest httpComponnent)
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
            switch (mState)
            {
                case InnerState.ReqLighthouseConfigFromCdn:
                    this.RequestLighthouseConfig(FileServerType.CDN);
                    break;
                case InnerState.RequestLighthouseConfigFromOss:
                    this.RequestLighthouseConfig(FileServerType.OSS);
                    break;
                case InnerState.ReqLighthouseConfigFailure:
                    break;
            }
        }


        public void StartGetLighthouseConfigFromRemote(Action<bool,string> callback, string lighthouseId = null)
        {
            this.Clear();

            this.mReqLighthouseConfigEvent = callback;

            this.mTargetLighthouseId = lighthouseId;

            this.mState = InnerState.ReqLighthouseConfigFromCdn;
        }

        /// <summary>
        /// 请求lighthouse配置
        /// </summary>
        /// <param name="from">
        /// 请求的出处。
        /// 0 ：CDN
        /// 1 ： 
        /// </param>
        private void RequestLighthouseConfig(FileServerType fileServerType)
        {
            string lighthouseUrl = null;

            lighthouseUrl = this.mContext.GetLighthouseUrl(this.mTargetLighthouseId, fileServerType);

            if (fileServerType == FileServerType.CDN)
            {
                s_mLogger.Info($"Request CDN , lighthouseUrl : {lighthouseUrl}");

                this.CurRequestFileServerType = FileServerType.CDN;

                this.mState = InnerState.RequestingCdn;
            }
            else if(fileServerType == FileServerType.OSS)
            {
                s_mLogger.Info($"Request OSS , lighthouseUrl : {lighthouseUrl}");

                this.CurRequestFileServerType = FileServerType.OSS;

                this.mState = InnerState.RequestingOss;
            }

            this.mHttpComponnent.Load(lighthouseUrl, OnLighthouseConfigResponseRet);
        }

        private void OnLighthouseConfigResponseRet(byte[] netData)
        {
            if (netData == null || netData.Length == 0)
            {
                switch (mState)
                {
                    case InnerState.RequestingCdn:
                        this.mState = InnerState.RequestLighthouseConfigFromOss;
                        break;
                    case InnerState.RequestingOss:
                        this.mState = InnerState.ReqLighthouseConfigFailure;
                        this.mReqLighthouseConfigEvent?.Invoke(false,null);
                        break;
                }
            }
            else
            {
                string lighthouseContents = m_Encoding.GetString(netData);
                this.mReqLighthouseConfigEvent?.Invoke(true,lighthouseContents);
            }
            this.Clear();
        }


        public void Clear()
        {
            this.mState = InnerState.Idle;
            this.mTargetLighthouseId = string.Empty;
            this.mReqLighthouseConfigEvent = null;
            this.CurRequestFileServerType = FileServerType.CDN;
        }

        public void Dispose()
        {
            this.mState = InnerState.Idle;
            this.CurRequestFileServerType = FileServerType.CDN;
            this.mTargetLighthouseId = string.Empty;
            this.mReqLighthouseConfigEvent = null;
            this.mContext = null;
            this.mHttpComponnent = null;
        }

        #endregion
    }


}
