/***************************************************************

 *  类名称：        AppBuildCommand

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/27 12:13:55

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System.IO;
using CenturyGame.AppBuilder.Editor.Builds;
using CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack;
using CenturyGame.Core.Pipeline;
using CenturyGame.AppBuilder.Editor.Builds.PipelineInputs;
using UnityEditor;
using UnityEngine;
using CenturyGame.AppBuilder.Editor.Builds.Filters.Concrete;
using CenturyGame.AppBuilder.Editor.Builds.Actions.ResProcess;
using CenturyGame.AppBuilder.Editor.Builds.InnerLoggers;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.LoggerModule.Runtime;

namespace CenturyGame.AppBuilder.Editor.Commands
{
    internal static class AppBuildCommand
    {
        [MenuItem("CenturyGame/AppBuilder/Commands/制作并上传Patch")]
        static void MakePatchResouces()
        {
            string configPath = AppBuildConfig.GetAppBuildConfigInst().AppBuildConfigFolder + "/MakePatchVersion.yaml";
            var result = RunPipeline(configPath);

            if (result.State == ProcessState.Error)
            {
                Debug.LogError($"Build app failure , error message : {result.Message}");
            }
            else
            {
                Debug.Log("Build app completed!");
            }
        }

        [MenuItem("CenturyGame/AppBuilder/Commands/制作一个基础版本")]
        static void MakeBaseVersionResources()
        {
            string configPath = AppBuildConfig.GetAppBuildConfigInst().AppBuildConfigFolder + "/MakeBaseVersion.yaml";

            var result = RunPipeline(configPath);

            if (result.State == ProcessState.Error)
            {
                Debug.LogError($"Build app failure , error message : {result.Message}");
            }
            else
            {
                Debug.Log("Build app completed!");
            }
        }

        [MenuItem("CenturyGame/AppBuilder/Commands/压缩Lua代码到StreamAssets文件夹")]
        static void ZipLuaFile()
        {
            string configPath = AppBuildConfig.GetAppBuildConfigInst().AppBuildConfigFolder + "/ZipLuaScript.yaml";

            var result = RunPipeline(configPath);

            if (result.State == ProcessState.Error)
            {
                Debug.LogError($"Build app failure , error message : {result.Message}");
            }
            else
            {
                Debug.Log("Build app completed!");
            }
        }

        static ProcessResult RunPipeline(string pipelineConfigPath)
        {
            if (string.IsNullOrEmpty(pipelineConfigPath))
            {
                return ProcessResult.Create(ProcessState.Error,$"The pipeline config that path is \"{pipelineConfigPath}\" is not found!");
            }

            LoggerManager.SetCurrentLoggerProvider(new AppBuilderLoggerProvider());

            var processor = AppBuilderPipelineProcessor.ReadFromBuildProcessConfig(pipelineConfigPath);

            AppBuildPipelineInput input = new AppBuildPipelineInput();

            return processor.Process(input);
        }


        //[MenuItem("Commands/clear data folder")]
        private static void clearStreamingAssets()
        {
            string folder = @"F:\SVNProjects\DianDianClient\TestUtf8";
            DirectoryInfo disk = new DirectoryInfo(folder);
            if (disk.Exists)
            {
                FileInfo[] files = disk.GetFiles("*.*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo info = files[i];
                    if (info.Extension == AbHelp.AbFileExt)
                    {
                        info.Delete();
                    }
                    else if (info.Extension == ".meta")
                    {
                        string sourceFile = info.FullName.Substring(0, info.FullName.Length - info.Extension.Length);
                        if (Path.GetExtension(sourceFile) == AbHelp.AbFileExt)
                        {
                            info.Delete();
                        }
                    }
                }
            }
        }


        [MenuItem("CenturyGame/AppBuilder/Commands/清理编译缓存")]
        static void ClearBuildCacheFolder()
        {
            string dir = AppBuildConfig.GetAppBuildConfigInst().GetBuildCacheFolderPath();
            dir = EditorUtils.OptimazePath(dir);

            if (Directory.Exists(dir))
                Directory.Delete(dir,true);

            Debug.Log("Clear build cache success !");
        }

    }
}
