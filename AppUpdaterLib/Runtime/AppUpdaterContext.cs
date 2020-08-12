﻿/***************************************************************

 *  类名称：          HotFixContext

 *  描述：            热更新模块上下文

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/21 15:09:20

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using CenturyGame.AppUpdaterLib.Runtime.Configs;
using CenturyGame.AppUpdaterLib.Runtime.Utilities;
using UnityEngine;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    internal partial class AppUpdaterContext
    {
        private static AppUpdaterContext mCurrent;
        public static AppUpdaterContext Current
        {
            get
            {
                if (mCurrent == null)
                {
                    var appUpdaterConfigText = Resources.Load<TextAsset>("appupdater");
                    var appUpdaterConfig = JsonUtility.FromJson<AppUpdaterConfig>(appUpdaterConfigText.text);
                    mCurrent = new AppUpdaterContext(appUpdaterConfig);
                }
                    
                return mCurrent;
            }
        }


        public AppUpdaterContext(AppUpdaterConfig config)
        {
            this.Config = config;
        }

        public void AppendInfo(string info)
        {
#if DEBUG_APP_UPDATER
            var Now = DateTime.Now.ToString("HH:mm:ss ");
            this.InfoStringBuilder.AppendLine($"{Now}{info}");
#endif
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
#elif UNITY_STANDLONE_WIN || UNITY_EDITOR
            mTempSb.Append(Application.streamingAssetsPath);
            mTempSb.Append("/");
#endif
            mTempSb.Append(path);
            return mTempSb.ToString();
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

            var language = GetCurrentLanguageName();

            string url;

            string serverUrl = (fileServerType == FileServerType.CDN) ? this.Config.cdnUrl : this.Config.ossUrl;

            if (!string.IsNullOrEmpty(lightHouseId))
                url = $"{serverUrl}/lighthouse-{this.Config.channel}-{language}-{lightHouseId}.json?v={curTime}&t={curTime}";
            else
                url = $"{serverUrl}/lighthouse-{this.Config.channel}-{language}.json?v={curTime}&t={curTime}";

            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCurrentVerisonFileUrl(FileServerType fileServerType = FileServerType.CDN)
        {
            if (CurrentResVersionIdx >= this.ResVersions.Length || CurrentResVersionIdx < 0)
            {
                throw new InvalidOperationException($"Get current version file url error ! CurrentResVersionIdx ：{CurrentResVersionIdx}");
            }

            string serverUrl = (fileServerType == FileServerType.CDN) ? this.Config.cdnUrl : this.Config.ossUrl;
            string url = $"{serverUrl}/version_list/{this.ResVersions[CurrentResVersionIdx]}";

            return url;
        }

        public string GetCurrentVersionFileName()
        {
            if (CurrentResVersionIdx >= this.ResVersions.Length || CurrentResVersionIdx < 0)
            {
                return null;
            }

            return this.ResVersions[CurrentResVersionIdx];
        }

        public string GetCurrentLocalVersionFileName()
        {
            if (CurrentResVersionIdx >= this.LocalResFiles.Length || CurrentResVersionIdx < 0)
            {
                return null;
            }

            return this.LocalResFiles[CurrentResVersionIdx];
        }

        public string GetCurUnityResManifestName(string version)
        {
            string manifestName = string.Empty;

#if UNITY_EDITOR
            manifestName = $"res_android-{version}.json";
#elif UNITY_ANDROID && !UNITY_EDITOR
            manifestName = $"res_android-{version}.json";
#elif UNITY_IPHONE && !UNITY_EDITOR
            manifestName = $"res_ios-{version}.json";
#else
            manifestName = $"res_ios-{version}.json";
#endif

            return manifestName;
        }

        public string GetCurDataResManifestName(string version)
        {
            var platform = Application.platform;

            string manifestName = $"res_data-{version}.json";
            return manifestName;
        }

        public string GetCurrentLanguageName()
        {
            var language = PlayerPrefs.GetString(CommonConst.APP_LANGUAGE_KEY, "en");

            return language;
        }

        public string GetRemoteResFileUrl(string fileName , FileServerType fileServerType = FileServerType.CDN)
        {
            string url = null;
            switch (fileServerType)
            {
                case FileServerType.CDN:
                    url = $"{this.Config.cdnUrl}/{fileName}";
                    break;
                case FileServerType.OSS:
                    url = $"{this.Config.ossUrl}/{fileName}";
                    break;
                default:
                    break;
            }

            return url;
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

    }

}