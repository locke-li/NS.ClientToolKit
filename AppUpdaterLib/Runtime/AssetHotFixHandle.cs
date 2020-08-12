//using System;
//using System.IO;
//using System.Collections;
//using System.Collections.Generic;
//using CenturyGame.AssetBundleManager.Runtime;
//using UnityEngine;
//using UnityEngine.Networking;

//public class AssetHotFixHandle
//{

//    private Action<bool> callbackHandle;
//    private HttpRequest req = null;
//    List<FileDesc> taskList = new List<FileDesc>();
//    private int currentIndex = 0;

//    private byte mState;
//    private byte state
//    {
//        set
//        {
//            Debug.LogError("mState is " + value);
//            this.mState = value;
//        }

//        get
//        {
//            return this.mState;
//        }
//    }
//    private float downloadDuration = 0;
//    private Action<LoadStateHandle> stateChangeHandle;
//    private LoadStateHandle msgState = null;
//    private byte[] tmpResult = null;
//    private bool next = false;
//    public AssetHotFixHandle()
//    {
//        req = new HttpRequest();
//        msgState = new LoadStateHandle();
//    }
//    public void LoadFileList(Action<LoadStateHandle> changeHandle)
//    {
//        stateChangeHandle = changeHandle;
//        if (next)
//        {
//            state = 8;
//        }
//        else
//        {
//            loadBuiltInConfigInfo();
//        }
//    }
//    /// <summary>
//    /// 加载内置配置索引
//    /// </summary>
//    private void loadBuiltInConfigInfo()
//    {
//        loadStateChange(LoadState.LoadLocalConfigInfo);
//        string path = AbHelp.GetStreamingAssetsPath(AbHelp.AbConfigInfoName, null, false);
//        AbHelp.HotFixLog(string.Concat("加载内置索引:", path));
//        req.Load(path, loadBuiltInConfigInfoCallback);
//    }
//    /// <summary>
//    /// 加载内置配置索引回调
//    /// </summary>
//    /// <param name="dat"></param>
//    private void loadBuiltInConfigInfoCallback(byte[] dat)
//    {
//        VersionManifest builtInConfig = JsonUtility.FromJson<VersionManifest>(System.Text.Encoding.UTF8.GetString(dat));
//        AbHelp.HotFixLog("加载内置索引回调，版本号：" + builtInConfig.Ver);
//        string localConfigPath = AbHelp.GetWritePath(AbHelp.AbConfigInfoClientName, true);
//        if (File.Exists(localConfigPath))
//        {
//            AbHelp.ConfigInfoClient = JsonUtility.FromJson<ABConfigInfoClient>(System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(localConfigPath)));
//            AbHelp.HotFixLog(string.Format("存在本地索引，地址：{0}，版本号：{1}", localConfigPath, AbHelp.ConfigInfoClient.Ver));
//            if (string.Compare(builtInConfig.Ver, AbHelp.ConfigInfoClient.Ver) > 0)
//            {
//                AbHelp.HotFixLog("内置比本地的版本号高：" + builtInConfig.Ver + ">" + AbHelp.ConfigInfoClient.Ver);
//                DirectoryInfo clearPath = new DirectoryInfo(string.Concat(Application.persistentDataPath, Path.DirectorySeparatorChar, AbHelp.RootFolder));
//                if (clearPath.Exists)
//                {
//                    clearPath.Delete(true);
//                    clearPath.Create();
//                    AbHelp.HotFixLog("删除文件夹:" + clearPath.FullName);
//                }
//                AbHelp.HotFixLog("选用内置索引作为索引，并存储为本地索引。");
//                AbHelp.ABConfigInfoClientCopyFrom(builtInConfig, localConfigPath);
//            }
//            else
//            {
//                AbHelp.HotFixLog("内置比本地版本号低：" + builtInConfig.Ver + "<=" + AbHelp.ConfigInfoClient.Ver);
//                AbHelp.HotFixLog("选用本地索引为当前索引。");
//                AbHelp.ABConfigInfoClientCopyFrom(AbHelp.ConfigInfoClient);
//            }
//        }
//        else
//        {
//            AbHelp.HotFixLog("无本地索引。");
//            AbHelp.HotFixLog("选用本地索引为当前索引。");
//            AbHelp.IsFirstRun = true;
//            AbHelp.ABConfigInfoClientCopyFrom(builtInConfig, localConfigPath);
//        }
//        loadRemoteHotfixInfo();
//    }
//    /// <summary>
//    /// 加载远程热更新索引
//    /// </summary>
//    private void loadRemoteHotfixInfo()
//    {
//        //string url = string.Concat(AbHelp.DefaultFileSite, AbHelp.GetPlatformStr(), AbHelp.HotfixInfoName, "?t=", DateTime.Now.ToString("yyyyMMddHHmm"));
//        string url = string.Concat(AbHelp.DefaultFileSite, AbHelp.GetPlatformStr(), AbHelp.HotfixInfoName);


//        AbHelp.HotFixLog(string.Concat("加载远程热更新索引:", url));
//        loadStateChange(LoadState.LoadRemoteHotFixInfo);
//        req.Load(url, loadRemoteHotfixInfoCallback);
//    }
//    /// <summary>
//    /// 加载远程配置索引回调
//    /// </summary>
//    /// <param name="dat"></param>
//    private void loadRemoteHotfixInfoCallback(byte[] data)
//    {
//        if (data == null || data.Length == 0)
//        {
//            AbHelp.HotFixLog("加载远程热更新索引回调失败！");
//            msgState.ErrorCode = "#FF01";
//            state = 6;
//        }
//        else
//        {
//            HotFixConfig hotfix = JsonUtility.FromJson<HotFixConfig>(System.Text.Encoding.UTF8.GetString(data));
//            AbHelp.HotFixLog("加载远程热更新索引回调，地址为：" + hotfix.FileSite);
//            AbHelp.FileSite = hotfix.FileSite;
//            AbHelp.UpdatePath = hotfix.UpdatePath;
//            if (string.IsNullOrEmpty(hotfix.MinVer) || string.IsNullOrEmpty(AbHelp.UpdatePath))
//            {
//                loadRemoteConfigInfo();
//            }
//            else
//            {
//                if (string.Compare(AbHelp.ConfigInfoClient.Ver, hotfix.MinVer) >= 0)
//                {
//                    loadRemoteConfigInfo();
//                }
//                else
//                {
//                    minVerUerSelect();
//                }
//            }

//        }
//    }
//    /// <summary>
//    /// 加载远程配置索引
//    /// </summary>
//    private void loadRemoteConfigInfo()
//    {
//        string url = AbHelp.GetUrl(AbHelp.AbConfigInfoName);
//        AbHelp.HotFixLog(string.Concat("加载远程索引:", url));
//        loadStateChange(LoadState.LoadRemoteConfigInfo);
//        req.Load(url, loadRemoteConfigInfoCallback);
//    }
//    /// <summary>
//    /// 加载远程配置索引回调
//    /// </summary>
//    /// <param name="dat"></param>
//    private void loadRemoteConfigInfoCallback(byte[] data)
//    {
//        if (data == null || data.Length == 0)
//        {
//            AbHelp.HotFixLog("加载远程索引回调失败！");
//            msgState.ErrorCode = "#FF02";
//            state = 6;
//        }
//        else
//        {
//            VersionManifest config = JsonUtility.FromJson<VersionManifest>(System.Text.Encoding.UTF8.GetString(data));
//            AbHelp.HotFixLog("加载远程索引回调，版本号：" + config.Ver);
//            taskList.Clear();
//            AbHelp.RemoteConfigInfoVer = config.Ver;
//            if (string.Compare(AbHelp.RemoteConfigInfoVer, AbHelp.ConfigInfoClient.Ver) > 0)
//            {
//                AbHelp.HotFixLog("远程比当前版本号高：" + AbHelp.RemoteConfigInfoVer + ">" + AbHelp.ConfigInfoClient.Ver);
//                for (int i = 0; i < config.Datas.Count; i++)
//                {
//                    var item = config.Datas[i];
//                    bool add = false;
//                    var info = AbHelp.GetTask(item, ref add);
//                    if (add)
//                    {
//                        taskList.Add(info);
//                        msgState.AllSize += (ulong)info.S;
//                    }
//                }
//            }
//            if (taskList.Count == 0)
//            {
//                AbHelp.HotFixLog("热更新系统未发现有更新。");
//                startReadABConfig();
//            }
//            else
//            {
//                AbHelp.HotFixLog("热更新系统发现有更新：" + taskList.Count);
//                networkUerSelect();
//            }
//        }
//    }
//    /// <summary>
//    /// 开始读取AB的配置
//    /// </summary>
//    private void startReadABConfig()
//    {
//        state = 8;
//    }
//    private void minVerUerSelect()
//    {
//        msgState.SelectAction = minVerSelectHandle;
//        loadStateChange(LoadState.MinVerConfirm);
//    }
//    /// <summary>
//    /// 判断用户的网络状态，确定是否立即更新。
//    /// </summary>
//    private void networkUerSelect()
//    {
//        switch (Application.internetReachability)
//        {
//            case NetworkReachability.NotReachable:
//                {
//                    AbHelp.HotFixLog("网络已断开");
//                    state = 6;
//                    break;
//                }
//            case NetworkReachability.ReachableViaLocalAreaNetwork:
//                {
//                    AbHelp.HotFixLog("wifi连接中");
//                    //loadList();


//                    //特殊处理。。。。。。。。。。。。。。。。。。。。。。
//                    if (lastClick == UserClick.Skip)
//                    {
//                        startReadABConfig();
//                    }
//                    else
//                    {
//                        msgState.SelectAction = networkSelectHandle;
//                        loadStateChange(LoadState.NetConfirm);
//                    }
//                    break;
//                }
//            case NetworkReachability.ReachableViaCarrierDataNetwork:
//                {
//                    AbHelp.HotFixLog("4G/3G网络连接中");

//                    //特殊处理。。。。。。。。。。。。。。。。。。。。。。
//                    if (lastClick == UserClick.Skip)
//                    {
//                        startReadABConfig();
//                    }
//                    else
//                    {
//                        msgState.SelectAction = networkSelectHandle;
//                        loadStateChange(LoadState.NetConfirm);
//                    }
//                    break;
//                }
//        }
//    }
//    /// <summary>
//    /// 低于最小版本用户选择
//    /// </summary>
//    /// <param name="select"></param>
//    private void minVerSelectHandle(UserClick select)
//    {
//        switch (select)
//        {
//            case UserClick.Ok:
//                {
//                    Application.OpenURL(AbHelp.UpdatePath);
//                    Application.Quit();
//                    break;
//                }
//            case UserClick.Skip:
//                {
//                    loadRemoteConfigInfo();
//                    break;
//                }
//        }
//        lastClick = select;
//    }

//    private UserClick lastClick = UserClick.None;
//    /// <summary>
//    /// 4G网络下用户选择
//    /// </summary>
//    /// <param name="select"></param>
//    private void networkSelectHandle(UserClick select)
//    {
//        switch (select)
//        {
//            case UserClick.Ok:
//                {
//                    loadList();
//                    break;
//                }
//            case UserClick.Skip:
//                {
//                    startReadABConfig();
//                    break;
//                }
//        }
//    }
//    /// <summary>
//    /// 开始下载流程
//    /// </summary>
//    private void loadList()
//    {
//        state = 1;
//        downloadDuration = 0;
//        currentIndex = 0;
//    }
//    private void loadStateChange(LoadState newState, ulong process = 0, float speed = 0, ulong size = 0)
//    {
//        msgState.LoadState = newState;
//        msgState.Prcess = process;
//        msgState.Speed = speed;
//        msgState.ItemDownloadSize = size;
//        stateChangeHandle(msgState);
//    }
//    public void Update()
//    {
//        req.Update();
//        loadListUpdate();
//    }
//    /// <summary>
//    /// 切换到后台是保存下载进度
//    /// </summary>
//    /// <param name="pause"></param>
//    public void OnPause(bool pause)
//    {
//        if (pause)
//        {
//            switch (state)
//            {
//                case 4:
//                case 5:
//                case 6:
//                case 7:
//                    {
//                        AbHelp.SaveConfigInfoClient(false);
//                        break;
//                    }
//            }
//        }
//    }
//    private void loadListUpdate()
//    {
//        if (state > 0)
//        {
//            switch (state)
//            {
//                //从远程下载配置
//                case 1:
//                    {
//                        loadStateChange(LoadState.UpdateConfig);
//                        req.Load(AbHelp.GetUrl(AbHelp.AbConfigName), loadRemoteConfigCallback);
//                        state = 2;
//                        break;
//                    }
//                //通用等待
//                case 2:
//                    {

//                        break;
//                    }
//                //下载完成后写入本地
//                case 3:
//                    {
//                        string path = AbHelp.GetWritePath(AbHelp.AbConfigName, true);
//                        File.WriteAllBytes(path, tmpResult);
//                        tmpResult = null;
//                        state = 4;
//                        break;
//                    }
//                //更新AB，下载AB
//                case 4:
//                    {
//                        loadStateChange(LoadState.UpdateAsset);
//                        string url = AbHelp.GetUrl(taskList[currentIndex].N);
//                        AbHelp.HotFixLog(string.Concat("Update AB:", (currentIndex + 1), ",url:", url));
//                        req.Load(url, updateAssetCallback);
//                        state = 5;
//                        break;
//                    }
//                //更新AB，下载进度
//                case 5:
//                    {
//                        msgState.ItemDownloadSize = req.LoadSize();
//                        downloadDuration += Time.deltaTime;
//                        msgState.LoadState = LoadState.UpdateAsset;
//                        msgState.CurrentDownloadSize = msgState.DownloadSize + msgState.ItemDownloadSize;
//                        msgState.Prcess = (1000 * msgState.CurrentDownloadSize) / msgState.AllSize;
//                        msgState.Speed = msgState.CurrentDownloadSize / downloadDuration;
//                        stateChangeHandle(msgState);
//                        break;
//                    }
//                //通用失败
//                case 6:
//                    {
//                        loadStateChange(LoadState.Fail);
//                        state = 0;
//                        break;
//                    }
//                //更新AB，下载完成写入本地，如果全部完成，
//                case 7:
//                    {
//                        var infoCLient = taskList[currentIndex];
//                        AbHelp.UpdateTask(infoCLient);

//                        string path = AbHelp.GetWritePath(infoCLient.N, true, AbHelp.AbFileExt);
//                        File.WriteAllBytes(path, tmpResult);

//                        msgState.LoadState = LoadState.UpdateAsset;
//                        msgState.ItemDownloadSize = req.LoadSize();
//                        msgState.DownloadSize += msgState.ItemDownloadSize;
//                        msgState.CurrentDownloadSize = msgState.DownloadSize;
//                        msgState.Prcess = (1000 * msgState.CurrentDownloadSize) / msgState.AllSize;
//                        msgState.Speed = msgState.CurrentDownloadSize / downloadDuration;
//                        stateChangeHandle(msgState);
//                        tmpResult = null;

//                        currentIndex++;
//                        if (currentIndex < taskList.Count)
//                        {
//                            state = 4;
//                        }
//                        else
//                        {
//                            AbHelp.SaveConfigInfoClient(true);
//                            taskList.Clear();
//                            state = 8;
//                        }
//                        break;
//                    }
//                //读取AB配置
//                case 8:
//                    {
//                        loadStateChange(LoadState.ReadConfig);
//                        string path = AbHelp.GetWritePath(AbHelp.AbConfigName);
//                        if (File.Exists(path))
//                        {
//                            AbHelp.HotFixLog(string.Concat("load rw path:", path));
//                            tmpResult = File.ReadAllBytes(path);
//                            state = 9;
//                        }
//                        else
//                        {
//                            path = AbHelp.GetStreamingAssetsPath(AbHelp.AbConfigName, null, false);
//                            AbHelp.HotFixLog(string.Concat("load buildin path :", path));
//                            req.Load(path, readConfigCallback);
//                            state = 2;
//                        }
//                        break;
//                    }
//                //读取AB配置完成
//                case 9:
//                    {
//                        AbHelp.AbConfig = JsonUtility.FromJson<ResManifest>(System.Text.Encoding.UTF8.GetString(tmpResult));
//                        loadStateChange(LoadState.Complete);
//                        state = 0;
//                        tmpResult = null;
//                        break;
//                    }
//            }
//        }
//    }
//    private void loadRemoteConfigCallback(byte[] data)
//    {
//        if (data == null || data.Length == 0)
//        {
//            msgState.ErrorCode = "#FF03";
//            state = 6;
//        }
//        else
//        {
//            state = 3;
//            tmpResult = data;
//        }
//    }

//    private void updateAssetCallback(byte[] data)
//    {
//        if (data == null || data.Length == 0)
//        {
//            msgState.ErrorCode = "#FF04";
//            state = 6;
//        }
//        else
//        {
//            state = 7;
//            tmpResult = data;
//        }
//    }
//    private void readConfigCallback(byte[] data)
//    {
//        if (data == null || data.Length == 0)
//        {
//            msgState.ErrorCode = "#FF05";
//            state = 6;
//        }
//        else
//        {
//            state = 9;
//            tmpResult = data;
//        }
//    }
//    public void Clear()
//    {
//        taskList.Clear();
//        tmpResult = null;
//        state = 0;
//        downloadDuration = 0;
//        currentIndex = 0;
//        msgState = null;
//    }
//}
//public sealed class LoadStateHandle
//{
//    public LoadStateHandle()
//    {
//    }
//    public Action<UserClick> SelectAction = null;
//    public LoadState LoadState = LoadState.None;
//    public float Speed = 0;
//    //process: 0 - 1000;
//    public ulong Prcess = 0;
//    public ulong ItemDownloadSize = 0, CurrentDownloadSize = 0, AllSize = 0, DownloadSize = 0;
//    public string ErrorCode;
//    public void Continue(UserClick next)
//    {
//        if (SelectAction != null)
//        {
//            SelectAction(next);
//        }
//    }
//}
//public enum LoadState
//{
//    None,
//    LoadLocalConfigInfo,
//    LoadRemoteHotFixInfo,
//    MinVerConfirm,
//    LoadRemoteConfigInfo,
//    UpdateConfig,
//    NetConfirm,
//    UpdateAsset,
//    ReadConfig,
//    Complete,
//    Fail
//}
//public enum UserClick
//{
//    None,
//    Ok,
//    Cancel,
//    Skip
//}