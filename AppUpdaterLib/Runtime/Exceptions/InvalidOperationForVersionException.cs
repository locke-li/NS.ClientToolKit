/***************************************************************

 *  类名称：        InvalidOperationForVersionException

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/6 12:13:02

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;

namespace CenturyGame.AppUpdaterLib.Runtime.Exceptions
{
    public class InvalidOperationForVersionException : Exception
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------
        public InvalidOperationForVersionException(string opMethod) : base($"Invalid method call ! method name : {opMethod}!")
        {
        }

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------




        #endregion

    }
}
