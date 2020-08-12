/***************************************************************

 *  类名称：        InvalidVersionNumGetOperationException

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/6 10:38:33

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;

namespace CenturyGame.AppBuilder.Runtime.Exceptions
{
    public class InvalidVersionNumGetOperationException : Exception
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

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public InvalidVersionNumGetOperationException(string propertyName) : base($"Get versoin property failure , property name : {propertyName}!")
        {

        }

        #endregion

    }
}
