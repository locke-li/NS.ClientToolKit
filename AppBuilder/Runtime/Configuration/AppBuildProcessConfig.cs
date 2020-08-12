/***************************************************************

 *  类名称：        AppBuildConfig

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/7 17:43:47

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System.Collections.Generic;

namespace CenturyGame.AppBuilder.Runtime.Configuration
{

    public class AppBuildActionInfo
    {
        public bool IsActionQueue { set; get; }

        public string TypeFullName { set; get; }

        public List<AppBuildActionInfo> Childs { set; get; } = new List<AppBuildActionInfo>();
    }

    public class AppBuildFilterInfo
    {
        public string TypeFullName { set; get; }

        public AppBuildActionInfo Action { set; get; } = new AppBuildActionInfo();
    }


    public class AppBuildProcessConfig
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------
        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------
        public List<AppBuildFilterInfo> Filters { set; get; } = new List<AppBuildFilterInfo>();

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
