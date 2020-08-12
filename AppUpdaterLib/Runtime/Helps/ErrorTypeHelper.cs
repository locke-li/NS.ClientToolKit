/***************************************************************

 *  类名称：        ErrorTypeHelper

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/20 16:18:49

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

namespace CenturyGame.AppUpdaterLib.Runtime.Helps
{
    internal class ErrorTypeHelper
    {
        public static string GetErrorString(AppUpdaterErrorType type)
        {
            switch (type)
            {
                case AppUpdaterErrorType.LoadBuiltinAppInfoFailure:
                    return "Load builtin appinfo.x failure!";
                case AppUpdaterErrorType.ParseBuiltinAppInfoFailure:
                    return "Parse builtin appinfo.x failure !";
                case AppUpdaterErrorType.ParseLocalAppInfoFailure:
                    return "Parse local appinfo.x failure !";
                case AppUpdaterErrorType.RequestLighthouseFailure:
                    return "Request lighthouse config failure!";
                case AppUpdaterErrorType.ParseLighthouseConfigError:
                    return "Parser lighthouse config error!";
                case AppUpdaterErrorType.RequestGetVersionFailure:
                    return "Request verison check data from remote http serve failure!";
                case AppUpdaterErrorType.RequestResManifestFailure:
                    return "Request resource manifest that from remote file server failure!";
                case AppUpdaterErrorType.DownloadFileFailure:
                    return "Download file that from remote file server failure!";
                case AppUpdaterErrorType.ParseLocalResManifestFailure:
                    return "Parse local resource manifest failure!";
                case AppUpdaterErrorType.ParseRemoteResManifestFailure:
                    return "Parse remote resource manifest failure!";
                case AppUpdaterErrorType.RequestDataResVersionFailure:
                    return "Request table data resource version failure!";
                case AppUpdaterErrorType.RequestUnityResVersionFailure:
                    return "Request unity resource data version failure!";
                case AppUpdaterErrorType.DiskIsNotEnoughToDownPatchFiles:
                    return "The disk available space is not enough to download path files !";
                default:
                    return "Error ! Unknow appupdater error! ";
            } 
        }
    }
}
