/***************************************************************

 *  类名称：        AssetBundleManager

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/17 14:53:55

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using CenturyGame.LoggerModule.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Object = UnityEngine.Object;

namespace CenturyGame.AssetBundleManager.Runtime
{
    public class AssetBundleManager : MonoBehaviour
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private static readonly Lazy<LoggerModule.Runtime.ILogger> s_mLogger = new Lazy<ILogger>(() =>
            LoggerManager.GetLogger("AssetBundleManager"));

        private static bool s_mInitialize = false;

        private static ABMgrHandle ABMgrHandle = null;

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        /// <summary>
        /// AssetBundle资源加载模块是否已初始化
        /// </summary>
        public static bool IsInitialize => s_mInitialize;

#if UNITY_EDITOR
        /// <summary>
        /// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        /// </summary>
        public static bool SimulateAssetBundleInEditor
        {
            get
            {
                if (m_SimulateAssetBundleInEditor == -1)
                    m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

                return m_SimulateAssetBundleInEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != m_SimulateAssetBundleInEditor)
                {
                    m_SimulateAssetBundleInEditor = newValue;
                    EditorPrefs.SetBool(kSimulateAssetBundles, value);
                }
            }
        }

        static int m_SimulateAssetBundleInEditor = -1;
        const string kSimulateAssetBundles = "SimulateAssetBundles";
#endif

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        /// <summary>
        /// 初始化AssetBundleManager
        /// </summary>
        /// <param name="onInitCompleted">初始化完成回调</param>
        /// <param name="onInitError">初始化异常回调</param>
        public static void Initialize(Action onInitCompleted = null, Action<ResourceLoadInitError> onInitError = null)
        {
            var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
            DontDestroyOnLoad(go);
            ABMgrHandle = go.AddComponent<ABMgrHandle>();
            ABMgrHandle.Init(() =>
                {
                    s_mInitialize = true;
                    onInitCompleted?.Invoke();
                    s_mLogger.Value.Info($"AssetBundleManager initialize completed !");

                },
                error =>
                {
                    onInitError?.Invoke(error);
                    s_mLogger.Value.Error($"AssetBundleManager initialize fail! Error : {error} .");
                });
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="path">资源相对路径。该路径相对于"Assets/ResourcesAB" ，例如：abscene/test，且不带后缀名</param>
        /// <returns></returns>
        public static Object Load(string path)
        {
            CheckIsInitialize("Load1");
            return ABMgrHandle.Load(path);
        }

        public static void LoadAsync(string path, ObjectCallBack callback)
        {
            CheckIsInitialize("LoadAsync1");
            ABMgrHandle.LoadAsync(path, callback);
        }

        public static Object Load(string path, System.Type type)
        {
            CheckIsInitialize("Load2");
            return ABMgrHandle.Load(path, type);
        }

        public static void LoadAsync(string path, System.Type type, ObjectCallBack callBack)
        {
            CheckIsInitialize("LoadAsync2");
            ABMgrHandle.LoadAsync(path, type, callBack);
        }

        public static T LoadFromResourceFolder<T>(string path)
            where T : Object
        {
            CheckIsInitialize("LoadFromResourceFolder");
            return ABMgrHandle.LoadFromResource<T>(path);
        }

        public static void LoadAndInit(string path, ABObjectCallBack callback, Vector3 pos, Vector3 rot,
            bool useDefaultPos = false)
        {
            CheckIsInitialize("LoadAndInit1");
            ABMgrHandle.LoadAndInit(path, callback, pos, rot, useDefaultPos);
        }

        public static void LoadAndInitAsync(string path, ABObjectCallBack callback, Vector3 pos, Vector3 rot, bool useDefaultPos = false)
        {
            CheckIsInitialize("LoadAndInitAsync1");
            ABMgrHandle.LoadAndInitAsync(path, callback, pos, rot, useDefaultPos);
        }

        public static void LoadAndInit(string path, ABObjectCallBack callback, bool useDefaultPos = false)
        {
            CheckIsInitialize("LoadAndInit2");
            ABMgrHandle.LoadAndInit(path, callback, useDefaultPos);
        }

        public static void LoadAndInitAsync(string path, ABObjectCallBack callback, bool useDefaultPos = false)
        {
            CheckIsInitialize("LoadAndInitAsync2");
            ABMgrHandle.LoadAndInitAsync(path, callback, useDefaultPos);
        }

        public static GameObject LoadAndInit(string path, Vector3 pos, Vector3 rot, bool useDefaultPos = false)
        {
            CheckIsInitialize("LoadAndInit3");
            return ABMgrHandle.LoadAndInit(path, pos, rot, useDefaultPos);
        }


        public static void Release(string path)
        {
            CheckIsInitialize("Release path");
            ABMgrHandle.Delete(path);
        }

        public static void Release(Object obj)
        {
            CheckIsInitialize("Release obj");
            ABMgrHandle.Delete(obj);
        }

        public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            CheckIsInitialize("LoadScene");
            ABMgrHandle.LoadScene(sceneName, mode);
        }

        [Obsolete("Use AssetBundleManager.LoadSceneAsync .  This function is not safe . ")]
        public static void UnLoadScene(string sceneName)
        {
            CheckIsInitialize("UnLoadScene");
            ABMgrHandle.UnLoadScene(sceneName);
        }

        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            CheckIsInitialize("LoadSceneAsync");
            return ABMgrHandle.LoadSceneAsync(sceneName, mode);
        }

        public static AsyncOperation UnLoadSceneAsync(string sceneName)
        {
            CheckIsInitialize("UnLoadSceneAsync");
            return ABMgrHandle.UnLoadSceneAsync(sceneName);
        }

        private static void CheckIsInitialize(string methodName)
        {
            if (!s_mInitialize)
                throw new NullReferenceException($"Your want to use \"AssetBuildManager\" that not initialized ! Call method :  \"{methodName}\" .");
        }
        #endregion

    }
}
