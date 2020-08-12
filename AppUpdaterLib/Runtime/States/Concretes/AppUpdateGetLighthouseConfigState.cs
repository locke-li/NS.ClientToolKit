/***************************************************************

 *  类名称：        AppUpdateGetLighthouseConfigState

 *  描述：		    获取更新的基础配置

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/14 11:02:12

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.IO;
using CenturyGame.AppUpdaterLib.Runtime.Configs;
using CenturyGame.AppUpdaterLib.Runtime.Managers;
using CenturyGame.AppUpdaterLib.Runtime.Manifests;
using UnityEngine;

namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    internal sealed class AppUpdateGetLighthouseConfigState : BaseAppUpdaterFunctionalState
    {
        #region Enums

        public enum LogicState
        {
            Idle,

            /// <summary>
            ///     请求Lighthouse配置
            /// </summary>
            ReqLighthouseConfig,

            /// <summary>
            ///     请求Lighthouse配置中
            /// </summary>
            RequestingLighthouseConfig,

            /// <summary>
            /// 加载app的版本信息
            /// </summary>
            LoadAppVerisonInfo,

            /// <summary>
            /// 加载app的版本失败
            /// </summary>
            LoadAppVerisonInfoFailure,

            //解析版本信息失败
            ParseBuiltinAppInfoFailure,

            //解析本地信息失败
            ParseLocalAppInfoFailure,

            /// <summary>
            /// 正在加载app版本信息
            /// </summary>
            LoadingAppVerisonInfo,

            /// <summary>
            /// 检查本地资源清单
            /// </summary>
            CheckLocalResManifest,

            CheckLocalResManifesting,

            /// <summary>
            ///     向HttpServer请求配置信息是否可靠
            /// </summary>
            ReqHttpServer,

            /// <summary>
            ///     HttpServer请求中
            /// </summary>
            RequestingHttpServer,

            /// <summary>
            ///     当前lighthouse配置较旧,下载HttpServer返回的lighthouse id对应的Lighthouse配置
            /// </summary>
            ReqLighthouseConfigAgain,

            /// <summary>
            ///     再次请求中
            /// </summary>
            RequestingLighthouseConfigAgain,

            /// <summary>
            ///     请求lighthouse配置失败
            /// </summary>
            ReqLighthouseConfigFailure,

            /// <summary>
            ///     获取lighthouse配置完成
            /// </summary>
            GetLighthouseCompleted
        }

        public enum CheckingResManifestType
        {
            UnKnow,

            DataRes,

            UnityRes,

            Done,
        }



        #endregion

        //--------------------------------------------------------------

        #region Fields

        //--------------------------------------------------------------

        private LogicState mState = LogicState.Idle;

        private CheckingResManifestType mCheckingResManifestType = CheckingResManifestType.UnKnow;

        /// <summary>
        ///     当前lighthouse 配置
        /// </summary>
        private LighthouseConfig mCurrentLighthouseConfig;

        /// <summary>
        ///     当前下载的Lighthouse文件的来源
        /// </summary>
        private FileServerType mCurLighthouseFromTo = FileServerType.CDN;

        #endregion

        //--------------------------------------------------------------

        #region Properties & Events

        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------

        #region Creation & Cleanup

        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------

        #region Methods

        //--------------------------------------------------------------


        public override void Enter(AppUpdaterFsmOwner entity, params object[] args)
        {
            base.Enter(entity, args);

            if (Context.LighthouseConfigDownloader == null)
                Context.LighthouseConfigDownloader = new LighthouseConfigDownloader(Context, Target.Request);

            mState = LogicState.ReqLighthouseConfig;
        }

        public override void Execute(AppUpdaterFsmOwner entity)
        {
            base.Execute(entity);

            Context.LighthouseConfigDownloader.Update();
            Context.Requester.Update();

            switch (mState)
            {
                case LogicState.ReqLighthouseConfig:
                    StartReqLighthouseConfig();
                    break;
                case LogicState.LoadAppVerisonInfo:
                    LoadAppVersion();
                    break;
                case LogicState.LoadAppVerisonInfoFailure:
                    OnFail(AppUpdaterErrorType.LoadBuiltinAppInfoFailure);
                    break;
                case LogicState.ParseBuiltinAppInfoFailure:
                     OnFail(AppUpdaterErrorType.ParseBuiltinAppInfoFailure);
                    break;
                case LogicState.ParseLocalAppInfoFailure:
                     OnFail(AppUpdaterErrorType.ParseLocalAppInfoFailure);
                    break;
                case LogicState.CheckLocalResManifest:
                    this.StartCheckResourceManifest();
                    break;
                case LogicState.CheckLocalResManifesting:
                    this.StartCheckingResourceManifest();
                    break;
                case LogicState.ReqHttpServer:
                    RequestHttpServer();
                    break;
                case LogicState.ReqLighthouseConfigAgain:
                    StartReqLighthouseConfig(mCurrentLighthouseConfig.MetaData.lighthouseId);
                    break;
                case LogicState.GetLighthouseCompleted:
                    Target.ChangeState<AppVersionCheckState>();
                    break;
                case LogicState.ReqLighthouseConfigFailure:
                    OnFail(AppUpdaterErrorType.RequestLighthouseFailure);
                    break;
            }
        }


        private void OnFail(AppUpdaterErrorType errorType)
        {
            Context.ErrorType = errorType;
            
            Target.ChangeState<AppUpdateFailureState>();
        }


        #region Get lighthouse config

        private void StartReqLighthouseConfig(string lighthouseId = null)
        {
            if (string.IsNullOrEmpty(lighthouseId))
            {
                Logger.Info("Start request lighthouse config!");
                mState = LogicState.RequestingLighthouseConfig;
            }
            else
            {
                Logger.Info($"Start request lighthouse config that id is {lighthouseId} !");
                mState = LogicState.RequestingLighthouseConfigAgain;
            }

            Context.LighthouseConfigDownloader.StartGetLighthouseConfigFromRemote(
                OnGetLighthouseConfigFromRemoteCallback
                , lighthouseId);
        }

        private void OnGetLighthouseConfigFromRemoteCallback(bool success, string contents)
        {
            if (success)
            {
                mCurLighthouseFromTo = Context.LighthouseConfigDownloader.CurRequestFileServerType;

                switch (mState)
                {
                    case LogicState.RequestingLighthouseConfig:

                        try
                        {
                            //Logger.Debug($"Lighthouse content : \r\n {contents}");
                            mCurrentLighthouseConfig = LighthouseConfig.ReadFromJson(contents);
                            mState = LogicState.LoadAppVerisonInfo;
                        }
                        catch (Exception e)
                        {
                            Logger.Error(
                                $"Parse lighthouse config that from remote file server failure! StackTrace : {e.StackTrace}");
                            Context.ErrorType = AppUpdaterErrorType.RequestLighthouseFailure;
                            mState = LogicState.ReqLighthouseConfigFailure;
                        }

                        break;
                    case LogicState.RequestingLighthouseConfigAgain:
                        var targetLighthouseConfig = LighthouseConfig.ReadFromJson(contents);
                        Logger.Info(
                            $"Get target config success , lighthouseId : {targetLighthouseConfig.MetaData.lighthouseId}");
                        AppVersionManager.MakeCurrentLighthouseConfig(targetLighthouseConfig);
                        mState = LogicState.GetLighthouseCompleted;
                        break;
                }
            }
            else //下载lighthouse失败
            {
                Context.ErrorType = AppUpdaterErrorType.RequestLighthouseFailure;
                Logger.Error($"Download lighthouse config failure ! Current state : {mState}");
                mState = LogicState.ReqLighthouseConfigFailure;
            }
        }

        #endregion

        #region Get appinfo

        private void LoadAppVersion()
        {
            mState = LogicState.LoadingAppVerisonInfo;
            LoadLocalBuiltInAppInfo();
        }

        private void LoadLocalBuiltInAppInfo()
        {
            var innerAppInfoPath = Context.GetStreamingAssetsPath(AssetsFileSystem.AppInfoFileName);
            Logger.Debug($"Start load built appinfo file , path : {innerAppInfoPath}");
            Target.Request.Load(innerAppInfoPath, LocalBuiltInAppInfoCallback);
        }

        private void LocalBuiltInAppInfoCallback(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                Logger.Error($"Load built app info file failure , file name is \"{AssetsFileSystem.AppInfoFileName}\" !");
                this.mState = LogicState.LoadAppVerisonInfoFailure;
                return;
            }
            
            AppInfoManifest builtinAppInfo = null; 
            try
            {
                builtinAppInfo = AppVersionManager.ParseAppInfo(bytes);
            }
            catch(Exception ex)
            {
                Logger.Error($"Parse built app info failure , error stackTrace : {ex.StackTrace}");
                this.mState = LogicState.ParseBuiltinAppInfoFailure;
                return;
            }
            
            AppInfoManifest localAppInfo = null; 
            try
            {
                localAppInfo = AppVersionManager.LoadLocalAppInfo();
            }
            catch(Exception ex)
            {
                Logger.Error($"Parse local app info failure , error stackTrace : {ex.StackTrace}");
                this.mState = LogicState.ParseLocalAppInfoFailure;
                return;
            } 
            
            if (localAppInfo != null)
            {
                Logger.Debug($"Local veriosn : {localAppInfo.version} , builtin verison : {builtinAppInfo.version}");

                var builtinVerison = new Version(builtinAppInfo.version);
                var localVersion = new Version(localAppInfo.version);
                var result = builtinVerison.CompareTo(localVersion);

                if (result > Version.VersionCompareResult.Equal)
                {
                    var clearPath = new DirectoryInfo(string.Concat(Application.persistentDataPath,
                        Path.DirectorySeparatorChar, AssetsFileSystem.RootFolderName));
                    if (clearPath.Exists)
                    {
                        clearPath.Delete(true);
                        clearPath.Create();
                        Logger.Info($"Clear external app folder : {clearPath.FullName} ");
                    }

                    Logger.Info("Make built appinfo as current!");
                    AppVersionManager.MakeCurrentAppInfo(builtinAppInfo);
                }
                else
                {
                    Logger.Info("Make local appinfo as current!");
                    AppVersionManager.MakeCurrentAppInfo(localAppInfo);
                }
            }
            else
            {
                Logger.Info("Local appinfo file is not exist , make built appinfo file as current!");
                Context.IsFirstRun = true;
                AppVersionManager.MakeCurrentAppInfo(builtinAppInfo);
            }

            this.mState = LogicState.CheckLocalResManifest;
        }

        #endregion

        #region Check local resource manifest

        private void StartCheckResourceManifest()
        {
            //this.mState = LogicState.ReqHttpServer;
            this.mState = LogicState.CheckLocalResManifesting;
            var localDataResName = AssetsFileSystem.AppDataResManifestName;

            if (!AppVersionManager.IsLocalResManifestExist(localDataResName))
            {
                this.mCheckingResManifestType = CheckingResManifestType.DataRes;
                var localDataResNameInnerAppInfoPath = Context.GetStreamingAssetsPath(localDataResName);
                Target.Request.Load(localDataResNameInnerAppInfoPath, OnLoadResManifestFileComplted);
            }
            else
            {
                Logger.Info("The data_res manifest is already exist , check untiy resource manifest!");
                this.StartCheckUnityDataRes();
            }
        }

        private void StartCheckUnityDataRes()
        {
            this.mCheckingResManifestType = CheckingResManifestType.UnityRes;
            var localUnityDataResName = AssetsFileSystem.UnityResManifestName;
            if (!AppVersionManager.IsLocalResManifestExist(localUnityDataResName))
            {
                Logger.Info($"The local manifest file that name is \"{localUnityDataResName}\" is not exist!");
                Target.Request.Load(Context.GetStreamingAssetsPath(localUnityDataResName), OnLoadResManifestFileComplted);
            }
            else
            {
                Logger.Info($"The local manifest file that name is \"{localUnityDataResName}\" is exist!");
                this.mCheckingResManifestType = CheckingResManifestType.Done;
            }
        }

        private void OnLoadResManifestFileComplted(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                if (this.mCheckingResManifestType == CheckingResManifestType.DataRes)
                {
                    throw new FileNotFoundException(AssetsFileSystem.AppDataResManifestName);
                }
                else if (this.mCheckingResManifestType == CheckingResManifestType.UnityRes)
                {
                    throw new FileNotFoundException(AssetsFileSystem.UnityResManifestName);
                }

                throw new InvalidOperationException("Load resource manifest file error!");
            }
            else
            {
                if (this.mCheckingResManifestType == CheckingResManifestType.DataRes)
                {
                    this.mCheckingResManifestType = CheckingResManifestType.UnityRes;
                    Logger.Info("Load builtin data_resource manifest completd !");
                    AppVersionManager.SaveToLocalDataResManifest(data, AssetsFileSystem.AppDataResManifestName);
                    this.StartCheckUnityDataRes();
                }
                else if(this.mCheckingResManifestType == CheckingResManifestType.UnityRes)
                {
                    Logger.Info("Load builtin untiy resource manifest completd !");
                    this.mCheckingResManifestType = CheckingResManifestType.Done;
                    AppVersionManager.SaveToLocalDataResManifest(data, AssetsFileSystem.UnityResManifestName);
                }
                else
                {
                    throw new InvalidOperationException($"Checking manifest type : {this.mCheckingResManifestType.ToString()}");
                }
            }
        }

        private void StartCheckingResourceManifest()
        {
            if (this.mCheckingResManifestType == CheckingResManifestType.Done)
            {
                this.mState = LogicState.ReqHttpServer;
                Logger.Info("Checking resource manifest complted!");
            }
        }

        #endregion

        #region Request getVersion response

        private void RequestHttpServer()
        {
            mState = LogicState.RequestingHttpServer;

            var sersveData = mCurrentLighthouseConfig.GetCurrentServerData();

            Context.Requester.ReqGetVersion(sersveData, AppVersionManager.AppInfo.version,
                mCurrentLighthouseConfig.MetaData.lighthouseId,
                mCurLighthouseFromTo,
                info =>
                {
                    if (info != null)
                    {
                        if (string.IsNullOrEmpty(info.lighthouseId))
                        {
                            Logger.Error("The lighthouse id that was return by server is null or empty!");
                            Context.ErrorType = AppUpdaterErrorType.RequestGetVersionFailure;
                            mState = LogicState.ReqLighthouseConfigFailure;
                            return;
                        }

                        Context.GetVersionResponseInfo = info;

                        if (info.lighthouseId == mCurrentLighthouseConfig.MetaData.lighthouseId)
                        {
                            Logger.Info("The current lighthouseconfig is latest !");
                            AppVersionManager.MakeCurrentLighthouseConfig(mCurrentLighthouseConfig);

                            mState = LogicState.GetLighthouseCompleted;
                        }
                        else
                        {
                            Logger.Info("The current lighthouseconfig is old , try to get a new one !");
                            mState = LogicState.ReqLighthouseConfigAgain;
                        }
                    }
                    else
                    {
                        Context.ErrorType = AppUpdaterErrorType.RequestGetVersionFailure;
                        mState = LogicState.ReqLighthouseConfigFailure;
                    }
                });
        }

        #endregion

        #endregion
    }
}