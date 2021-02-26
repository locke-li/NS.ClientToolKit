/***************************************************************

 *  类名称：          HotFixContext

 *  描述：            热更新模块上下文

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/21 15:09:20

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Diagnostics;
using CenturyGame.AppUpdaterLib.Runtime.Managers;
using CenturyGame.AppUpdaterLib.Runtime.Utilities;
using UnityEngine;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    internal partial class AppUpdaterContext
    {
        [Conditional("DEBUG_APP_UPDATER")]
        public void AppendInfo(string info)
        {
            var Now = DateTime.Now.ToString("HH:mm:ss ");
            this.InfoStringBuilder.AppendLine($"{Now}{info}");
        }

        public string GetStreamingAssetsPath(string path)
        {
            mTempSb.Clear();

#if UNITY_ANDROID && !UNITY_EDITOR
            mTempSb.Append(Application.streamingAssetsPath);
            mTempSb.Append("/");
#elif UNITY_IPHONE && !UNITY_EDITOR
            mTempSb.Append("file://");
            mTempSb.Append(Application.streamingAssetsPath);
            mTempSb.Append("/");
#elif UNITY_EDITOR
            mTempSb.Append("file://");
            mTempSb.Append(Application.streamingAssetsPath);
            mTempSb.Append("/");
#endif

#if APPEND_PLATFORM_NAME
            mTempSb.Append(Utility.GetPlatformName());
            mTempSb.Append("/");
#endif

            mTempSb.Append(path);
            return mTempSb.ToString();
        }

        public string GetRemoteRootUrl(FileServerType fileServerType)
        {
            var appUpdaterConfig = AppUpdaterConfigManager.AppUpdaterConfig;
            var serverUrl = (fileServerType == FileServerType.CDN) ? appUpdaterConfig.cdnUrl : appUpdaterConfig.ossUrl;
            return string.IsNullOrWhiteSpace(appUpdaterConfig.remoteRoot) ? serverUrl : $"{serverUrl}/{appUpdaterConfig.remoteRoot}";
        }

        /// <summary>
        /// 获取Lighthouse 配置的url
        /// </summary>
        /// <param name="lightHouseId">Lighthouse配置的id</param>
        /// <param name="fileServerType">文件服务器的类型</param>
        /// <returns></returns>
        public string GetLighthouseUrl(string lightHouseId, FileServerType fileServerType = FileServerType.CDN)
        {
            var curTime = TimeUtility.GetCurrentTimeSeconds() / 60;
            var appUpdaterConfig = AppUpdaterConfigManager.AppUpdaterConfig;
            //var language = GetCurrentLanguageName();

            string url;

            if (!string.IsNullOrEmpty(lightHouseId))
                url = $"{GetRemoteRootUrl(fileServerType)}/{appUpdaterConfig.channel}-lighthouse.json_{lightHouseId}?v={curTime}&t={curTime}";
            else
                url = $"{GetRemoteRootUrl(fileServerType)}/{appUpdaterConfig.channel}-lighthouse.json?v={curTime}&t={curTime}";

            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCurrentVerisonFileUrl(FileServerType fileServerType = FileServerType.CDN)
        {
            if (this.ResUpdateTarget.CurrentResVersionIdx >= this.ResUpdateTarget.ResVersions.Length || this.ResUpdateTarget.CurrentResVersionIdx < 0)
            {
                throw new InvalidOperationException($"Get current version file url error ! CurrentResVersionIdx ：" +
                                                    $"{this.ResUpdateTarget.CurrentResVersionIdx}");
            }

            string url = $"{GetRemoteRootUrl(fileServerType)}/version_list/{this.ResUpdateTarget.ResVersions[this.ResUpdateTarget.CurrentResVersionIdx]}";

            return url;
        }

        public string GetCurrentVersionFileName()
        {
            if (this.ResUpdateTarget.CurrentResVersionIdx >= this.ResUpdateTarget.ResVersions.Length || this.ResUpdateTarget.CurrentResVersionIdx < 0)
            {
                return null;
            }

            return this.ResUpdateTarget.ResVersions[this.ResUpdateTarget.CurrentResVersionIdx];
        }

        public string GetCurrentLocalVersionFileName()
        {
            if (this.ResUpdateTarget.CurrentResVersionIdx >= this.ResUpdateTarget.LocalResFiles.Length || this.ResUpdateTarget.CurrentResVersionIdx < 0)
            {
                return null;
            }

            return this.ResUpdateTarget.LocalResFiles[this.ResUpdateTarget.CurrentResVersionIdx];
        }
        

        public string GetCurrentLanguageName()
        {
            var language = PlayerPrefs.GetString(CommonConst.APP_LANGUAGE_KEY, "en");

            return language;
        }

        /// <summary>
        /// 获取设备存储信息
        /// </summary>
        public void FetchDeviceStorageInfo()
        {
            if (this.StorageInfoProvider != null)
            {
                this.DiskInfo.AvailableSpace = this.StorageInfoProvider.GetAvailableDiskSpace();
                this.DiskInfo.BusySpace = this.StorageInfoProvider.GetBusyDiskSpace();
                this.DiskInfo.TotalSpace = this.StorageInfoProvider.GetTotalDiskSpace();
                this.DiskInfo.time = DateTime.Now;
                this.DiskInfo.IsGetReady = true;
            }
        }

        /// <summary>
        /// 保存当前Revision信息
        /// </summary>
        public void SaveAppRevision()
        {
            if (!string.IsNullOrEmpty(this.ResUpdateTarget.TargetResVersionNum))
            {
                Version version = new Version(AppVersionManager.AppInfo.version);
                version.Patch = this.ResUpdateTarget.TargetResVersionNum;
                AppVersionManager.AppInfo.version = version.GetVersionString();
            }
            AppVersionManager.SaveCurrentAppInfo();
        }


        private string GetCurUnityResManifestName(string version)
        {
            string manifestName = $"res_{Utility.GetPlatformName().ToLower()}.json{CommonConst.WellNumUtf8}{version}";

            return manifestName;
        }

        private string GetCurDataResManifestName(string version)
        {
            string manifestName = $"res_data.json{CommonConst.WellNumUtf8}{version}";
            return manifestName;
        }

        public string GetResManifestName(UpdateResourceType type , string version)
        {
            if (type == UpdateResourceType.UnKnow)
            {
                throw new ArgumentException($"Method : AppUpdaterContext.GetResManifestName type : {type} .");
            }
            else if(type == UpdateResourceType.TableData)
            {
                return this.GetCurDataResManifestName(version);
            }
            else
            {
                return this.GetCurUnityResManifestName(version);
            }
        }

        public bool HasResNeedUpdate()
        {
            return this.ResUpdateTarget.ResVersions.Length == 0;
        }

    }

}
