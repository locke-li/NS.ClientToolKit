/***************************************************************

 *  类名称：        AppUpdaterManager

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/15 16:59:11

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;
using CenturyGame.AppUpdaterLib.Runtime.Interfaces;
using CenturyGame.LoggerModule.Runtime;
using System.IO;
using CenturyGame.AppUpdaterLib.Runtime.Configs;
using CenturyGame.AppUpdaterLib.Runtime.Manifests;
using UnityEngine;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;
using Object = UnityEngine.Object;

namespace CenturyGame.AppUpdaterLib.Runtime.Managers
{
    public static class AppUpdaterManager
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private static ILogger s_mLogger = LoggerManager.GetLogger("AppUpdaterManager");

        private static bool s_mInitialized = false;

        private static AppUpdaterService s_mService;

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        private static AppUpdaterConfig mAppUpdaterConfig;

        public static AppUpdaterConfig AppUpdaterConfig
        {
            get
            {
                if (mAppUpdaterConfig == null)
                {
                    var appUpdaterConfigText = Resources.Load<TextAsset>("appupdater");
                    mAppUpdaterConfig = JsonUtility.FromJson<AppUpdaterConfig>(appUpdaterConfigText.text);
                }

                return mAppUpdaterConfig;
            }   
        }

        public static string ClientUniqueId { set; get; } = string.Empty;

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public static void Init()
        {
            if (!s_mInitialized)
            {
                CreateService();
                s_mInitialized = true;
            }
        }

        private static void CreateService()
        {
            const string serviceName = "AppUpdater";
            var sersvicePfb = Resources.Load<GameObject>(serviceName);

            if (sersvicePfb == null)
            {
                throw new FileNotFoundException("AppUpdater");
            }

            var serviceGo = Object.Instantiate(sersvicePfb);
            serviceGo.name = serviceName;
            s_mService = serviceGo.GetComponent<AppUpdaterService>();
        }

        /// <summary>
        /// 注入App更新依赖的请求对象
        /// </summary>
        /// <param name="requester"></param>
        public static void AppUpdaterSetAppUpdaterRequester(IAppUpdaterRequester requester)
        {
            CheckIsInitialize("AppUpdaterSetAppUpdaterRequester");
            s_mService.SetAppUpdaterRequester(requester);
        }

        /// <summary>
        /// 设置更新模块发生错误时的回调
        /// </summary>
        /// <param name="callback"></param>
        public static void AppUpdaterSetErrorCallback(AppUpdaterErrorCallback callback)
        {
            CheckIsInitialize("AppUpdaterSetErrorCallback");
            s_mService.SetErrorCallback(callback);
        }

        /// <summary>
        /// 注册维护中回调
        /// </summary>
        /// <param name="callback"></param>
        public static void AppUpdaterSetServerMaintenanceCallback(AppUpdaterServerMaintenanceCallback callback)
        {
            CheckIsInitialize("AppUpdaterSetServerMaintenanceCallback");
            s_mService.SetServerMaintenanceCallback(callback);
        }

        /// <summary>
        /// 注册强更回调
        /// </summary>
        /// <param name="callback"></param>
        public static void AppUpdaterSetForceUpdateCallback(AppUpdaterForceUpdateCallback callback)
        {
            CheckIsInitialize("AppUpdaterSetForceUpdateCallback");
            s_mService.SetForceUpdateCallback(callback);
        }

        /// <summary>
        /// 注册目标版本信息回调
        /// </summary>
        /// <param name="callback"></param>
        public static void AppUpdaterSetOnTargetVersionObtainCallback(AppUpdaterOnTargetVersionObtainCallback callback)
        {
            CheckIsInitialize("AppUpdaterSetOnTargetVersionObtainCallback");
            s_mService.SetOnTargetVersionObtainCallback(callback);
        }

        /// <summary>
        /// 注册更新完成时回调
        /// </summary>
        /// <param name="callback"></param>
        public static void AppUpdaterSetPerformCompletedCallback(AppUpdaterPerformCompletedCallback callback)
        {
            CheckIsInitialize("AppUpdaterSetPerformCompletedCallback");
            s_mService.SetPerformCompletedCallback(callback);
        }

        /// <summary>
        /// 设置磁盘信息获取提供者
        /// </summary>
        /// <param name="provider"></param>
        public static void AppUpdaterSetStorageInfoProvider(IStorageInfoProvider provider)
        {
            CheckIsInitialize("SetStorageInfoProvider");
            s_mService.SetStorageInfoProvider(provider);
        }

        /// <summary>
        /// 获取热更新进度数据
        /// </summary>
        /// <returns></returns>
        public static AppUpdaterProgressData AppUpdaterGetAppUpdaterProgressData()
        {
            CheckIsInitialize("GetAppUpdaterProgressData");
            return AppUpdaterContext.Current.ProgressData;
        }

#if DEBUG_APP_UPDATER
        /// <summary>
        /// 
        /// </summary>
        public static void AppUpdaterStartUpdateAgain()
        {
            CheckIsInitialize("AppUpdaterStartUpdateAgain");
            s_mService.StartUpdateAgain();
        }
#endif

        /// <summary>
        /// 打开商店
        /// </summary>
        public static void OpenAppStore()
        {
            var currentLhConfig = AppVersionManager.LHConfig;
            if (!string.IsNullOrEmpty(currentLhConfig.UpdateData.AppStoreUrl))
            {
                Application.OpenURL(currentLhConfig.UpdateData.AppStoreUrl);
            }
            else
            {
                if (!string.IsNullOrEmpty(currentLhConfig.UpdateData.PackageUrl))
                {
                    Application.OpenURL(currentLhConfig.UpdateData.PackageUrl);
                }
                else
                {
                    s_mLogger.Error($"Current lighthouse config is invalid! Lighthouse id : {currentLhConfig.MetaData.lighthouseId}");
                }
            }
        }

        /// <summary>
        /// 获取当前服务器配置数据
        /// </summary>
        /// <returns></returns>
        public static LighthouseConfig.Server AppUpdaterGetServerData()
        {
            CheckIsInitialize("GetServerData");
            if(AppVersionManager.LHConfig == null)
                throw new InvalidOperationException("Get server config data failure !");
            return AppVersionManager.LHConfig.GetCurrentServerData();
        }

        /// <summary>
        /// 获取当前App信息清单
        /// </summary>
        /// <returns></returns>
        public static AppInfoManifest AppUpdaterGetAppInfoManifest()
        {
            CheckIsInitialize("GetAppInfoManifest");
            return AppVersionManager.AppInfo;
        }


        /// <summary>
        /// 获取LighthouseConfig清单
        /// </summary>
        /// <returns></returns>
        public static LighthouseConfig AppUpdaterGetLHConfig()
        {
            CheckIsInitialize("GetLHConfig");
            return AppVersionManager.LHConfig;
        }

        /// <summary>
        /// 获取渠道名
        /// </summary>
        /// <returns>返回渠道</returns>
        public static string AppUpdaterGetChannel()
        {
            CheckIsInitialize("AppUpdaterGetChannel");
            return AppVersionManager.Channel;
        }

        /// <summary>
        /// 是否更新成功
        /// </summary>
        /// <returns></returns>
        public static bool AppUpdaterIsSucceed()
        {
            if (s_mService == null)
                return false;
            return s_mService.IsSucceed();
        }

        /// <summary>
        /// 获取服务器Url
        /// </summary>
        /// <returns></returns>
        public static string AppUpdaterGetServerUrl()
        {
            CheckIsInitialize("GetServerUrl");
            return AppVersionManager.ServerUrl;
        }

        /// <summary>
        /// 设置示意，每种类型的示意都有对应的数值范围
        /// </summary>
        /// <param name="hintName">示意类型</param>
        /// <param name="hintVal">值</param>
        public static void AppUpdaterHint(AppUpdaterHintName hintName , int hitVal)
        {
            AppUpdaterHints.Instance.SetHintValue(hintName,hitVal);
        }

        public static void ManualStartAppUpdate()
        {
            CheckIsInitialize("ManualStartAppUpdate");
            s_mService.ManualStartAppUpdate();
        }

        private static void CheckIsInitialize(string methodName)
        {
            if (!s_mInitialized)
                throw new NullReferenceException($"Your want to use \"AppUpdaterManager\" that not initialized ! Call method :  \"{methodName}\" .");
        }


        public static void AppUpdaterDisposeAppUpdaterService()
        {
            if (s_mService != null)
            {
                Object.Destroy(s_mService.gameObject);
            }

            s_mInitialized = false;
        }

        #endregion

    }
}
