/***************************************************************

 *  类名称：        FileDownloader

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/12/24 16:55:10

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CenturyGame.FilesDeferredDownloader.Runtime
{
    public class FileDownloader : MonoBehaviour
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        public enum InnerState
        {
            Idle,

            DownloadFromCDN,

            DownloadFromOSS,
            
            DownloadFailure,

            DownloadSuccess,
        }

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



        #endregion
    }
}
