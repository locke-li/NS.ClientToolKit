/**************************************************************
 *  类名称：          AppBuildPipelineCommand
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/9/8 13:56:56
 *  最后修改人：
 *  版权所有 （C）:   aligames
 **************************************************************/

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace CenturyGame.AppBuilder.Editor.Commands
{
    public static class AppBuildPipelineCommand
    {
        [MenuItem("CenturyGame/AppBuilder/Commands/创建编译管线配置", priority= 1)]
        public static void CopyGraphPipelineFile()
        {
            string path = Path.GetFullPath("Packages/com.centurygame.clienttoolkit/AppBuilder" +
                "/Editor/Builds/BuildPipelineConfGenerator/Graph.asset");

            if (!File.Exists(path))
            {
                Debug.LogError("The Graph.asset that we want to copy is not exist in the current package , try to reinstall package!");
                return;
            }

            string targetFolderPath = $"{Application.dataPath}/CenturyGamePackageRes/AppBuilder/Editor"; ;
            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);

                AssetDatabase.Refresh();
            }

            var targetPath = $"{targetFolderPath}/AppBuildPipelineGraph.asset";
            bool writeconfig = true;
            if (File.Exists(targetPath)) 
            {
                writeconfig = EditorUtility.DisplayDialog("管线配置", "是否需要覆盖当前编译管线配置？", "覆盖", "取消");
            }

            if (writeconfig)
            {
                try
                {
                    File.Copy(path, targetPath);
                    Debug.Log($"Copy Graph.asset file success , xml path : {targetPath} .");
                    AssetDatabase.Refresh();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Debug.Log("用户取消创建管线配置。");
            }            
        }

    }
}