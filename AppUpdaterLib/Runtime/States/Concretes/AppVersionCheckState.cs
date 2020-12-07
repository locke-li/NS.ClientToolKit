/***************************************************************

 *  类名称：          VersionCheckState

 *  描述：            App及资源版本校验

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 18:34:10

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System.Collections.Generic;
using CenturyGame.AppUpdaterLib.Runtime.Managers;
using CenturyGame.AppUpdaterLib.Runtime.ResManifestParser;
using CenturyGame.Core.FSM;

namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    sealed class AppVersionCheckState : BaseAppUpdaterFunctionalState
    {
        #region Enums

        public enum InnerState
        {
            Idle,

            CheckMaintence,

            CheckingMaintence,

            CheckAppForceUpdate,

            CheckingAppForceUpdate,

            CheckResUpdate,

            CheckResVersionUpdateFailure,

            CheckResUpdateing,

            VersionCheckCompleted,
        }

        #endregion

        private InnerState mState = InnerState.Idle;

        public override void Enter(AppUpdaterFsmOwner entity, params object[] args)
        {
            base.Enter(entity, args);

            this.mState = InnerState.CheckMaintence;
        }


        public override void Execute(AppUpdaterFsmOwner entity)
        {
            base.Execute(entity);

            switch (this.mState)
            {
                case InnerState.CheckMaintence:
                    this.StartCheckMaintence();
                    break;
                case InnerState.CheckAppForceUpdate:
                    this.StartCheckAppForceUpdate();
                    break;
                case InnerState.CheckResUpdate:
                    this.StartCheckUpdateRes();
                    break;
                case InnerState.CheckResVersionUpdateFailure:
                    this.OnResVersionCheckFailure();
                    break;
                default:
                    break;
            }

        }

        private void StartCheckMaintence()
        {
            Logger.Info("Start check maintence !");
            this.mState = InnerState.CheckingMaintence;
            if (Context.GetVersionResponseInfo.maintenance)//服务器维护中
            {
                Logger.Info("The server is in maintence .");
                this.Target.AusmCallback?.Invoke(AppVersionManager.LHConfig.GetMaintenanceInfo());
                Context.AppendInfo("The server is in maintence .");
            }
            else
            {
                Logger.Info("The server is not in maintence , enter to check app forece upadte .");
                this.mState = InnerState.CheckAppForceUpdate;
            }
        }


        private void StartCheckAppForceUpdate()
        {
            Logger.Info("Start check app force update !");
            this.mState = InnerState.CheckingAppForceUpdate;

            //是否强更目前通过讨论由服务器返回的字段来决定
            if (Context.GetVersionResponseInfo.forceUpdate)
            {
                Logger.Info(" The current app client is too old , call app update function!");
                this.Target.AufuCallback?.Invoke(AppVersionManager.LHConfig.UpdateData);
                Context.AppendInfo(" The current app client is too old , call app update function!");
            }
            else
            {
                Logger.Info("Enter to check resupdate state !");
                this.mState = InnerState.CheckResUpdate;
            }
        }


        private void StartCheckUpdateRes()
        {
            Logger.Info("Start to check update resource !");
            this.mState = InnerState.CheckResUpdateing;

            if (string.IsNullOrEmpty(Context.GetVersionResponseInfo.update_detail.DataVersion))
            {

                Context.ErrorType = AppUpdaterErrorType.RequestDataResVersionFailure;
                this.mState = InnerState.CheckResVersionUpdateFailure;

                return;
            }

#if UNITY_EDITOR_WIN || (UNITY_ANDROID && !UNITY_EDITOR)
            if (string.IsNullOrEmpty(Context.GetVersionResponseInfo.update_detail.AndroidVersion))
#elif UNITY_EDITOR_OSX || (UNITY_IPHONE && !UNITY_EDITOR)
            if (string.IsNullOrEmpty(Context.GetVersionResponseInfo.update_detail.IOSVersion))
#endif
            {
                Context.ErrorType = AppUpdaterErrorType.RequestUnityResVersionFailure;
                this.mState = InnerState.CheckResVersionUpdateFailure;
                return;
            }
            
            List<string> resList = new List<string>();
            List<string> resVersionList = new List<string>();
            List<string> localResVersionList = new List<string>();
            List<string> localResFileNameList = new List<string>();
            List<BaseResManifestParser> parsers = new List<BaseResManifestParser>();

            if (AppVersionManager.AppInfo.dataResVersion !=
                Context.GetVersionResponseInfo.update_detail.DataVersion)
            {
                resList.Add(Context.GetCurDataResManifestName(Context.GetVersionResponseInfo.update_detail.DataVersion));
                resVersionList.Add(Context.GetVersionResponseInfo.update_detail.DataVersion);
                localResVersionList.Add(AppVersionManager.AppInfo.dataResVersion);
                localResFileNameList.Add(AssetsFileSystem.AppDataResManifestName);
                parsers.Add(new DataResManifestParser());
            }
#if UNITY_EDITOR_WIN || (UNITY_ANDROID && !UNITY_EDITOR)
            if (AppVersionManager.AppInfo.unityDataResVersion !=
                Context.GetVersionResponseInfo.update_detail.AndroidVersion)
            {
                resList.Add(Context.GetCurUnityResManifestName(
                    Context.GetVersionResponseInfo.update_detail.AndroidVersion));
                resVersionList.Add(Context.GetVersionResponseInfo.update_detail.AndroidVersion);
                localResVersionList.Add(AppVersionManager.AppInfo.unityDataResVersion);
                localResFileNameList.Add(AssetsFileSystem.UnityResManifestName);
                parsers.Add(new UnityResManifestParser());
            }

#elif UNITY_EDITOR_OSX || (UNITY_IPHONE && !UNITY_EDITOR)
            if (AppVersionManager.AppInfo.unityDataResVersion !=
                    Context.GetVersionResponseInfo.update_detail.IOSVersion)
                {
                    resList.Add(Context.GetCurUnityResManifestName(
                        Context.GetVersionResponseInfo.update_detail.IOSVersion));
                    resVersionList.Add(Context.GetVersionResponseInfo.update_detail.IOSVersion);
                    localResVersionList.Add(AppVersionManager.AppInfo.unityDataResVersion);
                    localResFileNameList.Add(AssetsFileSystem.UnityResManifestName);
                    parsers.Add(new UnityResManifestParser());
                }
#endif
            Context.ResVersions = resList.ToArray();
            Context.ResVersionNums = resVersionList.ToArray();
            Context.LocalResVersionNums = localResVersionList.ToArray(); 
            Context.ResVersionParsers = parsers.ToArray();
            Context.LocalResFiles = localResFileNameList.ToArray();
            this.StartUpdateRes();
        }

        private void StartUpdateRes()
        {
            if (Context.ResVersions.Length == 0)
            {
                Context.AppendInfo("The game resource is not change , enter game direct!");
                Logger.Info("The game resource is not change , enter game direct!");
                this.Target.ChangeState<AppUpdateCompletedState>();
            }
            else
            {
                Context.AppendInfo($"The resource group that you needed to update has {Context.ResVersions.Length} files to update , startup update the game resource . ");
                IRoutedEventArgs arg = new RoutedEventArgs()
                {
                    EventType = (int)AppUpdaterInnerEventType.StartPerformResUpdateOperation
                };
                this.Target.HandleMessage(in arg);
            }

            this.mState = InnerState.VersionCheckCompleted;
        }

        private void OnResVersionCheckFailure()
        {
            this.Target.ChangeState<AppUpdateFailureState>();
        }
    }
}
