/***************************************************************

 *  类名称：        IAppUpdaterRequester

 *  描述：		    用于与http服务器交互

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/19 16:18:04

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;
using CenturyGame.AppUpdaterLib.Runtime.Configs;
using CenturyGame.AppUpdaterLib.Runtime.Protocols;

namespace CenturyGame.AppUpdaterLib.Runtime.Interfaces
{
    public interface IAppUpdaterRequester
    {
        void Update();
        void ReqGetVersion(LighthouseConfig.Server serverData, string appVersion, string lighthouseId,
            FileServerType fromTo, Action<GetVersionResponseInfo> getVersionResponseInfoAction);

    }
}
