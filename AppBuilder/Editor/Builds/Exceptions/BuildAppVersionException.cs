/***************************************************************

 *  类名称：        BuildAppVersionException

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/8 10:49:53

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;

namespace CenturyGame.AppBuilder.Runtime.Exceptions
{
    public class BuildAppVersionException : Exception
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

        public BuildAppVersionException(string targetVersion , string lastVersion) 
            : base($"Target version : {targetVersion}  lastVersion : {lastVersion} , pleause to config your target" +
                   $" verison , it is need to big or equal your last build version!")
        {

        }


        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        #endregion

    }
}
