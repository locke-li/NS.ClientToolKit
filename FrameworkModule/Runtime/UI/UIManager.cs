using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CenturyGame.Framework.UI
{
    using AssetBundleManager = CenturyGame.AssetBundleManager.Runtime.AssetBundleManager;
    public enum EWindowLayer
    {
        Bottom = 0,
        Background,
        Common,
        Animation,
        Pop,
        Guide,
        Const,
        Top,
    }

    internal class UIPrefabData
    {
        internal string Name;
        internal string Path;
        internal UnityEngine.Object Prefab;
    }

    
    public class UIManager : FrameworkModule
    {
        private Dictionary<EWindowLayer, UIGroup> groupMap = new Dictionary<EWindowLayer, UIGroup>(8);
        private Dictionary<string, UIPrefabData> uiPrefabDic = new Dictionary<string, UIPrefabData>(64);
        private const string defaultPath = "ui/";

        internal override void Init()
        {
            int uiLayer = LayerMask.NameToLayer("UI");
            GameObject uiRoot = new GameObject("UIRoot")
            {
                layer = uiLayer
            };
            int layerCount = (int)EWindowLayer.Top + 1;
            for (int i = 0; i < layerCount; i++)
            {
                EWindowLayer layer = (EWindowLayer)i;
                GameObject layerObj = new GameObject(layer.ToString())
                {
                    layer = uiLayer
                };
                layerObj.transform.SetParent(uiRoot.transform);
                UIGroup uiGroup = new UIGroup(layer, layerObj.transform, i);
                groupMap.Add(layer, uiGroup);
            }

            var eventSys = new GameObject("EventSystem")
            {
                layer = uiLayer
            };
            eventSys.transform.SetParent(uiRoot.transform);
            eventSys.AddComponent<EventSystem>();
            eventSys.AddComponent<StandaloneInputModule>();
            

            Object.DontDestroyOnLoad(uiRoot);
        }

        internal override void Update(float elapseTime, float realElapseTime)
        {
        }

        internal override void LateUpdate()
        {
        }

        internal override void Shutdown()
        {
        }

        /// <summary>
        /// 从默认路径加载界面，并在指定层级创建界面。默认路径:ResourcesAB\ui
        /// </summary>
        /// <param name="uiName">ui名称</param>
        /// <param name="layer">层级</param>
        public void OpenWindow(string uiName, EWindowLayer layer)
        {
            OpenWindow(defaultPath, uiName, layer);
        }

        /// <summary>
        /// 从指定路径加载界面，并在指定层级创建界面
        /// </summary>
        /// <param name="path">ui路径</param>
        /// <param name="uiName">ui名称</param>
        /// <param name="layer">层级</param>
        public void OpenWindow(string path, string uiName, EWindowLayer layer)
        {
            if (!groupMap.ContainsKey(layer))
            {
                Debug.LogError("找不到指定的层级:" + layer);
                return;
            }
            if (!uiPrefabDic.ContainsKey(uiName))
            {
                string prefabPath = string.Concat(path, uiName);
                Object uiPrefab = AssetBundleManager.Load<GameObject>(prefabPath);
                if (uiPrefab != null)
                {
                    UIPrefabData pData = new UIPrefabData
                    {
                        Name = uiName,
                        Path = prefabPath,
                        Prefab = uiPrefab
                    };
                    uiPrefabDic.Add(uiName, pData);
                }
            }
            bool hasPrefab = uiPrefabDic.TryGetValue(uiName, out UIPrefabData data);
            if (hasPrefab)
            {
                groupMap[layer].OpenUIWnd(uiName, data.Prefab);
                groupMap[layer].RefreshDepth();
            }
            else
                Debug.LogErrorFormat("加载UI失败, ui name = {0}", uiName);
        }

        public void CloseWindow(string uiName, bool isDestroy = true)
        {
            var e = groupMap.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.CloseUIWnd(uiName, isDestroy))
                {
                    if (isDestroy)
                        e.Current.Value.RefreshDepth();
                    break;
                }
            }
            if (isDestroy)
            {
                if (uiPrefabDic.ContainsKey(uiName))
                {
                    var pData = uiPrefabDic[uiName];
                    uiPrefabDic.Remove(uiName);

#if UNITY_EDITOR
                    if (!AssetBundleManager.SimulateAssetBundleInEditor)
                            AssetBundleManager.Release(pData.Path);
#else
                    AssetBundleManager.Release(pData.Path);
#endif


                }
            }
        }

        public UIGroup GetGroup(EWindowLayer layer)
        {
            if (groupMap.ContainsKey(layer))
                return groupMap[layer];
            return null;
        }

        public UIWnd GetUIWnd(string uiName)
        {
            var e = groupMap.GetEnumerator();
            while (e.MoveNext())
            {
                var ui = e.Current.Value.GetUIWnd(uiName);
                if (ui != null)
                    return ui;
            }
            return null;
        }

        public UIWnd GetUIWnd(string uiName, EWindowLayer layer)
        {
            if (groupMap.ContainsKey(layer))
            {
                return groupMap[layer].GetUIWnd(uiName);
            }
            return null;
        }
    }
}