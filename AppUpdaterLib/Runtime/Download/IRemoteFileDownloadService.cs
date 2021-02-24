/***************************************************************

 *  类名称：        IRemoteFileDownloadService

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2021/2/24 11:26:21

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/


// ReSharper disable once CheckNamespace
namespace CenturyGame.AppUpdaterLib.Runtime.Download
{
    public interface IRemoteFileDownloadService
    {
        void SetDownloadCallBack(FileDownloadCallBack callback);

        void StartDownload(FileDesc fileDesc);
    }
}
