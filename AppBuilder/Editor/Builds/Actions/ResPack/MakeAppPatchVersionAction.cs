/***************************************************************

 *  类名称：        MakeAppPatchVersionAction

 *  描述：		    制作当前App的Patch资源版本

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/6 13:46:52

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using CenturyGame.AppBuilder.Runtime.Exceptions;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.Editor.UploadUtilitis;
using UnityEditor;
using UnityEngine;
using Version = CenturyGame.AppUpdaterLib.Runtime.Version;
using CenturyGame.Core.Pipeline;
using CenturyGame.AppBuilder.Editor.Builds.Exceptions;
#if DEBUG_FILE_CRYPTION
using File = CenturyGame.Core.IO.File;
#else
using File = System.IO.File;
#endif

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    public class MakeAppPatchVersionAction : BaseMakeVersionAction
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
            var lastVersionInfo = this.GetLastBuildInfo(filter, input);
            if (lastVersionInfo != null)
            {
                var appBuildContext = AppBuildContext;
                var appVersion = AppBuildConfig.GetAppBuildConfigInst().targetAppVersion;
                string versionStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
                Version baseVersion = new Version(versionStr);

                var lastVersion = new Version(lastVersionInfo.versionInfo.version);

                var result = baseVersion.CompareTo(lastVersion);

                if (AppBuildConfig.GetAppBuildConfigInst().incrementRevisionNumberForPatchBuild)
                {
                    if (result > Version.VersionCompareResult.Equal)//制作Patch时，基础版本不能大于上次制作的Patch版本号
                    {
                        appBuildContext.ErrorSb.AppendLine($"Large VersionCompareResult.Equal , curVersion : {versionStr.ToString()} , lastVersion : {lastVersion} .");
                        return false;
                    }

                    if (result < Version.VersionCompareResult.LowerForPatch)//制作Patch时，基础版本只能在Patch级别小于上次制作的版本号
                    {
                        appBuildContext.ErrorSb.AppendLine($"Large VersionCompareResult.HigherForPatch , curVersion : {versionStr.ToString()} , lastVersion : {lastVersion} .");
                        return false;
                    }
                }
            }
            else
            {
                throw new LastBuildVersionFileNotFoundException("Last build version file is not found！");
            }

            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            var appBuildContext = AppBuildContext;
            appBuildContext.makeVersionMode = Contexts.AppBuildContext.MakeVersionMode.MakePatchVersion;
            mergerFile(filter,input);
            this.State = ActionState.Completed;
        }

        private void mergerFile(IFilter filter, IPipelineInput input)
        {
            var appBuildContext = AppBuildContext;
            string unitResManifestPath = appBuildContext.LastBuildUnityResManifestPath;

            //Check last res verison manifest file is or not exist !
            if(!File.Exists(unitResManifestPath))
                throw new LastBuildVersionFileNotFoundException($"The file that name is \"{unitResManifestPath}\" is not found!");
            FileInfo targetFile = new FileInfo(unitResManifestPath);
            LastBuildVersion target = JsonUtility.FromJson<LastBuildVersion>(File.ReadAllText(targetFile.FullName, appBuildContext.TextEncoding));

            //Check verison 后续用于发指定版本的Patch时的检查
            //var curVesion = new Version(appBuildContext.AppInfoManifest.version);
            //var lastVersion = new Version(target.Version);
            //var compareResult = curVesion.CompareTo(lastVersion);
            //if (compareResult < Version.VersionCompareResult.Equal &&
            //    compareResult > Version.VersionCompareResult.HigherForPatch)
            //{
            //    throw new MakeAppPatchException(appBuildContext.AppInfoManifest.version,
            //        target.Version);
            //}


            //为的是维护一个基础版本系列的文件集合
            var diffList = CalculateUnityResDifference(appBuildContext.VersionManifest,target);

            makePatch(filter,input,diffList, target);
        }


        private List<FileDesc> CalculateUnityResDifference(VersionManifest versionManifest ,
            LastBuildVersion basicFileListInfo)
        {
            Dictionary<string, FileDesc> targetList = new Dictionary<string, FileDesc>();
            List<FileDesc> diffFileList = new List<FileDesc>();
            for (int i = 0; i < basicFileListInfo.Info.Datas.Count; i++)
            { 
                var info = basicFileListInfo.Info.Datas[i];
                targetList[info.N] = info;
            }
            for (int i = 0; i < versionManifest.Datas.Count; i++)
            {
                var info = versionManifest.Datas[i];
                if (targetList.TryGetValue(info.N,out var tempFileDesc))
                {
                    if (tempFileDesc.H != info.H)
                    {
                        diffFileList.Add(info);
                    }
                }
                else
                {
                    targetList[info.N] = info;
                    diffFileList.Add(info);
                }
            }

            return diffFileList;
        }

        private void makePatch(IFilter filter, IPipelineInput input,List<FileDesc> patchList, LastBuildVersion target)
        {
            var appBuildContext = AppBuildContext;

            //保存版本文件清单
            string localUnityResUpdateManifestPath = appBuildContext.GetLocalUnityResUpdateManifestPath();
            var versinManifestJson = appBuildContext.VersionManifestToJson(appBuildContext.VersionManifest);
            var versinManifestBytes = appBuildContext.TextEncoding.GetBytes(versinManifestJson);
            File.WriteAllText(localUnityResUpdateManifestPath, versinManifestJson, appBuildContext.TextEncoding);
            Logger.Info($"Save file \"{localUnityResUpdateManifestPath}\" completed");

            var curVersion = GetBuildVersion(filter,input,patchList);
            string version = curVersion.GetVersionString();
            Logger.Info($"Created a new verison number that value is \"{version}\" !");
            appBuildContext.AppInfoManifest.version = version;

            //保存AppInfo文件
            appBuildContext.AppInfoManifest.unityDataResVersion = EditorUtils.GetMD5(versinManifestBytes);
            var builtinAppInfoFilePath = appBuildContext.GetBuiltinAppInfoFilePath();
            var appInfoJson = appBuildContext.ToJson(appBuildContext.AppInfoManifest);
            File.WriteAllText(builtinAppInfoFilePath, appInfoJson, appBuildContext.TextEncoding);
            Logger.Info($"Save file \"{builtinAppInfoFilePath}\" completed");

            //获取包路径
            string packagePath = appBuildContext.GetPackageFolderPath();
            string sourceFolderPath = CreateCurVersionFolder(packagePath, curVersion.GetVersionFolderString());
            CraeteRemoteUnityResVersionManifestFile(filter,input,sourceFolderPath);

            //生成Path文件
            CreatePatchFiles(filter,input, sourceFolderPath, patchList);

            appBuildContext.UploadFileFolder = sourceFolderPath;

            AssetDatabase.Refresh();
        }


        private void CreatePatchFiles(IFilter filter, IPipelineInput input, string sourceFolderPath , List<FileDesc> patchList)
        {
            var appBuildContext = AppBuildContext;
            int totalSize = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < patchList.Count; i++)
            {
                var info = patchList[i];
                sb.AppendLine(info.N + ":" + AbHelp.GetSizeStr(info.S));
                totalSize += info.S;

                string sourcePath = string.Concat(System.Environment.CurrentDirectory
                    , Path.DirectorySeparatorChar
                    , appBuildContext.StreamingAssetsFolder
                    , Path.DirectorySeparatorChar
                    , info.N);

                string fileName = $"{info.N}#{info.H}#{info.S}";
                string targetPath = string.Concat(sourceFolderPath
                    , Path.DirectorySeparatorChar
                    , AssetsFileSystem.AppUnityResRemoteRoot, Path.DirectorySeparatorChar, fileName);

                var directoryName = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                //将差异文件拷贝到Package目录下，以便上传
                Logger.Info($"Copy sourcePath : {sourcePath} targetPath : {targetPath}");
                File.Copy(sourcePath, targetPath, true);
            }

            Logger.Info($"The patch has {patchList.Count} file!");
            sb.AppendLine("Total size:" + AbHelp.GetSizeStr(totalSize));
            Logger.Info("Patch info : \r\n" + sb.ToString());
        }

        private Version GetBuildVersion(IFilter filter, IPipelineInput input, List<FileDesc> patchList)
        {
            var appVersion = AppBuildConfig.GetAppBuildConfigInst().targetAppVersion;
            string versionStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
            Version curVersion = new Version(versionStr);
            var lastVersionInfo = this.GetLastBuildInfo(filter, input);
            if (lastVersionInfo != null)
            {
                var lastVersion = new Version(lastVersionInfo.versionInfo.version);

                if (AppBuildConfig.GetAppBuildConfigInst().incrementRevisionNumberForPatchBuild)
                {
                    lastVersion.IncrementOneForPatch();
                    curVersion = lastVersion.Clone();
                }
            }
            else
            {
                throw  new LastBuildVersionFileNotFoundException(versionStr); 
            }
            return curVersion;
        }

        /*
        private Version GetBuildVersion1(List<FileDesc> patchList)
        {
            var appVersion = AppBuildConfig.GetAppBuildConfigInst().targetAppVersion;
            var lastBuildInfo = GetLastBuildInfo();
            Version lastVersion = new Version(lastBuildInfo.version);

            var context = Context.GetAppBuildContext();
            //context.lastBuildVersionString = lastBuildInfo.version;


            var targetVersinStr = appVersion.GetBaseVersion();
            Version targetVersion = new Version(targetVersinStr);

            var result = targetVersion.CompareTo(lastVersion);

            switch (result)
            {
                case Version.VersionCompareResult.HigherForMajor:
                    targetVersinStr = $"{appVersion.Major}.0.0";
                    Debug.Log("Generate a new major version !");
                    break;
                case Version.VersionCompareResult.HigherForMinor:
                    targetVersinStr = $"{appVersion.Major}.{appVersion.Minor}.0";
                    Debug.Log("Generate a new minor version !");
                    break;
                case Version.VersionCompareResult.HigherForPatch:
                    targetVersinStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
                    Debug.Log("Generate a new patch version !");
                    break;
                case Version.VersionCompareResult.LowerForPatch:
                    targetVersinStr = $"{appVersion.Major}.{appVersion.Minor}.{lastVersion.PatchNum}";
                    Debug.Log("Increment the old patch number!");
                    break;
                case Version.VersionCompareResult.Equal:
                    Debug.Log($"Version do not change , use old ！Version : \"{targetVersinStr}\" !");
                    break;
                default:
                    throw new Exception($"Generate version failure ! result : {result} !");
            }

            Version curVersion = new Version(targetVersinStr);
            if (result == Version.VersionCompareResult.LowerForPatch && patchList.Count > 0)
            {
                curVersion.IncrementOneForPatch();
            }
            else if (result == Version.VersionCompareResult.Equal && patchList.Count > 0)
            {
                curVersion.IncrementOneForPatch();
            }
            return curVersion;
        }
        */
        #endregion
    }
}
