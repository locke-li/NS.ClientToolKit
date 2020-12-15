/***************************************************************

 *  类名称：          HotFixOwner

 *  描述：

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 18:13:46

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using CenturyGame.AppUpdaterLib.Runtime.Configs;
using CenturyGame.AppUpdaterLib.Runtime.States.Concretes;
using CenturyGame.Core.FSM;
using CenturyGame.Core.MessengerSystem;
using UnityEngine;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    internal sealed class AppUpdaterFsmOwner : IFSMOwner
    {
        public enum AppUpdaterState
        {
            Idle,
            Runing,
            Maintenance,
            ForceUpdate,
            Error,
            Done,
        }

        internal class AppUpdaterCallBacks
        {
            public AppUpdaterErrorCallback ErrorCallback;
            public AppUpdaterServerMaintenanceCallback ServermaintenanceCallback;
            public AppUpdaterForceUpdateCallback ForceUpdateCallback;
            public AppUpdaterPerformCompleted PerformCompletedCallback;
        }

        #region

        public AppUpdaterState State = AppUpdaterState.Idle;

        private AppUpdaterCallBacks mCallBacks = new AppUpdaterCallBacks();

        #region Inner FSM

        public AppUpdaterContext Context => AppUpdaterContext.Current;

        private StateMachine<AppUpdaterFsmOwner> mFSM;
        public StateMachine<AppUpdaterFsmOwner> FSM => mFSM;

        #endregion

        public HttpRequest Request { set; get; } = new HttpRequest();

        #endregion

        public void Init()
        {
            this.InitializeComponent();
        }
            

        private void InitializeComponent() 
        {
            this.InitializeFSM();

            this.AddListeners();
        }

        private void InitializeFSM()
        {
            this.mFSM = new StateMachine<AppUpdaterFsmOwner>(this);
            this.mFSM.SetCurrentState<AppUpdateInitState>();
            this.mFSM.SetGlobalState<AppUpdaterGlobalState>();
        }

        private void AddListeners()
        {
        }

        public bool HandleMessage(in IRoutedEventArgs msg)
        {
            return this.mFSM.HandleMessage(in msg);
        }


        public void ChangeState<T>() where T : State<AppUpdaterFsmOwner>,new() 
        {
            this.mFSM.ChangeState<T>();
        }

        public void Update()
        {
            this.Request.Update();
            mFSM?.Update();
        }

        internal void StartUpdateOperationAgain() 
        {
            this.Clear();
            Context.AppendInfo("Start app update operation again !");
            this.ChangeState<AppUpdateInitState>();
        }
        
        internal void Clear()
        {
            Context?.Clear();
        }


        #region 由外部发送的事件封装


        #endregion

        #region Set Callbacks

        public void SetErrorCallback(AppUpdaterErrorCallback callback)
        {
            mCallBacks.ErrorCallback = callback;
        }

        public void SetServerMaintenanceCallback(AppUpdaterServerMaintenanceCallback callback)
        {
            mCallBacks.ServermaintenanceCallback = callback;
        }

        public void SetForceUpdateCallback(AppUpdaterForceUpdateCallback callback)
        {
            mCallBacks.ForceUpdateCallback = callback;
        }

        public void SetPerformCompletedCallback(AppUpdaterPerformCompleted callback)
        {
            mCallBacks.PerformCompletedCallback = callback;
        }

        #endregion

        #region Callbacks


        public void OnErrorCallback(AppUpdaterErrorType errorType, string desc)
        {
            this.State = AppUpdaterState.Error;
            this.mCallBacks.ErrorCallback?.Invoke(errorType, desc);
        }

        public void OnMaintenanceCallBack(LighthouseConfig.MaintenanceInfo maintenanceInfo)
        {
            this.State = AppUpdaterState.Maintenance;
            this.mCallBacks.ServermaintenanceCallback?.Invoke(maintenanceInfo);
        }

        public void OnForceUpdateCallBack(LighthouseConfig.UpdateDataInfo info)
        {
            this.State = AppUpdaterState.ForceUpdate;
            this.mCallBacks.ForceUpdateCallback?.Invoke(info);
        }

        public void OnCompletedCallback()
        {
            this.State = AppUpdaterState.Done;
            this.mCallBacks.PerformCompletedCallback?.Invoke();
        }


        #endregion

    }
}
