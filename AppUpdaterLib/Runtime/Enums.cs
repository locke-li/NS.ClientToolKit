/***************************************************************

 *  类名称：          Enums

 *  描述：            用于定义热更相关枚举类型

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/21 12:11:53

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


namespace CenturyGame.AppUpdaterLib.Runtime
{

    public enum AppUpdaterInnerEventType
    {
        StartPerformResUpdateOperation,

        OnCurrentResUpdateCompleted,

        PerformResUpdateOperation,

        OnApplicationFocus,

        OnApplicationQuit,
    }


    public enum FileServerType
    {
        CDN,
        OSS,
    }


    public enum AppUpdaterErrorType
    {
        None,

        LoadBuiltinAppInfoFailure,//加载内建AppInfo配置失败

        ParseBuiltinAppInfoFailure,//解析StreamingAssets目录下的appInfo失败

        ParseLocalAppInfoFailure,//解析本地appinfo失败

        DownloadLighthouseFailure,//下载lighthouse配置失败

        ParseLighthouseConfigError,//解析lighthouse配置失败

        LighthouseConfigCheckInvalid,//无效的lighthouse配置

        DownloadLighthouseConfigInvalid,//下载lighthouse配置无效

        RequestGetVersionFailure,//EntryPointRequest请求返回失败

        RequestDataResVersionFailure,//返回的数据表资源版本为空

        RequestUnityResVersionFailure,//返回的Unity资源版本为空

        RequestAppRevisionNumIsSmallToLocal,//返回的修订号小于当前修订号

        RequestAppRevisionNumFailure,//返回的Unity资源修订号不合理

        RequestResManifestFailure,//请求清单文件失败

        ParseLocalResManifestFailure,//解析本地清单文件失败

        ParseRemoteResManifestFailure,//解析远端清单文件失败

        DownloadFileFailure,//文件下载失败

        DiskIsNotEnoughToDownPatchFiles,//磁盘空间不足

        Unknow,
    }

    /// <summary>
    /// 资源更新类型
    /// </summary>
    public enum UpdateResourceType
    {
        UnKnow,

        TableData,

        NormalResource,
    }


    public enum AppUpdaterHintName
    {
        LOWER_LUA_NAME,//lua路径小写，解决针对ios大小写敏感相关问题
    }

    public enum AppUpdaterBool
    {
        FALSE,
        TRUE
    }

}