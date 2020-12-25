/***************************************************************

 *  类名称：        FilesDeferredDownloadService

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/12/23 20:46:48

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using UnityEngine;

namespace CenturyGame.FilesDeferredDownloader.Runtime
{
    internal class FilesDeferredDownloadService : MonoBehaviour
    {
        #region Inner Class & Enum ...

        public enum DownloadState
        {
            Idle,

            StartDownloadFiles,

            DownloadingFiles,

            DownloadFailed,

            DownloadSuccess,
        }

        //public class InternalCallBacks
        //{
        //    public System.Action<bool> DownloadCompleted;
            
        //}
        

        #endregion

        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private DownloadState mState = DownloadState.Idle;

        private string mCurFileSetName = string.Empty;

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

        private void Update()
        {
            switch (this.mState)
            {
                case DownloadState.Idle:
                    break;
                case DownloadState.StartDownloadFiles:
                    break;
                case DownloadState.DownloadingFiles:
                    break;
                case DownloadState.DownloadFailed:
                    break;
                case DownloadState.DownloadSuccess:
                    this.OnDownloadSuccess();
                    break;
                default:
                    break;
            }
        }


        private void OnStartDownloadFiles()
        {
          
        }

        private void OnDownloadFailure()
        {
            this.Clear();
        }

        private void OnDownloadSuccess()
        {
            this.Clear();
        }

        public void SyncFiles(string fileSetName)
        {
            if (this.mState != DownloadState.Idle)
            {
                return;
            }
            this.mCurFileSetName = fileSetName;
            this.mState = DownloadState.StartDownloadFiles;
        }


        private void Clear()
        {
            this.mState = DownloadState.Idle;
            this.mCurFileSetName = string.Empty;
        }
        #endregion

    }
}
