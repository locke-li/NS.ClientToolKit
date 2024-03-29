/***************************************************************

 *  类名称：        AppUpdaterConfig

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/13 10:48:21

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;

namespace CenturyGame.AppUpdaterLib.Runtime.Configs
{
    [Serializable]
    public class AppUpdaterConfig
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------


        #region 游戏更新使用的相关URL

        /*
         * CDN是资源文件访问的主要途径，如果cdnUrl访问失败，则会去Oss上访问，如果还是访问失败，则访问失败
         */

        /// <summary>
        /// app启动热更新时访问的远程cdn链接url
        /// </summary>
        public string cdnUrl = "";

        /// <summary>
        /// app启动热更新时备用的oss链接url
        /// </summary>
        public string ossUrl = "";

        /// <summary>
        /// 渠道名
        /// </summary>
        public string channel = "";

        /// <summary>
        /// 远端根目录
        /// </summary>
        public string remoteRoot = "";

        /// <summary>
        /// 跳过热更新
        /// </summary>
        public bool skipAppUpdater = false;

        #endregion


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
