/*
 * AB 的内存占用：
 * Other:SerializedFile
 * Asset:具体资源
 * Not Saved:AB包已经加载的占用
 * Scene Memory:引用信息
 */

using System;
using System.Collections.Generic;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.AssetBundleManager.Runtime;
using CenturyGame.AssetBundleManager.Runtime.Diagnostics;
using CenturyGame.LoggerModule.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;
using Object = UnityEngine.Object;
#if DEBUG_FILE_CRYPTION
using File = CenturyGame.Core.IO.File;
#else
using File = System.IO.File;
using System.IO;
#endif
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


/// <summary>
/// AB系统管理器类具体实现
/// </summary>
public class ABMgrHandle : MonoBehaviour
{
    private static readonly Lazy<ILogger> s_mLogger = new Lazy<ILogger>(() =>
        LoggerManager.GetLogger("ABMgrHandle"));

    protected static ABMgrHandle mInstance;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
        mInstance = this;
#if ENABLE_ASSET_DEBUG_VIEW
        gameObject.AddComponent<AssetBundleManagerDebugView>();
#endif
    }

    public enum InitialState
    {
        UnKnow,

        StartInit,

        LoadingResManifest,

        BeforeInitialized,

        Initialized,
    }

    public InitialState InitState { private set; get; } = InitialState.UnKnow;

    protected static ABConfigHandle ABConfigOperate;

    private static Action onCompletedEvent;
    private static Action<ResourceLoadInitError> onInitErrorEvent;

    public static void Init(Action onInitCompleted = null, Action<ResourceLoadInitError> onInitError = null)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            onCompletedEvent = onInitCompleted;
            onInitErrorEvent = onInitError;
            if (mInstance.InitState == InitialState.UnKnow)
            {
                mInstance.InitState = InitialState.StartInit;
            }
        }
        else
        {
            mInstance.InitState = InitialState.Initialized;
#if ENABLE_ASSET_DEBUG_VIEW
            var debugView = mInstance.gameObject.AddComponent<AssetBundleManagerDebugView>();
            debugView.Init(null);
#endif
            onInitCompleted?.Invoke();
        }
#else
        onCompletedEvent = onInitCompleted;
        onInitErrorEvent = onInitError;
        if (mInstance.InitState == InitialState.UnKnow)
        {
            mInstance.InitState = InitialState.StartInit;
        }
#endif

        SceneManager.sceneLoaded += (scene, loadMode) =>
        {
            s_mLogger.Value.Debug($"Scene load , scene name : {scene.name} , loadMode : {loadMode} .");
        };

        SceneManager.sceneUnloaded += scene =>
        {
            s_mLogger.Value.Debug($"Scene unload , scene name : {scene.name}  .");
        };
    }



#if UNITY_EDITOR
    private static Dictionary<string, ABRequest> editorCacheList = new Dictionary<string, ABRequest>();
    private static void initEditorCacheList()
    {
        if(editorCacheList.Count == 0)
        { 
            System.IO.DirectoryInfo resources = new System.IO.DirectoryInfo(String.Concat(System.Environment.CurrentDirectory, System.IO.Path.DirectorySeparatorChar, AbHelp.ResourcesPath));
            System.IO.FileInfo[] allFiles = resources.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < allFiles.Length; i++)
            {
                System.IO.FileInfo info = allFiles[i];
                if (info.Extension != ".meta")
                {
                    string assetPath = info.FullName.Substring(resources.FullName.Length + 1);
#if UNITY_EDITOR_WIN
                    assetPath = assetPath.Replace(System.IO.Path.DirectorySeparatorChar, '/');
#endif
                    string pathName = assetPath.Substring(0, assetPath.Length - info.Extension.Length);
                    ABRequest item = new ABRequest(pathName, string.Concat(AbHelp.ResourcesPath, "/", assetPath));
                    item.IsDone = true;
                    editorCacheList[pathName] = item;
                }
            }
        }
    }
    private static ABRequest getABRequest(string path)
    {
        initEditorCacheList();
        
        ABRequest result = null;
        if (editorCacheList.ContainsKey(path))
        {
            result = editorCacheList[path];
            result.AddUseCount();
        }
        return result;
    }
    private static bool abExist(string path)
    {
        initEditorCacheList();
        return editorCacheList.ContainsKey(path);
    }
#else
    private static ABRequest getABRequest(string sceneName)
    {
        ABRequest result = new ABRequest(null, sceneName);
        result.IsDone = true;
        return result;
    }
    private static bool abExist(string sceneName)
    {
        return true;
    }
#endif
    public static bool ABExist(string path)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            return ABConfigOperate.RequestExist(path);
        }
        else
        {
            return abExist(path);
        }
#else
        return ABConfigOperate.RequestExist(path);
#endif
    }
    public static Object Load(string path)
    {
        path = path.ToLower();
        ABRequest req = LoadAB(path);
        if (req == null)
        {
            s_mLogger.Value?.Error($"Load asset failure , path : \"{path}\" .");
            return null;
        }
        else
        {
            return req.MainAsset;
        }
    }

    public static void LoadAsync(string path, ObjectCallBack callback)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            loadABAsync(path, delegate (ABRequest req)
            {
                if (req != null)
                    req.LoadAsync(callback);
                else
                    callback?.Invoke(null);
            });
        }
        else
        {
            ABRequest req = getABRequest(path);
            if (req == null)
            {
                s_mLogger.Value?.Error($"Load asset failure , path : \"{path}\" .");
                callback(null);
            }
            else
            {
                Object obj = req.MainAsset;
                callback(obj);
            }
        }
#else
        loadABAsync(path, delegate (ABRequest req)
        {
            if (req != null)
                req.LoadAsync(callback);
            else
                callback?.Invoke(null);
        });
#endif
    }

    public static Object Load(string path, System.Type type)
    {
        ABRequest req = LoadAB(path);
        if (req == null)
        {
            s_mLogger.Value?.Error($"Load asset failure , path : \"{path}\" , asset type : \"{type.Name}\" .");
            return null;
        }
        else
        {
            return req.Load(type);
        }
    }

    public static void LoadAsync(string path, System.Type type, ObjectCallBack callback)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            loadABAsync(path, delegate (ABRequest req)
            {
                if (req != null)
                    req.LoadAsync(callback, type);
                else
                    callback?.Invoke(null);
            });
        }
        else
        {
            ABRequest req = getABRequest(path);
            if (req == null)
            {
                s_mLogger.Value?.Error($"Load asset failure , path : \"{path}\" .");
                callback(null);
            }
            else
            {
                Object obj = req.Load(type);
                callback(obj);
            }
        }
#else
        loadABAsync(path, delegate (ABRequest req)
        {
            if (req != null)
                req.LoadAsync(callback);
            else
                callback?.Invoke(null);
        });
#endif
    }

    public static T LoadFromResource<T>(string path) where T : Object
    {
        Object obj = Resources.Load<T>(path);
        if (obj == null) {
            s_mLogger.Value?.Error("ABMgrHandle Load Fail: " + path);
        }
        return (T)obj;
    }

    public static T Load<T>(string path, string defaultPath, out bool exist, out ABRequest request) where T : Object
    {
        request = LoadAB(path);
        if (request == null)
        {
            exist = false;
            if (defaultPath == null)
            {
                s_mLogger.Value?.Error(path);
            }
            else
            {
                request = LoadAB(defaultPath);
                if (request == null)
                {
                    s_mLogger.Value?.Error(path);
                }
                else
                {
                    return request.Load<T>();
                }
            }
            return default(T);
        }
        else
        {
            exist = true;
            return request.Load<T>();
        }
    }
    
    public static void LoadAndInit(string path, ABObjectCallBack callback, Vector3 pos, Vector3 rot, bool useDefaultPos = false)
    {
        ABRequest req = LoadAB(path);
        if (req != null)
        {
            req.LoadABObject(callback, pos, rot, useDefaultPos);
        }
        else
        {
            s_mLogger.Value?.Debug($"LoadAndInit failure ! Path : \"{path}\" .");
            callback(null);
        }
    }

    public static void LoadAndInitAsync(string path, ABObjectCallBack callback, Vector3 pos, Vector3 rot, bool useDefaultPos = false)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            loadABAsync(path, delegate (ABRequest req)
            {
                if (req != null)
                {
                    req.LoadAsync(delegate (UnityEngine.Object asset)
                    {
                        if (asset != null)
                        {
                            GameObject go = ABRequest.InitObj(req, pos, rot, useDefaultPos, asset);
                            callback(go);
                        }
                        else
                            callback?.Invoke(null);
                    }, null, false);
                }
                else
                    callback?.Invoke(null);
            });
        }
        else
        {
            ABRequest req = getABRequest(path);
            if (req != null)
                req.LoadABObject(callback, pos, rot, useDefaultPos);
            else
                callback?.Invoke(null);
        }
#else
        loadABAsync(path, delegate (ABRequest req)
        {
            if (req != null)
            {
                req.LoadAsync(delegate (UnityEngine.Object asset)
                {
                    if (asset != null)
                    {
                        GameObject go =  ABRequest.InitObj(req, pos, rot, useDefaultPos, asset);
                        callback(go);
                    }
                    else
                        callback?.Invoke(null);
                }, null, false);
            }
            else
                callback?.Invoke(null);
        });
#endif
    }

    public static void LoadAndInit(string path, ABObjectCallBack callback, bool useDefaultPos = false)
    {
        LoadAndInit(path, callback, Vector3.zero, Vector3.zero, useDefaultPos);
    }

    public static void LoadAndInitAsync(string path, ABObjectCallBack callback, bool useDefaultPos = false)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            loadABAsync(path, delegate (ABRequest req)
            {
                if (req != null)
                {
                    req.LoadAsync(delegate (UnityEngine.Object asset)
                    {
                        if (asset != null)
                        {
                            GameObject go = ABRequest.InitObj(req, Vector3.zero, Vector3.zero, useDefaultPos, asset);
                            callback(go);
                        }
                        else
                            callback?.Invoke(null);
                    }, null, false);
                }
                else
                    callback?.Invoke(null);
            });
        }
        else
        {
            ABRequest req = getABRequest(path);
            if (req != null)
                req.LoadABObject(callback, Vector3.zero, Vector3.zero, useDefaultPos);
            else
                callback?.Invoke(null);
        }
#else
        loadABAsync(path, delegate (ABRequest req)
        {
            if (req != null)
            {
                req.LoadAsync(delegate (UnityEngine.Object asset)
                {
                    if (asset != null)
                    {
                        GameObject go = ABRequest.InitObj(req, Vector3.zero, Vector3.zero, useDefaultPos, asset);
                        callback(go);
                    }
                    else
                        callback?.Invoke(null);
                }, null, false);
            }
            else
                callback?.Invoke(null);
        });
#endif
    }

    public static GameObject LoadAndInit(string path, Vector3 pos, Vector3 rot, bool useDefaultPos = false)
    {
        ABRequest req = LoadAB(path);
        if (req == null)
        {
            s_mLogger.Value?.Debug($"LoadAndInit gameobject failure ! Path : \"{path}\" .");
            return null;
        }
        else
        {
            return ABRequest.InitObj(req, pos, rot, useDefaultPos);
        }
    }
    
    private static ABRequest LoadAB(string path)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            return loadAB(path);
        }
        else
        {
            return getABRequest(path);
        }
#else
        return loadAB(path);
#endif

    }

    private static ABRequest loadAB(string path)
    {
        ABRequest req = ABConfigOperate.GetRequest(path);
        if (req == null)
        {
            return null;
        }
        else
        {
            req.LoadAsset(null, false);
            return req;
        }
    }

    private static void loadABAsync(string path, ABRequestCallBack callback)
    {
        ABRequest req = ABConfigOperate.GetRequest(path);
        if (req != null)
        {
            req.LoadAsset(callback, true);
        }
        else
        {
            Debug.LogErrorFormat("get ABRequest fail, path = {0}", path);
            callback?.Invoke(null);
        }
    }
    
    private static byte[] LoadLua(string path)
    {
        byte[] dat = null;
        ABRequest result = ABConfigOperate.GetRequest(path);
        if(result != null)
        {
            result.LoadAsset(null, false);
            if (result.ABHandle.AB != null)
            {
                TextAsset txt = result.MainAsset as TextAsset;
                if (txt != null)
                    dat = txt.bytes;
            }
        }
        return dat;
    }
    
    private static ABRequest loadSceneBase(string sceneName, LoadSceneMode mode = LoadSceneMode.Single , bool isAsync = false , ABRequestCallBack callback = null)
    {
        string scenePath = $"{ABRequest.SCENE_FOLDER_NAME}/{sceneName}".ToLower();
        ABRequest result = ABConfigOperate.GetRequest(scenePath);
        if(result == null)
        {
            s_mLogger.Value?.Debug($"loadSceneBase failure ! scenePath : \"{scenePath}\" .");
        }
        else
        {
            result.LoadScene(callback, isAsync, mode);
        }
        return result;
    }

    private static ABRequest unLoadSceneBase(string sceneName)
    {
        string scenePath = $"{ABRequest.SCENE_FOLDER_NAME}/{sceneName}".ToLower();
        ABRequest result = ABConfigOperate.GetRequest(scenePath);
        if (result == null)
        {
            s_mLogger.Value?.Debug($"unLoadSceneBase failure ! scenePath : \"{scenePath}\" .");
        }
        else
        {
            result.Delete();
        }
        return result;
    }

    public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            loadSceneBase(sceneName, mode);
            SceneManager.LoadScene(sceneName, mode);
        }
        else
        {
            string scenePath = GetScenePathRelativeProject(sceneName);

            if (!File.Exists(scenePath))
            {
                throw new FileNotFoundException($"The scene that name is \"{sceneName}\" is not found !");
            }
            EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters(mode));
        }
#else
        loadSceneBase(sceneName, mode);
        SceneManager.LoadScene(sceneName, mode);
#endif
    }

    [Obsolete("Use ABMgrHandle.LoadSceneAsync . This function is not safe.")]
    public static void UnLoadScene(string sceneName)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            unLoadSceneBase(sceneName);
            SceneManager.UnloadScene(sceneName);
        }
        else
        {
            //string scenePath = $"{Application.dataPath}/ResourcesAB/{ABRequest.SCENE_FOLDER_NAME}/{sceneName}.unity";
            //if (!File.Exists(scenePath))
            //{
            //    throw new FileNotFoundException($"The scene that name is \"{sceneName}\" is not found !");
            //}
            SceneManager.UnloadScene(sceneName);
        }
#else
        unLoadSceneBase(sceneName);
        SceneManager.UnloadScene(sceneName);
#endif
    }


    public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            loadSceneBase(sceneName, mode);

            return SceneManager.LoadSceneAsync(sceneName, mode);
        }
        else
        {
            string scenePath = GetScenePathRelativeProject(sceneName);
            if (!File.Exists(scenePath))
            {
                throw new FileNotFoundException($"The scene that name is \"{sceneName}\" is not found !");
            }
            return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(mode));
        }
#else
        loadSceneBase(sceneName, mode);
        return SceneManager.LoadSceneAsync(sceneName, mode);
#endif

    }

    public static AsyncOperation UnLoadSceneAsync(string sceneName)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            unLoadSceneBase(sceneName);
            return SceneManager.UnloadSceneAsync(sceneName);
        }
        else
        {
            return SceneManager.UnloadSceneAsync(sceneName);
        }
#else
            unLoadSceneBase(sceneName);
            return SceneManager.UnloadSceneAsync(sceneName);
#endif
    }

#if UNITY_EDITOR

    private static string GetScenePathRelativeProject(string sceneName)
    {
        string scenePath = $"Assets/ResourcesAB/{ABRequest.SCENE_FOLDER_NAME}/{sceneName}.unity";
        s_mLogger.Value.Info($"scenePath : \"{scenePath}\" .");
        return scenePath;
    }

#endif

    public static void Delete(string path)
    {
#if UNITY_EDITOR
        if (!AssetBundleManager.SimulateAssetBundleInEditor)
        {
            if (path != null)
            {
                ABRequest requeset = ABConfigOperate.GetRequestFormLoaded(path);
                if (requeset != null && requeset.IsDone)
                    requeset.Delete();
            }
        }
        else
        {
            s_mLogger.Value?.Warn($"Delete operation is not implemention in editor load mode!");
        }
#else
        if (path != null)
        {
            ABRequest requeset = ABConfigOperate.GetRequestFormLoaded(path);
            if (requeset != null && requeset.IsDone)
                requeset.Delete();
        }
#endif

    }

    public static void Delete(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            ABRequest.DeleteObj(obj);
        }
    }
    
    public static ABItem GetItem(string path)
    {
        return ABConfigOperate.GetItem(path);
    }

    private static void loadConfig(ResManifest conf)
    {
        ABConfigOperate = new ABConfigHandle(conf);

#if ENABLE_ASSET_DEBUG_VIEW
        var debugView = mInstance.gameObject.AddComponent<AssetBundleManagerDebugView>();
        debugView.Init(ABConfigOperate);
#endif
    }
    private static void initConfig()
    {
        //#if UNITY_EDITOR
        //        if (AbHelp.AbConfig == null)
        //        {
        //            string path = AssetsFileSystem.GetStreamingAssetsPath(AbHelp.AbConfigName, null, false);
        //            AbHelp.AbConfig = JsonUtility.FromJson<ResManifest>(System.Text.Encoding.UTF8.GetString(System.IO.File.ReadAllBytes(path)));
        //        }
        //#endif
        loadConfig(AbHelp.AbConfig);
    }

    //public bool IsDebug = false;
    //public bool IsUseAB = false;
    //public bool IsDone = false;

    void Update()
    {
        if (this.InitState == InitialState.StartInit)
        {
            this.LoadAbConfig();
        }
        else if (this.InitState == InitialState.BeforeInitialized)
        {
            initConfig();
            this.InitState = InitialState.Initialized;
            onCompletedEvent?.Invoke();
            onCompletedEvent = null;

            //if (!AbHelp.IsDone)
            //{
               
            //    AbHelp.IsDone = true;
            //}
        }

        this.mRequest.Update();
        if (ABConfigOperate != null)
            ABConfigOperate.UpdateItem();
        //updateParameter();
    }
    //protected void updateParameter()
    //{
    //    if (IsDone != AbHelp.IsDone)
    //        IsDone = AbHelp.IsDone;
    //    if (IsDebug != AbHelp.IsDebug)
    //        AbHelp.IsDebug = IsDebug;
    //    if (IsUseAB != AbHelp.IsUseAB)
    //        AbHelp.IsUseAB = IsUseAB;
    //}

    private readonly HttpRequest mRequest = new HttpRequest();
    private void LoadAbConfig()
    {
        this.InitState = InitialState.LoadingResManifest;
        ReadFileList();
    }

    private void ReadFileList()
    {
        string path = AssetsFileSystem.GetWritePath(AssetsFileSystem.UnityABFileName);
        if (File.Exists(path))//编辑器下有可能不会有这个文件
        {
            s_mLogger.Value?.Info(string.Concat("Load file list path:", path));

            var bytes = File.ReadAllBytes(path);
            this.ReadAbConfigCompleted(bytes);
        }
        else
        {
            path = AssetsFileSystem.GetStreamingAssetsPath(AssetsFileSystem.UnityABFileName, null, false);
            s_mLogger.Value?.Info(string.Concat("load buildin file list path :", path));
            this.mRequest.Load(path, readConfigCallback);
        }
    }

    private void readConfigCallback(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            onInitErrorEvent?.Invoke(ResourceLoadInitError.LoadResManifestFailure);
        }
        else
        {
            this.ReadAbConfigCompleted(data);
        }
    }

    private void ReadAbConfigCompleted(byte[] bytes)
    {
        AbHelp.AbConfig = JsonUtility.FromJson<ResManifest>(new System.Text.UTF8Encoding(false,true).GetString(bytes));
        this.InitState = InitialState.BeforeInitialized;
    }

}

