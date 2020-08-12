/***************************************************************

 *  类名称：        MakeAppPatchException

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/22 15:31:37

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;

namespace CenturyGame.AppBuilder.Editor.Builds.Exceptions
{
    public class MakeAppPatchException : Exception 
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

        public MakeAppPatchException(string baseVersion, string curVersion)
            : base($"Make app patch error ! Your want to make a patch that verison is {curVersion} , but the " +
                   $"base verison is {baseVersion} !")
        {

        }

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        #endregion

    }
}
