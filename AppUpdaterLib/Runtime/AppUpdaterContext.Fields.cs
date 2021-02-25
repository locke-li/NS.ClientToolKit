/***************************************************************

 *  类名称：        AppUpdaterContext

 *  描述：			热更新模块上下文环境	

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/26 17:54:47

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Text;
using CenturyGame.AppUpdaterLib.Runtime.Configs;
using CenturyGame.AppUpdaterLib.Runtime.Helps;
using CenturyGame.AppUpdaterLib.Runtime.Interfaces;
using CenturyGame.AppUpdaterLib.Runtime.Protocols;
using CenturyGame.AppUpdaterLib.Runtime.ResManifestParser;
using CenturyGame.AppUpdaterLib.Runtime.Utilities;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    public class AppUpdaterProgressData
    {
        #region 下载信息

        /// <summary>
        /// 当前热更新的资源类型
        /// </summary>
        public UpdateResourceType CurrentUpdateResourceType { set; get; }

        /// <summary>
        /// 当前下载进度
        /// </summary>
        public float Progress { set; get; }


        /// <summary>
        /// 总共需要下载的文件数目
        /// </summary>
        public ulong TotalDownloadFileCount { set; get; }

        /// <summary>
        /// 总的下载大小
        /// </summary>
        public ulong TotalDownloadSize { set; get; }

        /// <summary>
        /// 当前正在下载的文件的大小 
        /// </summary>
        public ulong CurrentDownloadingFileSize { set; get; }

        /// <summary>
        /// 当前已下载的文件数目
        /// </summary>
        public ulong CurrentDownloadFileCount { set; get; }
        /// <summary>
        /// 当前已下载的大小
        /// </summary>
        public ulong CurrentDownloadSize { set; get; }

        /// <summary>
        /// 下载总时长
        /// </summary>
        public float CurrentDownloadFileTotalTime { set; get; }

        /// <summary>
        /// 当前下载速度
        /// </summary>
        //public float LoadSpeed { set; get; }

        #endregion

        public void Clear()
        {

        }
    }

    //public enum ResSyncMode
    //{
    //    FULL,//同步远端所有的资源

    //    LOCAL,//只同步本地清单资源

    //    SUB_GROUP,//只同步子组的资源
    //}

    //internal class ResUpdateConfig
    //{
    //}


    internal class DiskInfo
    {
        public bool IsGetReady = false;

        public int AvailableSpace;
        public int BusySpace;
        public int TotalSpace;

        public DateTime time;

        public void Clear()
        {
            this.IsGetReady = false;
            this.AvailableSpace = 0;
            this.BusySpace = 0;
            this.TotalSpace = 0;
            this.time = TimeUtility.EpochTime;            
        }

    }
    

    internal partial class AppUpdaterContext
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        /// <summary>
        /// 当前所处的状态
        /// </summary>
        public string StateName { set; get; }

        /// <summary>
        /// 进度数据
        /// </summary>
        public AppUpdaterProgressData ProgressData { private set; get; } = new AppUpdaterProgressData();

        /// <summary>
        /// 热更新错误类型
        /// </summary>
        public AppUpdaterErrorType ErrorType { set; get; } = AppUpdaterErrorType.None;

        /// <summary>
        /// 错误信息
        /// </summary>
        public StringBuilder InfoStringBuilder { set; get; } = new StringBuilder();

        private StringBuilder mTempSb = new StringBuilder();
        
        public bool IsFirstRun = false;

        /// <summary>
        /// GetVersion返回相关
        /// </summary>
        public GetVersionResponseInfo GetVersionResponseInfo { set; get; }

        public IAppUpdaterRequester Requester { set; get; }

        #endregion

        #region Resource update

        public static readonly string[] DefaultResVersions = new string[0];
        /// <summary>
        /// 目标版本清单名
        /// </summary>
        public string[] ResVersions = DefaultResVersions;

        /// <summary>
        ///  目标资源版本号 
        /// </summary>
        public static readonly string[] DefaultResVersionNums = new string[0];
        public string[] ResVersionNums = DefaultResVersionNums;

        /// <summary>
        /// 本地资源版本号
        /// </summary>
        public static readonly string[] DefaultLocalResVersionNums = new string[0];
        public string[] LocalResVersionNums = DefaultLocalResVersionNums;


        public static readonly string[] DefaultLocalResFiles = new string[0];
        /// <summary>
        /// 本地清单名列表
        /// </summary>
        public string[] LocalResFiles = DefaultLocalResFiles;

        public static readonly BaseResManifestParser[] DefaultResVersionParsers = new BaseResManifestParser[0];
        public BaseResManifestParser[] ResVersionParsers = DefaultResVersionParsers;

        public static readonly int DefaultResVerisonIdx = -1;
        /// <summary>
        /// 当前更新的资源版本相对于ResVersions的索引
        /// </summary>
        public int CurrentResVersionIdx = DefaultResVerisonIdx;

        /// <summary>
        /// 资源目标版本号，即目标Patch
        /// </summary>
        public string TargetResVersionNum = string.Empty;

        public LighthouseConfigDownloader LighthouseConfigDownloader = null;

        //public RemoteFileDownloader RemoteFileDownloader = null;

        public IStorageInfoProvider StorageInfoProvider = null;

        public DiskInfo DiskInfo = new DiskInfo();

        public void Clear()
        {
            this.StateName = string.Empty;
            this.ProgressData.CurrentUpdateResourceType = UpdateResourceType.UnKnow;
            this.ProgressData.Progress = 0;
            this.ProgressData.TotalDownloadSize = 0;
            this.ProgressData.CurrentDownloadingFileSize = 0;
            this.ProgressData.CurrentDownloadSize = 0;
            this.ProgressData.TotalDownloadFileCount = 0;
            this.ProgressData.CurrentDownloadFileCount = 0;

            this.ErrorType = AppUpdaterErrorType.None;
            this.mTempSb.Clear();
            this.IsFirstRun = false;
            this.GetVersionResponseInfo = null;

            this.ResVersions = DefaultResVersions;
            this.LocalResFiles = DefaultLocalResFiles;
            this.ResVersionParsers = DefaultResVersionParsers;
            this.CurrentResVersionIdx = DefaultResVerisonIdx;

            this.LighthouseConfigDownloader?.Clear();
            //this.RemoteFileDownloader?.Clear();
            this.DiskInfo.Clear();
        }

#if DEBUG_APP_UPDATER
      
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"更新的资源类型 : {this.GetResType(this.ProgressData.CurrentUpdateResourceType)}");
            sb.AppendLine($"当前所属状态 : {this.StateName}");
            sb.AppendLine($"当前下载进度 : {(this.ProgressData.Progress * 100):f1}%");
            sb.AppendLine($"当前需要下载的文件数量 : {this.ProgressData.TotalDownloadFileCount}");
            sb.AppendLine($"当前需要下载的文件大小 : {this.ProgressData.TotalDownloadSize}");
            sb.AppendLine($"正在下载的文件大小 : {this.ProgressData.CurrentDownloadingFileSize}");
            sb.AppendLine($"当前已下载的文件数量 : {this.ProgressData.CurrentDownloadFileCount}");
            sb.AppendLine($"当前已下载的文件大小 : {this.ProgressData.CurrentDownloadSize}");
            sb.AppendLine($"当前下载的总时长 : {this.ProgressData.CurrentDownloadFileTotalTime}");
            
            if (this.DiskInfo.IsGetReady)
            {
                sb.AppendLine($"Last fetch disk info time : {this.DiskInfo.time:yyyy_MM_dd-HH_mm_ss.fff}");
                sb.AppendLine($"Disk totalSpace : {this.DiskInfo.TotalSpace:f1}");
                sb.AppendLine($"Disk availableSpace: {this.DiskInfo.AvailableSpace:f1}");
                sb.AppendLine($"Disk busySpace : {this.DiskInfo.BusySpace:f1}");
            }

            if (this.ErrorType != AppUpdaterErrorType.None)
            {
                sb.Append($"错误类型 ：{this.ErrorType} 描述 : {ErrorTypeHelper.GetErrorString(this.ErrorType)}");
            }

            return sb.ToString();
        }

        public string GetResType(UpdateResourceType type)
        {
            switch (type)
            {
                case UpdateResourceType.TableData:
                    return "表数据";
                case UpdateResourceType.NormalResource:
                    return "Unity资源";
                default:
                    return "未知";
            }
        }

#endif
#endregion

    }
}
