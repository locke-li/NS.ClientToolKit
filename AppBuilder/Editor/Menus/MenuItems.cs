/**************************************************************
 *  类名称：          MenuItems
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/11/27 20:43:41
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using System.IO;
using System.Text;
using CenturyGame.AppBuilder.Editor.Builds;
using UnityEditor;
using UnityEngine;

namespace CenturyGame.ClientToolKit.AppBuilder.Editor.Menus
{
    public static class MenuItems
    {


        [MenuItem("CenturyGame/AppBuilder/信息提取/显示上一次构建信息")]
        static void ShowLastBuildInfo()
        {
            string path = AppBuildConfig.GetAppBuildConfigInst().LastBuildInfoPath;

            path = EditorUtils.OptimazePath(path);
            if (!File.Exists(path))
            {
                Debug.LogError("此前从未执行过构建或构建信息已被清除！");

                return;
            }

            string jsonContents = File.ReadAllText(path, new UTF8Encoding(false, true));
            
            Debug.Log($"The Last build info : \n{jsonContents}");
        }

    }
}