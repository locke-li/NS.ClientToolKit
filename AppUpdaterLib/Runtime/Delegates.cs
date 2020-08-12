/***************************************************************

 *  类名称：        Delegates

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/15 17:03:55

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using CenturyGame.AppUpdaterLib.Runtime.Configs;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    /// <summary>
    /// 更新模块发生错误时回调
    /// </summary>
    /// <param name="errorType"></param>
    /// <param name="desc"></param>
    public delegate void AppUpdaterErrorCallback(AppUpdaterErrorType errorType , string desc);

    /// <summary>
    /// “服务器维护中”的回调函数
    /// </summary>
    /// <param name="errorType"></param>
    /// <param name="desc"></param>
    public delegate void AppUpdaterServerMaintenanceCallback(LighthouseConfig.MaintenanceInfo maintenanceInfo);

    /// <summary>
    /// 强制更新App的回调函数
    /// </summary>
    public delegate void AppUpdaterForceUpdateCallback(LighthouseConfig.UpdateDataInfo info);

    /// <summary>
    /// 热更新模块执行完成回调
    /// </summary>
    public delegate void AppUpdaterPerformCompleted();
}
