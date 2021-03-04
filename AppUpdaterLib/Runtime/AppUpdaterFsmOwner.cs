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
using CenturyGame.AppUpdaterLib.Runtime.ResManifestParser;
using CenturyGame.AppUpdaterLib.Runtime.States.Concretes;
using CenturyGame.Core.FSM;

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
            public AppUpdaterOnTargetVersionObtainCallback OnTargetVersionObtainCallback;
            public AppUpdaterPerformCompletedCallback PerformCompletedCallback;
        }

        #region

        public AppUpdaterState State = AppUpdaterState.Idle;

        private AppUpdaterCallBacks mCallBacks = new AppUpdaterCallBacks();

        private AppUpdaterFileUpdateRuleFilter _fileUpdateRuleFilter = null;
        public AppUpdaterFileUpdateRuleFilter FileUpdateRuleFilter => _fileUpdateRuleFilter;

        /// <summary>
        /// 资源的保留目录，此目录在app覆盖安装后不会删除，用于多文件环境下的游戏中的更新
        /// </summary>
        private string _retainedDataFolderName = string.Empty;
        public string RetainedDataFolderName => _retainedDataFolderName;

        private static readonly UnityResManifestParser UnityManifestParser = new UnityResManifestParser();

        private static readonly DataResManifestParser DataResManifestParser = new DataResManifestParser(); 

        #region Inner FSM

        private AppUpdaterContext _mContext = new AppUpdaterContext();
        public AppUpdaterContext Context => _mContext;

        private StateMachine<AppUpdaterFsmOwner> mFSM;
        public StateMachine<AppUpdaterFsmOwner> FSM => mFSM;

        #endregion

        public HttpRequest Request { set; get; } = new HttpRequest();

        #endregion

        public void Init()
        {
            this.CreateFsm();
        }

        private void CreateFsm()
        {
            this.mFSM = new StateMachine<AppUpdaterFsmOwner>(this);
        }

        public bool HandleMessage(in IRoutedEventArgs msg)
        {
            return this.mFSM.HandleMessage(in msg);
        }

        public void ChangeState<T>() where T : State<AppUpdaterFsmOwner>,new() 
        {
            this.mFSM.ChangeState<T>();
        }

        private void InitializeFsm()
        {
            this.mFSM.SetCurrentState<AppUpdateInitState>();
            this.mFSM.SetGlobalState<AppUpdaterGlobalState>();
        }

        public void Update()
        {
            this.Request.Update();
            mFSM?.Update();
        }

        public void StartupFsm()
        {
            if (_mContext.IsFirstRun)
            {
                this.InitializeFsm();
                _mContext.IsFirstRun = false;
            }
            else
            {
                this.mFSM.ChangeState<AppUpdateInitState>();
            }
        }

        public void StartUpdateOperationAgain() 
        {
            this.Clear();
            Context.AppendInfo("Start app update operation again !");
            this.StartupFsm();

            if (AppUpdaterHints.Instance.ManualPerformAppUpdate)
            {
                this.ManualStartAppUpdate();
            }
        }


        public void BindFileUpdateRuleFilter(AppUpdaterFileUpdateRuleFilter filter)
        {
            this._fileUpdateRuleFilter = filter;
        }

        public void UnBindFileUpdateRuleFilter()
        {
            this._fileUpdateRuleFilter = null;
        }

        
        public void SetRetainedDataFolderName(string name)
        {
            this._retainedDataFolderName = name;
        }

        public void StartDownloadPartialDataRes()
        {
            this.Clear();
            Context.AppendInfo("Start resource partial update operation again !");
            IRoutedEventArgs arg = new RoutedEventArgs()
            {
                EventType = (int)AppUpdaterInnerEventType.StartPerformResPartialUpdateOperation
            };
            this.HandleMessage(in arg);
        }

        public void ManualStartAppUpdate()
        {
            IRoutedEventArgs arg = new RoutedEventArgs()
            {
                EventType = (int)AppUpdaterInnerEventType.PerformAppUpdate
            };
            this.HandleMessage(in arg);
        }


        public BaseResManifestParser GetResManifestParserByType(UpdateResourceType type)
        {
            if (type == UpdateResourceType.TableData)
            {
                return DataResManifestParser;
            }
            else if (type == UpdateResourceType.NormalResource)
            {
                return UnityManifestParser;
            }

            throw new ArgumentException($"UpdateResourceType : {type}");
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

        public void SetOnTargetVersionObtainCallback(AppUpdaterOnTargetVersionObtainCallback callback)
        {
            mCallBacks.OnTargetVersionObtainCallback = callback;
        }

        public void SetPerformCompletedCallback(AppUpdaterPerformCompletedCallback callback)
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


        public void OnnTargetVersionObtainCallback(string version)
        {
            this.mCallBacks.OnTargetVersionObtainCallback?.Invoke(version);
        }


        public void OnCompletedCallback()
        {
            this.State = AppUpdaterState.Done;
            this.mCallBacks.PerformCompletedCallback?.Invoke();
        }

        #endregion

    }
}
