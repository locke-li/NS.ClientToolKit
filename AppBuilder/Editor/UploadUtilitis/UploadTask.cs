/***************************************************************

 *  类名称：        UploadTask

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/26 11:26:13

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenturyGame.Editor.UploadUtilitis
{
    public struct UploadTask
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        public string Source, Target;
        //public bool IsFolder;

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        public UploadTask(string source, string target)
        {
            this.Source = source;
            this.Target = target;
        }

        #endregion

        //--------------------------------------------------------------

        #region Methods

        //--------------------------------------------------------------

        #endregion

    }
}
