using System;
using System.Collections.Generic;
using UnityEngine;

namespace CenturyGame.Framework.UI
{
    public class UIGroup
    {
        public EWindowLayer Layer { get; private set; }

        internal Transform Root { get; private set; }

        public int GroupDepth { get; private set; }

        private List<UIWnd> uiList = new List<UIWnd>();

        public int UIWndCount
        {
            get
            {
                return uiList.Count;
            }
        }

        public UIGroup(EWindowLayer layer, Transform root, int groupDepth)
        {
            Layer = layer;
            Root = root;
            GroupDepth = groupDepth;
        }

        internal void OpenUIWnd(string uiName, UnityEngine.Object uiPrefab)
        {
            UIWnd uiWnd = GetUIWnd(uiName);
            if (uiWnd != null)
            {
                uiList.Remove(uiWnd);
                uiList.Add(uiWnd);
                if (!uiWnd.gameObject.activeSelf)
                    uiWnd.gameObject.SetActive(true);
            }
            else
            {
                GameObject obj = UnityEngine.Object.Instantiate(uiPrefab) as GameObject;
                obj.name = obj.name.Substring(0, obj.name.Length - 7);
                obj.transform.SetParent(Root);
                UIWnd newUI = obj.AddComponent<UIWnd>();
                uiList.Add(newUI);
            }
        }

        internal UIWnd GetUIWnd(string uiName)
        {
            for (int i = 0; i < uiList.Count; i++)
            {
                if (uiList[i].UIName.Equals(uiName))
                {
                    return uiList[i];
                }
            }
            return null;
        }

        internal bool CloseUIWnd(string uiName, bool isDestroy)
        {
            int index = -1;
            for (int i = 0; i < uiList.Count; i++)
            {
                if (uiList[i].UIName.Equals(uiName))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                if (isDestroy)
                {
                    UnityEngine.Object.Destroy(uiList[index].gameObject);
                    uiList.RemoveAt(index);
                }
                else
                {
                    uiList[index].gameObject.SetActive(false);
                }
                return true;
            }
            return false;
        }

        internal void RefreshDepth()
        {
            for (int i = 0; i < uiList.Count; i++)
            {
                uiList[i].RefreshDepth(GroupDepth, i);
            }
        }
    }
}