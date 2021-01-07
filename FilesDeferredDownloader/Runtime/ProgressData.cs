/***************************************************************

 *  类名称：        PrograssData

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/12/29 11:04:42

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenturyGame.FilesDeferredDownloader.Runtime
{
    public class ProgressData
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        /// <summary>
        /// 文件集名
        /// </summary>
        public string FileSetName { set; get; }

        /// <summary>
        /// 当前已下载的文件数量
        /// </summary>
        public int CurrentDownloadedFileCount { set; get; }

        /// <summary>
        /// 需要下载的文件总数
        /// </summary>
        public int TotalDownloadFileCount { set; get; }

        /// <summary>
        /// 当前正在下载的文件大小
        /// </summary>
        public ulong CurrentDownloadingFileSize { set; get; }

        /// <summary>
        /// 当前正在下载的文件进度
        /// </summary>
        public float CurrentDownloadingFileProgress { set; get; }

        /// <summary>
        /// 当前已下载的文件大小
        /// </summary>
        public ulong CurrentDownloadedFileSize { set; get; }

        /// <summary>
        /// 需要下载的文件总大小
        /// </summary>
        public ulong TotalDownloadSize { set; get; }

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------
        
        /// <summary>
        /// 当前下载进度
        /// </summary>
        public float Progress
        {
            get
            {
                if (this.TotalDownloadSize == 0)
                    return 0;
                return this.CurrentDownloadedFileSize / (float) this.TotalDownloadSize;
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

        public void Clear()
        {
            this.FileSetName = string.Empty;
            this.CurrentDownloadedFileCount = 0;
            this.TotalDownloadFileCount = 0;
            this.CurrentDownloadingFileSize = 0;
            this.CurrentDownloadingFileProgress = 0;
            this.CurrentDownloadedFileSize = 0;
            this.TotalDownloadSize = 0;
        }

        #endregion

    }
}
