/**************************************************************
 *  类名称：          ConstructStreamingAssetsAction
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/11/27 10:36:49
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using CenturyGame.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    class ConstructStreamingAssetsAction : BaseBuildFilterAction
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        #endregion


        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        #endregion


        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion


        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------


        private string GetUploadDir()
        {
            return AppBuildContext.GetResStoragePath();
        }

        public override bool Test(IFilter filter, IPipelineInput input)
        {


            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            CopyAssetsToStreamingAssetsPath();

            this.State = ActionState.Completed;
        }


        private void CopyAssetsToStreamingAssetsPath()
        {
            CopyGameResources();

            CopyUnityResFileList();

            CopyDataResFileList();

            Logger.Info("Copy game resources to streaming assets path !");
        }


        private void CopyGameResources()
        {
            var streamingPath = AppBuildContext.GetAssetsOutputPath();
            if (Directory.Exists(streamingPath))
            {
                Directory.Delete(streamingPath, true);
            }
            AssetDatabase.Refresh();
            Directory.CreateDirectory(streamingPath);

            var resStorage = AppBuildContext.GetResStoragePath();
            DirectoryInfo dirInfo = new DirectoryInfo(resStorage);
            FileInfo[] fileInfos = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            Logger.Info("Start copy game resources to streaming assets path !");
            foreach (var fileInfo in fileInfos)
            {
                string sourcePath = EditorUtils.OptimazePath(fileInfo.FullName);
                if (sourcePath.Contains(AppBuildContext.GenCodePattern))
                {
                    continue;
                }

                if (sourcePath.Contains("protokitgo.yaml"))
                {
                    continue;
                }

                if (sourcePath.Contains("resource_versions.release"))
                {
                    continue;
                }

                string targePath = $"{streamingPath}{sourcePath.Replace(resStorage, "")}";

                string dirName = Path.GetDirectoryName(targePath);

                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                File.Copy(sourcePath, targePath, true);
            }
            Logger.Info("Copy game resources  completed!");
        }

        private void CopyUnityResFileList()
        {
            var streamingPath = AppBuildContext.GetAssetsOutputPath();
            
            var platformName = string.Empty;
#if UNITY_EDITOR && UNITY_ANDROID
            platformName = "android";
#elif UNITY_EDITOR && UNITY_IPHONE
            platformName = "ios";
#else
            throw new InvalidOperationException($"Unsupport build platform : {EditorUserBuildSettings.activeBuildTarget} .");
#endif

            var configRepoPath = AppBuildConfig.GetAppBuildConfigInst().GameTableDataConfigPath;

            var resListPath = $"{configRepoPath}/gen/rawdata/version_list/res_{platformName}.json";

            if (!File.Exists(resListPath))
            {
                throw new FileNotFoundException($"The file path is \"{resListPath}\" .");
            }

            Logger.Info("Start copy resource list file to streaming assets path !");
            //var contents = File.ReadAllText(resListPath,AppBuildContext.TextEncoding);
            //var removeStr = $"{AppBuildConfig.GetAppBuildConfigInst().upLoadInfo.remoteDir}/resource/";
            //contents = contents.Replace(removeStr, "");
            //File.WriteAllText($"{streamingPath}/res_{platformName}.json", 
            //    contents,AppBuildContext.TextEncoding);
            File.Copy(resListPath, $"{streamingPath}/res_{platformName}.json");
            Logger.Info("copy resource list file completed!");
            AssetDatabase.Refresh();
        }

        private void CopyDataResFileList()
        {
            var streamingPath = AppBuildContext.GetAssetsOutputPath();
            var configRepoPath = AppBuildConfig.GetAppBuildConfigInst().GameTableDataConfigPath;

            var dataResListPath = $"{configRepoPath}/gen/rawdata/version_list/res_data.json";

            if (!File.Exists(dataResListPath))
            {
                throw new FileNotFoundException($"The file path is \"{dataResListPath}\" .");
            }

            Logger.Info("Start copy data resource list file to streaming assets path !");
            File.Copy(dataResListPath, $"{streamingPath}/res_data.json");
            Logger.Info("copy data resource list file completed!");
            AssetDatabase.Refresh();
           
        }

        #endregion
    }
}