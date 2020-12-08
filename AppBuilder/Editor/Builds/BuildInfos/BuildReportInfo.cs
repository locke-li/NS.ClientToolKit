/***************************************************************

 *  类名称：        BuildReportInfo

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/21 17:15:28

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;

namespace CenturyGame.AppBuilder.Editor.Builds.BuildInfos
{
    [Serializable]
    public class BuildReportInfo
    {

        #region

        [Serializable]
        public class MetaData
        {
            public string buildTime;

            public string machineName;

            public string unityVersion;
        }

        #endregion

        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        /// <summary>
        /// 0 : 时间
        /// 1 ：app版本
        /// 2 ：unity资源版本
        /// </summary>
        public static readonly string BuildReportFileNamePattern = "{0}_{1}.report";

        public MetaData meta = new MetaData();

        public string buildTarget;

        public string unityResVerison;

        public string dataResVersion;



        public string ossUrl;
        public string cdnUrl;
        public string channel;

        public bool makeBaseVersion;

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
