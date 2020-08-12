/***************************************************************

 *  类名称：        AppUpdateDataResState

 *  描述：		    数据资源更新

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/12 11:35:18

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using CenturyGame.AppUpdaterLib.Runtime.Managers;
using CenturyGame.AppUpdaterLib.Runtime.ResManifestParser;
using CenturyGame.Core.FSM;
using CenturyGame.Core.Functional;
using UnityEngine;

namespace CenturyGame.AppUpdaterLib.Runtime.States.Concretes
{
    class AppUpdateDataResState : BaseAppUpdaterFunctionalState
    {
        #region Inner class&enum

        public enum InnerState
        {
            Idle,

            //Get remote resManifest
            StartRequestResManifest,
            RequestingManifest,
            RequestManifestCompleted,
            RequestManifestFailured,

            ParseLocalResManifestFailure,//解析本地配置失败

            ParseRemoteResManifestFailure,//解析远程配置失败

            DiskSpaceNotEnough,//磁盘空间不足以支撑下载更新文件

            //Get res difference
            StartCalculateResDiff,
            CalculatingResDiff,
            
            //Download difference and apply patch
            DownloadAndApplyDiff,

            //Download|ApplyPatch error
            DownloadAndApplyPatchFailure,

            //Resource update completd
            ResUpdateCompleted
        }

        public enum FileDownLoadState
        {
            Idle,
            StartDownLoad,
            DownLoading,
            DownLoadCompleted,
            DownloadFail
        }

        #endregion

        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private InnerState mCurState = InnerState.Idle;

        private FileServerType mCurFileServerType = FileServerType.CDN;

        private VersionManifest mCurRemoteManifest = null;

        private FileDownLoadState mCurFileDownLoadState = FileDownLoadState.Idle;

        private List<FileDesc> mCurDownloadTasks = new List<FileDesc>();
        private int mCurrentDownloadIndex = 0;

        private BaseResManifestParser mCurManifestParser = null;

        private VersionManifest mLocalManifest = null;

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

            if (Context.RemoteFileDownloader == null)
            {
                Context.RemoteFileDownloader = new RemoteFileDownloader(Context,this.Target.Request);
            }
            else
            {
                Context.RemoteFileDownloader.Clear();
            }
        }

        

        public override void Execute(AppUpdaterFsmOwner entity)
        {
            base.Execute(entity);
            Context.RemoteFileDownloader.Update();
            switch (mCurState)
            {
                case InnerState.StartRequestResManifest:
                    this.RequestResManifest(FileServerType.CDN);
                    break;
                case InnerState.RequestManifestFailured:
                    this.OnResUpdateFailure(AppUpdaterErrorType.RequestResManifestFailure);
                    break;
                case InnerState.RequestManifestCompleted:
                    this.ProcessRequestResManifestCompleted();
                    break;
                case InnerState.StartCalculateResDiff://开始计算差异
                    this.ProcessCalculateResDifference();
                    break;
                case InnerState.DownloadAndApplyDiff://开始应用差异
                    this.ProcessDownloadAndApplyDiff();
                    break;
                case InnerState.ResUpdateCompleted://更新完成
                    this.ProcessResUpdateCompleted();
                    break;
                case InnerState.DownloadAndApplyPatchFailure:
                    this.OnResUpdateFailure(AppUpdaterErrorType.DownloadFileFailure);
                    break;
                case InnerState.ParseLocalResManifestFailure:
                    this.OnResUpdateFailure(AppUpdaterErrorType.ParseLocalResManifestFailure);
                    break;
                case InnerState.ParseRemoteResManifestFailure:
                    this.OnResUpdateFailure(AppUpdaterErrorType.ParseRemoteResManifestFailure);
                    break;
                case InnerState.DiskSpaceNotEnough:
                    this.OnResUpdateFailure(AppUpdaterErrorType.DiskIsNotEnoughToDownPatchFiles);
                    break;

            }
        }

        private void RequestResManifest(FileServerType fileServerType)
        {
            this.mCurFileServerType = fileServerType;
            string url = Context.GetCurrentVerisonFileUrl(this.mCurFileServerType);
            Logger.Info($"Start request remote resource manifest {url} .");


            Context.AppendInfo($"Res current version : {Context.LocalResVersionNums[Context.CurrentResVersionIdx]} target version : {Context.ResVersionNums[Context.CurrentResVersionIdx]}");
            Context.AppendInfo($"Current resource group name is {Context.GetCurrentLocalVersionFileName()} .");

            this.mCurState = InnerState.RequestingManifest;
            this.Target.Request.Load(url, OnResponseResManifestCallback);
        }

        private void OnResponseResManifestCallback(byte[] netData)
        {
            if (netData == null || netData.Length == 0)
            {
                if (this.mCurFileServerType == FileServerType.CDN)
                {
                    this.RequestResManifest(FileServerType.OSS);
                }
                else
                {
                    this.mCurState = InnerState.RequestManifestFailured;
                }
            }
            else
            {
                string resManifestContent = System.Text.Encoding.UTF8.GetString(netData);

                var version = Context.ResVersions[Context.CurrentResVersionIdx];
                this.mCurManifestParser = Context.ResVersionParsers[Context.CurrentResVersionIdx];
                Context.ProgressData.CurrentUpdateResourceType = this.mCurManifestParser.GetUpdateResourceType();

                Logger.InfoFormat("veriosn : {0} , parser : {1} .",version, this.mCurManifestParser.GetType().Name);

                try
                {
                    this.mCurRemoteManifest = this.mCurManifestParser.Parse(resManifestContent);
                }
                catch (Exception ex)
                {
                    Context.ErrorType = AppUpdaterErrorType.ParseRemoteResManifestFailure;

                    this.mCurState = InnerState.ParseRemoteResManifestFailure;

                    Logger.Error($"Error stackTrace : {ex.StackTrace}");

                    return;
                }
                
                this.mCurState = InnerState.StartCalculateResDiff;
            }
        }


        private void OnResUpdateFailure(AppUpdaterErrorType failureType)
        {
            Logger.Error($"Resource update failure , failure type : {failureType}! ");
            Context.ErrorType = failureType;
            this.Target.ChangeState<AppUpdateFailureState>();
        }

        /// <summary>
        /// 处理资源清单完成
        /// </summary>
        private void ProcessRequestResManifestCompleted()
        {
            Logger.Error($"Request manifest complted!");

            this.mCurFileServerType = FileServerType.CDN;

            this.mCurState = InnerState.StartCalculateResDiff;
        }


        private void ProcessCalculateResDifference()
        {
            Logger.Info($"Start calculate resource difference !");
            this.mCurState = InnerState.CalculatingResDiff;

            var localResManifestContents= AppVersionManager.LoadLocalResManifestContents(Context.LocalResFiles[Context.CurrentResVersionIdx]);

            if (string.IsNullOrEmpty(localResManifestContents))
            {
                throw new FileNotFoundException(Context.LocalResFiles[Context.CurrentResVersionIdx]);
            }

            try
            {
                this.mLocalManifest = this.mCurManifestParser.Parse(localResManifestContents);
            }
            catch (Exception ex)
            {
                Context.ErrorType = AppUpdaterErrorType.ParseLocalResManifestFailure;

                this.mCurState = InnerState.ParseLocalResManifestFailure;

                Logger.Error($"Error stackTrace : {ex.StackTrace}");

                return;
            }

            var diff = this.mLocalManifest.CalculateDifference(this.mCurRemoteManifest);
            
            if (diff != null && diff.Count > 0)
            {
                ulong totalDownloadSize = 0;
                diff.ForeachCall(x => totalDownloadSize += (ulong)x.S);
                Context.ProgressData.TotalDownloadSize += totalDownloadSize;
                Context.ProgressData.TotalDownloadFileCount += (ulong)diff.Count;

                Logger.Info($"Needed to update {diff.Count} files , total size is {totalDownloadSize}B .");

                Context.FetchDeviceStorageInfo();

                if (Context.DiskInfo.IsGetReady && Context.DiskInfo.BusySpace + CommonConst.MIN_DISK_AVAILABLE_SPACE > Context.DiskInfo.TotalSpace)
                {
                    this.mCurState = InnerState.DiskSpaceNotEnough;
                }
                else
                {
                    this.mCurState = InnerState.DownloadAndApplyDiff;
                    this.mCurrentDownloadIndex = 0;
                    this.mCurDownloadTasks = diff;
                    this.mCurFileDownLoadState = FileDownLoadState.StartDownLoad;
                }
            }
            else
            {
                Logger.Info($"The current remote manifest file that name is {Context.GetCurrentVersionFileName()} is the same as local .");
                this.mCurState = InnerState.ResUpdateCompleted;
            }
        }

        private void ProcessDownloadAndApplyDiff()
        {
            Context.ProgressData.CurrentDownloadFileTotalTime += Time.unscaledDeltaTime;
            switch (this.mCurFileDownLoadState)
            {
                case FileDownLoadState.Idle:
                    this.ProcessFileIdle();
                    break;
                case FileDownLoadState.StartDownLoad:
                    this.ProcessFileStartDownLoad();
                    break;
                case FileDownLoadState.DownLoading:
                    this.ProcessDownLoading();
                    break;
                case FileDownLoadState.DownLoadCompleted:
                    this.ProcessDownLoadCompleted();
                    break;
                case FileDownLoadState.DownloadFail:
                    this.ProcessDownloadFail();
                    break;
                default: break;
            }

            this.UpdateDownloadInfos();
        }

        #region Download file from remote to local


        private void ProcessFileIdle()
        {
            
        }

        private void UpdateDownloadInfos()
        {
        }
        private void ProcessFileStartDownLoad()
        {
            var fileDesc = this.mCurDownloadTasks[this.mCurrentDownloadIndex];
            Context.ProgressData.CurrentDownloadingFileSize = (ulong)fileDesc.S;
            string fileName = this.mCurManifestParser.GetRemotePath(fileDesc);

            Context.RemoteFileDownloader.StartLoadResFileFromRemote(OnFileDownloaded , fileName);

            this.mCurFileDownLoadState = FileDownLoadState.DownLoading;
        }

        private void OnFileDownloaded(bool success , byte[] data)
        {
            if (!success)
            {
                this.mCurState = InnerState.DownloadAndApplyPatchFailure;
                this.mCurFileDownLoadState = FileDownLoadState.DownloadFail;

                Logger.Error($"Download file that name is \"{this.mCurDownloadTasks[this.mCurrentDownloadIndex].N}\" failure! ");

                this.SaveCurrentConfig(false);
            }
            else
            {
                this.WriteRemoteFileDataToLocal(data);
            }
        }

        private void WriteRemoteFileDataToLocal(byte[] data)
        {
            Context.ProgressData.CurrentDownloadSize += (ulong)data.Length;
            Context.ProgressData.CurrentDownloadFileCount++;
            Context.ProgressData.Progress = Context.ProgressData.CurrentDownloadSize / ((float)Context.ProgressData.TotalDownloadSize);

            string path = AssetsFileSystem.GetWritePath($"{this.mCurManifestParser.GetLocalRoot()}/{this.mCurDownloadTasks[this.mCurrentDownloadIndex].N}", true);
            File.WriteAllBytes(path, data);
            this.mLocalManifest.UpdateInnerFile(this.mCurDownloadTasks[this.mCurrentDownloadIndex]);

            if (this.CheckDownloadOperationIsCompleted())
            {
                Context.AppendInfo($"Current resource version update to \"{Context.ResVersionNums[Context.CurrentResVersionIdx]}\".");
                Logger.Info($"Update manifest that name is {Context.GetCurrentVersionFileName()} is success!");
                this.mCurFileDownLoadState = FileDownLoadState.Idle;
                this.mCurState = InnerState.ResUpdateCompleted;
            }
            else
            {
                Logger.Info($"Load file that name is {this.mCurDownloadTasks[this.mCurrentDownloadIndex].N} completed!");
                this.mCurFileDownLoadState = FileDownLoadState.DownLoadCompleted;
            }
        }

        private void ProcessDownLoading()
        {
            
        }


        private void ProcessDownLoadCompleted()
        {
            this.mCurrentDownloadIndex++;

            this.mCurFileDownLoadState = FileDownLoadState.StartDownLoad;
        }


        public bool CheckDownloadOperationIsCompleted()
        {
            return this.mCurrentDownloadIndex >= this.mCurDownloadTasks.Count - 1;
        }

        private void ProcessDownloadFail()
        {
            this.mCurFileDownLoadState = FileDownLoadState.Idle;
        }

        #endregion


        private void ProcessResUpdateCompleted()
        {
            this.SaveCurrentConfig();
            Logger.Info($"Update current resource completed , remote manifest name is \"{Context.GetCurrentVersionFileName()}\" .");
            IRoutedEventArgs arg = new RoutedEventArgs()
            {
                EventType = (int)AppUpdaterInnerEventType.OnCurrentResUpdateCompleted
            };
            this.Target.HandleMessage(in arg);
        }

        public override bool OnMessage(AppUpdaterFsmOwner entity, in IRoutedEventArgs eventArgs)
        {
            var eventType = (AppUpdaterInnerEventType)eventArgs.EventType;

            switch (eventType)
            {
                case AppUpdaterInnerEventType.PerformResUpdateOperation:
                    this.Reset();
                    this.mCurState = InnerState.StartRequestResManifest;
                    return true;
                default:
                    break;
            }

            return base.OnMessage(entity, in eventArgs);
        }

        private void SaveCurrentConfig(bool updateVerisonNum = true)
        {
            string curManifestName = Context.LocalResFiles[Context.CurrentResVersionIdx];
            var json = this.mCurManifestParser.Serialize(this.mLocalManifest);
            AppVersionManager.SaveToLocalDataResManifest(json, curManifestName);

            if (updateVerisonNum)
            {
                this.mCurManifestParser.WriteToAppInfo(Context.ResVersionNums[Context.CurrentResVersionIdx]);
            }
        }

        public override void Exit(AppUpdaterFsmOwner entity)
        {
            base.Exit(entity);
            Context.ProgressData.CurrentDownloadingFileSize = 0 ;
            this.Recycle();
        }

        private void Recycle()
        {
            List<FileDesc> recycleList = new List<FileDesc>();

            if (this.mCurRemoteManifest != null)
            {
                this.mCurRemoteManifest.Datas.ForEach(x => recycleList.Add(x));
                this.mCurRemoteManifest.Datas.Clear();
            }

            if (this.mLocalManifest != null)
            {
                this.mLocalManifest.Datas.ForEach(x =>
                {
                    if (!recycleList.Contains(x))
                        recycleList.Add(x);
                });
                this.mLocalManifest.Datas.Clear();
            }

            recycleList.ForEach(x=>VersionManifestParser.Pools.Recycle(x));
        }

        public override void Reset()
        {
            base.Reset();

            this.mCurState = InnerState.Idle;
            this.mCurFileServerType = FileServerType.CDN;
            this.mCurRemoteManifest = null;
            this.mCurFileDownLoadState = FileDownLoadState.Idle;
            this.mCurDownloadTasks.Clear();
            this.mCurrentDownloadIndex = 0;
            this.mCurManifestParser = null;

        }

        #endregion

    }
}