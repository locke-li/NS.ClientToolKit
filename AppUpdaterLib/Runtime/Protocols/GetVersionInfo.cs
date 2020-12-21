/***************************************************************

 *  类名称：        HttpGetVersionInfo

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/11 10:22:19

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;

namespace CenturyGame.AppUpdaterLib.Runtime.Protocols
{
    public class ResUpdateDetail
    {
        /// <summary>
        /// 表数据版本
        /// </summary>
        public string DataVersion;

        /// <summary>
        /// 资源版本号
        /// </summary>
        public string ResVersionNum;

        /// <summary>
        /// android资源版本
        /// </summary>
        public string AndroidVersion;

        /// <summary>
        /// ios端资源版本
        /// </summary>
        public string IOSVersion;
    }

    public class GetVersionResponseInfo
    {
        /// <summary>
        /// 当前游戏服url
        /// </summary>
        public string url;

        /// <summary>
        /// lighthouse 配置id
        /// </summary>
        public string lighthouseId = string.Empty;

        /// <summary>
        /// 是否是维护中
        /// </summary>
        public bool maintenance = false;

        /// <summary>
        /// 是否强制更新app
        /// </summary>
        public bool forceUpdate = false;

        /// <summary>
        /// 资源更新详细信息
        /// </summary>
        public ResUpdateDetail update_detail;
    }
}
