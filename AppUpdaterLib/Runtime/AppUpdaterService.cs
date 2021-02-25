/***************************************************************

 *  类名称：        HotfixService

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/23 15:23:54

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using UnityEngine;
using System;
using CenturyGame.AppUpdaterLib.Runtime.Diagnostics;
using CenturyGame.AppUpdaterLib.Runtime.Interfaces;
using CenturyGame.AppUpdaterLib.Runtime.Managers;
using CenturyGame.Core.FSM;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    internal sealed class AppUpdaterService : MonoBehaviour
    {
        #region Fields


        private AppUpdaterFsmOwner mOwner;

        private IStorageInfoProvider _mStorageInfoProvider = null;

        #endregion


        #region Properties And Events

        public AppUpdaterContext Context => mOwner?.Context;

        #endregion

        #region Init

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            this.InitializeComponent();
        }

        public void InitializeComponent()
        {
            this.CreateFSMOwner();
            this.InitOthers();
        }

        private void CreateFSMOwner()
        {
            this.mOwner = new AppUpdaterFsmOwner();
            this.mOwner.Init();
        }

        private void InitOthers()
        {
#if DEBUG_APP_UPDATER
            this.gameObject.AddComponent<AppUpdaterDebugView>();
            Context.FetchDeviceStorageInfo();
#endif
        }

        #endregion

        #region Set Callbacks

        public void SetErrorCallback(AppUpdaterErrorCallback callback)
        {
            this.mOwner.SetErrorCallback(callback);
        }

        public void SetServerMaintenanceCallback(AppUpdaterServerMaintenanceCallback callback)
        {
            this.mOwner.SetServerMaintenanceCallback(callback);
        }

        public void SetForceUpdateCallback(AppUpdaterForceUpdateCallback callback)
        {
            this.mOwner.SetForceUpdateCallback(callback);
        }

        public void SetOnTargetVersionObtainCallback(AppUpdaterOnTargetVersionObtainCallback callback)
        {
            this.mOwner.SetOnTargetVersionObtainCallback(callback);
        }

        public void SetPerformCompletedCallback(AppUpdaterPerformCompletedCallback callback)
        {
            this.mOwner.SetPerformCompletedCallback(callback);
        }

        #endregion

        public void SetAppUpdaterRequester(IAppUpdaterRequester requester)
        {
            Context.Requester = requester;
        }

        public void SetStorageInfoProvider(IStorageInfoProvider provider)
        {
            this._mStorageInfoProvider = provider;
            if (this.Context != null)
            {
                this.Context.StorageInfoProvider = provider;
            }
        }

        public void ManualStartAppUpdate()
        {
            IRoutedEventArgs arg = new RoutedEventArgs()
            {
                EventType = (int)AppUpdaterInnerEventType.PerformAppUpdate
            };
            this.mOwner.HandleMessage(in arg);
        }

        public void StartUpdateAgain()
        {
            this.mOwner.StartUpdateOperationAgain();

            if (AppUpdaterHints.Instance.ManualPerformAppUpdate)
            {
                this.ManualStartAppUpdate();
            }
        }

        public bool IsSucceed()
        {
            return this.mOwner.State == AppUpdaterFsmOwner.AppUpdaterState.Done;
        }

        #region Unity Callbacks

        public void Update()
        {
            this.mOwner?.Update();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            IRoutedEventArgs arg = new RoutedEventArgs<bool>
            {
                EventType = (int)AppUpdaterInnerEventType.OnApplicationFocus,
                arg = hasFocus
            };
            this.mOwner.HandleMessage(in arg);
        }

        private void OnApplicationQuit()
        {
            IRoutedEventArgs arg = new RoutedEventArgs
            {
                EventType = (int)AppUpdaterInnerEventType.OnApplicationQuit,
            };
            this.mOwner.HandleMessage(in arg);
        }

        #endregion
    }
}
