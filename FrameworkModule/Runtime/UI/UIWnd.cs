using System;
using UnityEngine;
using UnityEngine.Events;
using CenturyGame.LuaModule.Runtime;
using CenturyGame.LuaModule.Runtime.Interfaces;

namespace CenturyGame.Framework.UI
{
    public class UIWnd : MonoBehaviour
    {
        private ILuaTable logicEnv;
        private readonly static string BindMethod = "BindView";
        public readonly static int GroupDepthFactor = 1000;
        public readonly static int DepthFactor = 10;
        private Canvas canvas;

        public string UIName { get; private set; }

        internal int Depth
        {
            get { return canvas.sortingOrder; }
            set { canvas.sortingOrder = value; }
        }

        internal int DepthInGroup { get; set; }

        [HideInInspector]
        public UnityEvent OnInit = new UnityEvent();

        [HideInInspector]
        public UnityEvent OnEnter = new UnityEvent();

        [HideInInspector]
        public UnityEvent OnExit = new UnityEvent();

        [HideInInspector]
        public UnityEvent OnClose = new UnityEvent();

        [HideInInspector]
        public UnityEvent OnUpdate = new UnityEvent();

        private void Awake()
        {
            UIName = gameObject.name;
            canvas = GetComponent<Canvas>();
            //根据命名约束判断是否是CS逻辑界面，CS逻辑界面不绑定Lua脚本
            if (!UIName.EndsWith("_cs"))
            {
                BindLuaScript();
            }
            OnInit?.Invoke();
        }

        private void BindLuaScript()
        {
            ILuaEnvironment luaEnv = LuaAccessManager.Environment;
            string luaFileName = string.Concat(UIName + "logic");
            string cmd = string.Concat("require 'ui/", luaFileName, "'");
            luaEnv.DoString(cmd, luaFileName);
            logicEnv = LuaAccessManager.GetGlobalTable(UIName);
            var bindFunction = logicEnv.GetFunction(BindMethod);
            bindFunction?.InstCall(this);
        }

        private void OnEnable()
        {
            OnEnter?.Invoke();
        }

        private void OnDisable()
        {
            OnExit?.Invoke();
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            OnClose?.Invoke();
        }

        internal void RefreshDepth(int groupDepth, int depthInGroup)
        {
            DepthInGroup = depthInGroup;
            int oldDepth = Depth;
            int deltaDepth = GroupDepthFactor * groupDepth + DepthFactor * DepthInGroup - oldDepth;
            Depth += deltaDepth;
            //Canvas[] canvas = GetComponentsInChildren<Canvas>(true);
            //for (int i = 0; i < canvas.Length; i++)
            //{
            //    canvas[i].sortingOrder += deltaDepth;
            //}
        }
    }
}