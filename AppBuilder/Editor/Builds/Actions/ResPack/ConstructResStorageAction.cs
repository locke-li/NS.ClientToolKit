﻿/**************************************************************
 *  类名称：          ConstructResStorageAction
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/11/26 16:30:35
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.AssetBundleManager.Runtime;
using CenturyGame.Core.Pipeline;
using UnityEngine;
using UnityEditor;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    class ConstructResStorageAction : BaseBuildFilterAction
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


        public override bool Test(IFilter filter, IPipelineInput input)
        {
            var manifesetPath = GetAssetbundleManifestFilePath();
            if (!File.Exists(manifesetPath))
            {
                AppBuildContext.ErrorSb.AppendLine($"The assetbundle manifest that path is \"{manifesetPath}\" is not exist!");
                return false;
            }

            var luaprojectDir = input.GetData<string>("LuaProject", AppBuildContext.GetLuaProjectPath());

            if (string.IsNullOrEmpty(luaprojectDir))
            {
                AppBuildContext.ErrorSb.AppendLine($"The LuaProject not set!");
                return false;
            }

            if (!Directory.Exists(luaprojectDir))
            {
                AppBuildContext.ErrorSb.AppendLine($"The LuaProject that path is \"{luaprojectDir}\" is not exist!");
                return false;
            }

            return true;
        }


        public override void Execute(IFilter filter, IPipelineInput input)
        {
            Logger.Info($"Start construct the files that needed to upload !");

            this.ConstructUploadRes(filter,input);

            this.State = ActionState.Completed;
        }


        private string GetOutputAssetbundleFolder()
        {
            string result = string.Concat(System.Environment.CurrentDirectory,
                Path.DirectorySeparatorChar,
                AppBuildContext.Temporary,
                Path.DirectorySeparatorChar,
                AppBuildContext.AbExportFolder);
            return EditorUtils.OptimazePath(result);
        }

        private string GetAssetbundleManifestFilePath()
        {
            var abDir = GetOutputAssetbundleFolder();
            var manifestPath = string.Concat(abDir,
                Path.DirectorySeparatorChar,
                AppBuildContext.AbExportFolder);
            return EditorUtils.OptimazePath(manifestPath);
        }

        private void ConstructUploadRes(IFilter filter, IPipelineInput input)
        {
            var resStorage = CreateStorageDir();

            CopyAssetBundlesToUploadStorage(resStorage);

            CopyLuaLogicScriptToUploadStorage(filter, input,resStorage);
        }


        private string CreateStorageDir()
        {
            var resStorage = AppBuildContext.GetResStoragePath();
            if (Directory.Exists(resStorage))
            {
                Directory.Delete(resStorage, true);
            }

            Directory.CreateDirectory(resStorage);
            return resStorage;
        }

        private void CopyAssetBundlesToUploadStorage(string resStorage)
        {
            var abDir = GetOutputAssetbundleFolder();
            var manifestPath = GetAssetbundleManifestFilePath();
            Logger.Info($"The assetbundle manifest path : {manifestPath}!");
            byte[] bytes = File.ReadAllBytes(manifestPath);
            var mainAb = AssetBundle.LoadFromMemory(bytes);
            var mainAbManifest = mainAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            mainAb.Unload(false);

            var abNames = mainAbManifest.GetAllAssetBundles();

            
            List<ABTableItem> list = new List<ABTableItem>();
            foreach (var abName in abNames)
            {
                var srcAbPath = $"{abDir}/{abName}";
                var desPath = $"{resStorage}/{abName}";
                var desDirPath = Path.GetDirectoryName(desPath);
                if (!Directory.Exists(desDirPath))
                {
                    Directory.CreateDirectory(desDirPath);
                }
                File.Copy(srcAbPath, desPath, true);
                ABTableItem item = new ABTableItem();
                item.Name = abName.Replace(AppBuildContext.ResExt, "");
                List<string> dependencies = new List<string>();
                foreach (var dependencie in mainAbManifest.GetAllDependencies(item.Name))
                {
                    dependencies.Add(dependencie.Replace(AppBuildContext.ResExt,""));
                }

                item.Dependencies = dependencies.ToArray();
                list.Add(item);
            }

            Logger.Info("Start craete target custom assetbundle manifest .");
            string targetPath = $"{resStorage}/{AssetsFileSystem.UnityABFileName}";
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            ResManifest resManifest = new ResManifest();
            resManifest.Tables = list.ToArray();
            var json = AppBuildContext.ToJson(resManifest);
            File.WriteAllText(targetPath, json,AppBuildContext.TextEncoding);
            Logger.Info("Create target custom manifest completed!");

            AssetDatabase.Refresh();
            Logger.Debug("Copy assetbundles completed!");
        }

        private void CopyLuaLogicScriptToUploadStorage(IFilter filter, IPipelineInput input,string resStorage)
        {
            var luaprojectDir = input.GetData<string>("LuaProject",AppBuildContext.GetLuaProjectPath());
            luaprojectDir = EditorUtils.OptimazePath(luaprojectDir);

            DirectoryInfo dirInfo = new DirectoryInfo(luaprojectDir);
            FileInfo[] fileInfos = dirInfo.GetFiles("*.lua", SearchOption.AllDirectories);

            foreach (var fileInfo in fileInfos)
            {
                string sourcePath = EditorUtils.OptimazePath(fileInfo.FullName);
                if (sourcePath.Contains(AppBuildContext.GenCodePattern))
                {
                    continue;
                }

                string targePath = $"{resStorage}/lua{sourcePath.Replace(luaprojectDir, "")}";

                string dirName = Path.GetDirectoryName(targePath);

                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                File.Copy(sourcePath, targePath, true);
            }
        }


        #endregion

    }
}