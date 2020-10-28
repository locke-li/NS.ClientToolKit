using System;
using UnityEngine;
using CenturyGame.Framework.UI;
using CenturyGame.Framework.Network;
using CenturyGame.Framework.Lua;
using CenturyGame.Framework.Http;
using CenturyGame.AssetBundleManager.Runtime;
using CenturyGame.LoggerModule.Runtime;
using CenturyGame.Log4NetForUnity.Runtime;


namespace CenturyGame.Framework
{
    using AssetBundleManager = AssetBundleManager.Runtime.AssetBundleManager;
    public class GameLauncher : MonoBehaviour
    {
        public string GameEntryTypeName;

        public IGameEntry GameEntry { get; private set; }

        public static UIManager UI
        {
            get;
            private set;
        }

        public static NetworkManager Network
        {
            get;
            private set;
        }

        public static LuaManager Lua
        {
            get;
            private set;
        }

        public static HttpManager Http
        {
            get;
            private set;
        }

        private void Start()
        {
            LoggerManager.SetCurrentLoggerProvider(new Log4NetLoggerProvider());
            InitFrameworkModules();
            AssetBundleManager.Initialize(OnABMgrInited, OnABMgrInitFailed);
        }

        private void Update()
        {
            FrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
            GameEntry?.Update();
        }

        private void LateUpdate()
        {
            FrameworkEntry.LateUpdate();
            GameEntry?.LateUpdate();
        }

        private void FixedUpdate()
        {
            GameEntry?.FixedUpdate();
        }

        private void OnApplicationFocus(bool focus)
        {
            GameEntry?.OnApplicationFocus(focus);
        }

        private void OnApplicationPause(bool pause)
        {
            GameEntry?.OnApplicationPause(pause);
        }

        private void OnApplicationQuit()
        {
            FrameworkEntry.ShutDown();
            GameEntry?.QuitGame();
            LoggerManager.Shutdown();
        }

        private void InitFrameworkModules()
        {
            Lua = FrameworkEntry.GetModule<LuaManager>();
            UI = FrameworkEntry.GetModule<UIManager>();
            Network = FrameworkEntry.GetModule<NetworkManager>();
            Http = FrameworkEntry.GetModule<HttpManager>();
        }

        private bool CreateGameEntry(string typeName)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                var ti = typeof(IGameEntry);
                var assemble = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var asm in assemble)
                {
                    foreach (var t in asm.GetTypes())
                    {
                        if (!ti.IsAssignableFrom(t) || !t.IsClass || t.IsAbstract || t.IsGenericType) continue;
                        if (t.FullName.Equals(GameEntryTypeName))
                        {
                            GameEntry = Activator.CreateInstance(t) as IGameEntry;
                            return true;
                        }
                    }
                }
                Debug.LogErrorFormat("无法创建游戏入口, 游戏入口类型={0}", GameEntryTypeName);
            }
            else
                Debug.LogError("未设置游戏入口类型");
            return false;
        }

        private void OnABMgrInited()
        {
            if (CreateGameEntry(GameEntryTypeName))
            {
                GameEntry?.StartGame();
            }
        }

        private void OnABMgrInitFailed(ResourceLoadInitError errCode)
        {
            Debug.LogErrorFormat("AssetBundleManager初始化失败, error code = {0}", errCode);
        }
    }
}