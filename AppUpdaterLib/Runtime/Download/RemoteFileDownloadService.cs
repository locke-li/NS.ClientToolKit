/***************************************************************

 *  类名称：        RemoteFileDownloadService

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2021/2/24 11:00:30

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using UnityEngine;

namespace CenturyGame.AppUpdaterLib.Runtime.Download
{
    internal class RemoteFileDownloadService : MonoBehaviour, IRemoteFileDownloadService,IDisposable
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private FileDownloader mDownloader = null;

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

        #region Unity

        void Awake()
        {
            var httpComponent = gameObject.AddComponent<HttpDownloadComponent>();
            this.mDownloader = new FileDownloader(httpComponent);
        }

        void Update()
        {
            this.mDownloader.Update();
        }

        #endregion


        public void SetDownloadCallBack(FileDownloadCallBack fileDesc)
        {
            this.mDownloader.SetDownloadCallBack(fileDesc);
        }

        public void StartDownload(FileDesc fileDesc)
        {
            this.mDownloader.Download(fileDesc);
        }

        public void Dispose()
        {

        }

        #endregion

    }
}
