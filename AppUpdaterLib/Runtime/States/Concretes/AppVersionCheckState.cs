﻿/***************************************************************

 *  类名称：          VersionCheckState

 *  描述：            App及资源版本校验

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 18:34:10

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using CenturyGame.AppUpdaterLib.Runtime.Managers;
using CenturyGame.AppUpdaterLib.Runtime.Manifests;
using CenturyGame.AppUpdaterLib.Runtime.ResManifestParser;
using CenturyGame.Core.FSM;
using UnityEngine;

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
                this.Target.OnMaintenanceCallBack(AppVersionManager.LHConfig.GetMaintenanceInfo());
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
                this.Target.OnForceUpdateCallBack(AppVersionManager.LHConfig.UpdateData);
                Context.AppendInfo(" The current app client is too old , call app update function!");
            }
            else
            {
                Logger.Info("Enter to check resupdate state !");
                this.mState = InnerState.CheckResUpdate;
            }
        }


        private bool CheckUpdateDetailValid()
        {
            if (string.IsNullOrEmpty(Context.GetVersionResponseInfo.update_detail.DataVersion))
            {
                Context.ErrorType = AppUpdaterErrorType.RequestDataResVersionFailure;
                Logger.Error($"DataVersion is null!");
                return false;
            }

            string unityDataResVersion = Context.GetVersionResponseInfo.update_detail.ResVersion;
            Logger.Info($"EnableTableDataUpdate : {AppUpdaterHints.Instance.EnableTableDataUpdate }.");
            if (AppUpdaterHints.Instance.EnableTableDataUpdate && string.IsNullOrEmpty(Context.GetVersionResponseInfo.update_detail.DataVersion))
            {
                Context.ErrorType = AppUpdaterErrorType.RequestUnityResVersionFailure;

                Logger.Error($"unityDataResVersion is null!");
                return false;
            }

            try
            {
                var revision = System.Convert.ToInt32(Context.GetVersionResponseInfo.update_detail.ResVersionNum);
                var version = new Version(AppVersionManager.AppInfo.version);
                var localRevision = version.PatchNum;
                if (localRevision > revision)
                {
                    Context.ErrorType = AppUpdaterErrorType.RequestAppRevisionNumIsSmallToLocal;
                    Logger.Error($"revision : {revision} , local revision : {localRevision} .");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error msg : {e.Message} , \n  stackTrace : \n {e.StackTrace}");
                Context.ErrorType = AppUpdaterErrorType.RequestAppRevisionNumFailure;
                return false;
            }

            return true;
        }


        private void StartCheckUpdateRes()
        {
            Logger.Info("Start to check update resource !");
            this.mState = InnerState.CheckResUpdateing;

            if (!CheckUpdateDetailValid())
            {
                this.mState = InnerState.CheckResVersionUpdateFailure;
                return;
            }
            
            Context.ResUpdateTarget.TargetResVersionNum = Context.GetVersionResponseInfo.update_detail.ResVersionNum;
            string unityDataResVersion = Context.GetVersionResponseInfo.update_detail.ResVersion;
            List<VersionDesc> descs = null;
            if (AppVersionManager.AppInfo.unityDataResVersion != unityDataResVersion)
            {
                descs = new List<VersionDesc>();
                VersionDesc info = new VersionDesc
                {
                    Type = UpdateResourceType.NormalResource,
                    LocalMd5 = AppVersionManager.AppInfo.unityDataResVersion,
                    RemoteMd5 = unityDataResVersion,
                };
                Logger.Info($"Add Descs : Type : {info.Type} , LocalMd5 : {info.LocalMd5} RemoteMd5 : {info.RemoteMd5} .");
                descs.Add(info);
            }

            if (AppUpdaterHints.Instance.EnableTableDataUpdate)
            {
                var dataVersion = Context.GetVersionResponseInfo.update_detail.DataVersion;
                if (AppVersionManager.AppInfo.dataResVersion !=
                    dataVersion)
                {
                    if (descs == null)
                        descs = new List<VersionDesc>();
                    VersionDesc info = new VersionDesc
                    {
                        Type = UpdateResourceType.TableData,
                        LocalMd5 = AppVersionManager.AppInfo.dataResVersion,
                        RemoteMd5 = dataVersion
                    };
                    Logger.Info($"Add Descs : Type : {info.Type} , LocalMd5 : {info.LocalMd5} RemoteMd5 : {info.RemoteMd5} .");
                    descs.Add(info);
                }
            }

            if (descs != null)
            {
                Logger.Info($"Update round :  {descs.Count} .");
                Context.ResUpdateTarget.VersionDescs = descs.ToArray();
            }

            this.StartUpdateRes();
        }

        private void SetTargetVersionInfo()
        {
            var detail = Context.GetVersionResponseInfo.update_detail;
            var revision = System.Convert.ToInt32(detail.ResVersionNum);
            var version = new Version(AppVersionManager.AppInfo.version);
            version.Patch = revision.ToString();
            AppInfoManifest targetAppInfo = new AppInfoManifest();
            targetAppInfo.version = version.GetVersionString();
            targetAppInfo.dataResVersion = detail.DataVersion;
            targetAppInfo.TargetPlatform = Utility.GetPlatformName();
            targetAppInfo.unityDataResVersion = detail.ResVersion;
            Logger.Debug($"Target app info : \n{JsonUtility.ToJson(targetAppInfo, true)}");
            AppVersionManager.SetTargetVersion(targetAppInfo);
            this.Target.OnnTargetVersionObtainCallback(targetAppInfo.version);
        }

        private void SetUpdateMode()
        {
            if (AppUpdaterHints.Instance.EnableResIncrementalUpdate)
            {
                Context.ResUpdateConfig.Mode = ResSyncMode.LOCAL;
            }
            else
            {
                Context.ResUpdateConfig.Mode = ResSyncMode.FULL;
            }
        }

        private void StartUpdateRes()
        {
            this.SetTargetVersionInfo();
            this.SetUpdateMode();
            if (Context.IsNeedUpdate())
            {
                Context.AppendInfo("The game resource is not change , enter game direct!");
                Logger.Info("The game resource is not change , enter game direct!");
                Context.SaveAppRevision();
                this.Target.ChangeState<AppUpdateCompletedState>();
            }
            else
            {
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
