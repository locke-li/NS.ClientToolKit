/***************************************************************

 *  类名称：        VersionStringInvalidException

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/6 10:23:33

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;

namespace CenturyGame.AppBuilder.Runtime.Exceptions
{
    public class InvalidVersionStringException : Exception
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

        public InvalidVersionStringException() : base($"Invalid version null number !")
        {

        }

        public InvalidVersionStringException(string versionStsr) : base($"Invalid version number : {versionStsr} !")
        {
        }


        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        #endregion

    }
}
