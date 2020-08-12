//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.SceneManagement;

//public class AssetHotFix : MonoBehaviour
//{
//    private AssetHotFixHandle handle = null;
//    bool isPaused = false;

//    void Start()
//    {
//        handle = new AssetHotFixHandle();
//        handle.LoadFileList(OnStateChange);
//    }

//    private void Update()
//    {
//        if (handle != null)
//        {
//            handle.Update();
//        }
//    }

//    private void OnDestroy()
//    {
//        if (handle != null)
//        {
//            handle.Clear();
//        }
//    }
//    void OnApplicationFocus(bool hasFocus)
//    {
//        isPaused = !hasFocus;
//        if (handle != null)
//            handle.OnPause(isPaused);
//    }

//    void OnApplicationPause(bool pauseStatus)
//    {
//        isPaused = pauseStatus;
//        if (handle != null)
//            handle.OnPause(isPaused);
//    }

//    public UnityEngine.UI.Text MainInfo;
//    public UnityEngine.UI.Text UpdateTitle;

//    public UnityEngine.UI.Text NextText;
//    public UnityEngine.UI.Text SkipText;
//    public GameObject UpdateWindow;

//    public void NextFunc()
//    {
//        stateHandle.Continue(UserClick.Ok);
//        UpdateWindow.SetActive(false);
//    }

//    public void SkipFunc()
//    {
//        stateHandle.Continue(UserClick.Skip);
//        UpdateWindow.SetActive(false);
//    }

//    private void OnGUI()
//    {
//        if (isPaused)
//            GUI.Label(new Rect(100, 100, 50, 30), "Game paused");
//        if (stateHandle == null)
//        {
//            return;
//        }

//        switch (stateHandle.LoadState)
//        {
//            case LoadState.None:
//                {
//                    MainInfo.text = stateHandle.LoadState.ToString();
//                    break;
//                }
//            case LoadState.LoadLocalConfigInfo:
//                {
//                    MainInfo.text = stateHandle.LoadState.ToString();
//                    break;
//                }
//            case LoadState.MinVerConfirm:
//                {
//                    GUI.Window(0, new Rect((Screen.width - 600) / 2, (Screen.height - 300) / 2, 600, 300), minVerConfirm, "+检测版本更新+");
//                    MainInfo.text = stateHandle.LoadState.ToString();
//                    break;
//                }
//            case LoadState.LoadRemoteConfigInfo:
//                {
//                    MainInfo.text = stateHandle.LoadState.ToString();
//                    break;
//                }
//            case LoadState.NetConfirm:
//                {
//                    GUI.Window(0, new Rect((Screen.width - 600) / 2, (Screen.height - 300) / 2, 600, 300), netConfirm, "+检测资源更新+");
//                    MainInfo.text = stateHandle.LoadState.ToString();
//                    break;
//                }
//            case LoadState.UpdateConfig:
//                {
//                    MainInfo.text = stateHandle.LoadState.ToString();
//                    break;
//                }
//            case LoadState.UpdateAsset:
//                {
//                    GUI.Window(0, new Rect((Screen.width - 600) / 2, (Screen.height - 300) / 2, 600, 300), updateAsset, "+更新资源+");

//                    StringBuilder tmp = new StringBuilder();
//                    tmp.Append("需下载大小:");
//                    tmp.Append(AbHelp.GetSizeStr(stateHandle.AllSize));
//                    tmp.Append("已下载:");
//                    tmp.Append(AbHelp.GetSizeStr(stateHandle.CurrentDownloadSize));
//                    tmp.Append("\r\n");
//                    tmp.Append("下载速度:");
//                    tmp.Append(string.Concat(AbHelp.GetSizeStr(stateHandle.Speed), "/S"));
//                    tmp.Append("当前进度:");
//                    tmp.Append(string.Concat(stateHandle.Prcess / 10, "%"));
//                    MainInfo.text = tmp.ToString();
//                    break;
//                }

//            case LoadState.Complete:
//                {
//                    MainInfo.text = stateHandle.LoadState.ToString();
//                    break;
//                }
//            case LoadState.Fail:
//                {
//                    GUI.Window(0, new Rect((Screen.width - 600) / 2, (Screen.height - 300) / 2, 600, 300), extGame, "哎呀，更新出错啦！");
//                    MainInfo.text = string.Format("请检查网络{0}，稍后再试！", stateHandle.ErrorCode);
//                    break;
//                }
//        }
//        GUILayout.Label(stateHandle.LoadState.ToString());
//    }

//    void minVerConfirm(int windowid)
//    {
//        GUILayout.Label("热更新系统检测有新版本，是否更新？", GUILayout.Height(100));
//        GUILayout.BeginHorizontal();
//        if (GUILayout.Button("下载最新版本", GUILayout.Height(100)))
//        {
//            stateHandle.Continue(UserClick.Ok);
//        }
//        if (GUILayout.Button("跳过最新版本", GUILayout.Height(100)))
//        {
//            stateHandle.Continue(UserClick.Skip);
//        }
//        GUILayout.EndHorizontal();
//        定义窗体可以活动的范围
//        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
//    }

//    void netConfirm(int windowid)
//    {
//        GUILayout.Label(string.Format("热更新系统检测到需要下载{0}资源，是否更新？", AbHelp.GetSizeStr(stateHandle.AllSize)), GUILayout.Height(100));
//        GUILayout.BeginHorizontal();
//        if (GUILayout.Button("下载最新资源", GUILayout.Height(100)))
//        {
//            stateHandle.Continue(UserClick.Ok);
//        }
//        if (GUILayout.Button("跳过下载最新资源", GUILayout.Height(100)))
//        {
//            stateHandle.Continue(UserClick.Skip);
//        }
//        GUILayout.EndHorizontal();
//        定义窗体可以活动的范围
//        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
//    }

//    void extGame(int windowid)
//    {
//        GUILayout.Label(string.Format("请检查网络{0}，稍后再试！", stateHandle.ErrorCode), GUILayout.Height(100));
//        GUILayout.BeginHorizontal();
//        if (GUILayout.Button("确定", GUILayout.Height(100)))
//        {
//            Application.Quit();
//        }
//        GUILayout.EndHorizontal();
//        定义窗体可以活动的范围
//        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
//    }

//    void updateAsset(int windowid)
//    {
//        GUILayout.BeginHorizontal();
//        GUILayout.Label("需下载大小:");
//        GUILayout.Label(AbHelp.GetSizeStr(stateHandle.AllSize));
//        GUILayout.EndHorizontal();

//        GUILayout.BeginHorizontal();
//        GUILayout.Label("已下载:");
//        GUILayout.Label(AbHelp.GetSizeStr(stateHandle.CurrentDownloadSize));
//        GUILayout.EndHorizontal();

//        GUILayout.BeginHorizontal();
//        GUILayout.Label("下载速度:");
//        GUILayout.Label(string.Concat(AbHelp.GetSizeStr(stateHandle.Speed), "/S"));
//        GUILayout.EndHorizontal();

//        GUILayout.BeginHorizontal();
//        GUILayout.Label("当前进度:");
//        GUILayout.Label(string.Concat(stateHandle.Prcess / 10, "%"));
//        GUILayout.EndHorizontal();

//        GUILayout.HorizontalSlider(stateHandle.Prcess, 0, 1000, GUILayout.Height(100));
//    }

//    private LoadState lastState = LoadState.None;
//    private LoadStateHandle stateHandle = null;
//    private void OnStateChange(LoadStateHandle e)
//    {
//        if (lastState != e.LoadState)
//        {
//            AbHelp.HotFixLog("Hotfix state change:" + e.LoadState);
//            lastState = e.LoadState;

//            switch (lastState)
//            {
//                case LoadState.Complete:
//                    {
//                        ABMgr.Init();
//                        if (AbHelp.IsFirstRun)
//                        {
//#if M5_MAP_USE_AB
//                        CopyFiles();
//#endif
//                        }
//                        if (NextScene)
//                        {
//#if M5_ONLYBIGWORLD
//                                                        ABMgr.LoadScene("Entry", LoadSceneMode.Single);
//#else
//                            SceneManager.LoadScene("Entry", LoadSceneMode.Single);
//#endif
//                        }
//                        break;
//                    }
//                case LoadState.MinVerConfirm:
//                    {
//                        UpdateWindow.SetActive(true);
//                        UpdateTitle.text = "热更新系统检测有新版本，是否更新？";
//                        NextText.text = "下载最新版本";
//                        SkipText.text = "跳过最新版本";
//                        break;
//                    }
//                case LoadState.NetConfirm:
//                    {
//                        UpdateWindow.SetActive(true);
//                        UpdateTitle.text = string.Format("热更新系统检测到需要下载{0}资源，是否更新？", AbHelp.GetSizeStr(stateHandle.AllSize));
//                        NextText.text = "下载最新资源";
//                        SkipText.text = "跳过下载资源";
//                        break;
//                    }
//                case LoadState.Fail:
//                    {
//                        UpdateWindow.SetActive(true);
//                        UpdateTitle.text = string.Format("请检查网络{0}，稍后再试！", stateHandle.ErrorCode);
//                        NextText.gameObject.SetActive(false);
//                        SkipText.gameObject.SetActive(false);
//                        break;
//                    }
//            }
//        }
//        stateHandle = e;
//    }
//    public static bool NextScene = true;

//    private void CopyFiles()
//    {
//        Debug.Log("Start copyfile");
//        string naviBaseFilePath = "MapData/MapLogicData/CopyCollideFiles";
//        TextAsset config = ABMgr.Load<TextAsset>(naviBaseFilePath);
//        if (config != null)
//        {
//            var stream = new MemoryStream(config.bytes);
//            BinaryReader reader = new BinaryReader(stream);
//            int Row = reader.ReadInt32();
//            List<string> testList = new List<string>();
//            for (int i = 0; i < Row; i++)
//            {
//                testList.Add(reader.ReadString());
//            }
//            stream.Close();

//            string confStr = config.text;
//            string[] lines = confStr.Split(new char[] { '\r' });
//            foreach (var f in testList)
//            {
//                CopyFile(f);
//            }
//        }
//        ABMgr.Delete(naviBaseFilePath);
//    }

//    private void CopyFile(string file)
//    {
//        Debug.Log("copy file, source file name: " + file);
//        TextAsset sourceFile = ABMgr.Load<TextAsset>(file);
//        if (sourceFile != null)
//        {
//            string targetPath = AbHelp.GetWritePath(string.Concat("CopyMapData/", Path.GetFileName(file)), true);
//            Debug.Log("copy file, target path: " + targetPath);
//            File.WriteAllBytes(targetPath, sourceFile.bytes);
//        }
//        ABMgr.Delete(file);
//    }
//}
