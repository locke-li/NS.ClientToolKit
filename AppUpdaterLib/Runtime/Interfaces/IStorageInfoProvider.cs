/***************************************************************

 *  类名称：        IStorageInfoProvider

 *  描述：			存储信息获取，该API目前只针对移动平台设计

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/29 17:06:36

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

namespace CenturyGame.AppUpdaterLib.Runtime.Interfaces
{
    public interface IStorageInfoProvider
    {
        int GetAvailableDiskSpace();

        int GetTotalDiskSpace();

        int GetBusyDiskSpace();
    }
}
