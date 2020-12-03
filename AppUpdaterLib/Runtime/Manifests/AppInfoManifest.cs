/***************************************************************

 *  类名称：        AppVersion

 *  描述：			本地App版本信息

 *  作者：           Chico(wuyuanbing)

 *  创建时间：      2020/5/13 16:39:07

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Security;

namespace CenturyGame.AppUpdaterLib.Runtime.Manifests
{
    [Serializable]
    public class AppInfoManifest
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        /// <summary>
        /// 当前app的Version
        /// </summary>
        public string version = "";

        /// <summary>
        /// 表数据版本
        /// </summary>
        public string dataResVersion;

        /// <summary>
        /// unity资源版本
        /// </summary>
        public string unityDataResVersion;

        /// <summary>
        /// 渠道
        /// </summary>
        public string channel;

        /// <summary>
        /// 目标平台
        /// </summary>
        public string TargetPlatform;

        #endregion

        //--------------------------------------------------------------

        #region Properties & Events

        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------

        #region Creation & Cleanup

        //--------------------------------------------------------------

        public AppInfoManifest()
        {
            
        }

        #endregion

        //--------------------------------------------------------------

        #region Methods

        //--------------------------------------------------------------

        #endregion

    }
}
