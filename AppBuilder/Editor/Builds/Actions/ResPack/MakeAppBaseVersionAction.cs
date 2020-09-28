/***************************************************************

 *  类名称：        MakeBaseVersionAction

 *  描述：			制作当前App的基础资源版本

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/6 13:34:15

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using CenturyGame.AppBuilder.Runtime.Exceptions;
using UnityEditor;
using UnityEngine;
using Version = CenturyGame.AppUpdaterLib.Runtime.Version;
using CenturyGame.Core.Pipeline;
#if DEBUG_FILE_CRYPTION
using File = CenturyGame.Core.IO.File;
#else
using File = System.IO.File;
#endif

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    public class MakeAppBaseVersionAction : BaseMakeVersionAction
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
                var appVersion = AppBuildConfig.GetAppBuildConfigInst().targetAppVersion;
                string versionStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
                Version baseVersion = new Version(versionStr);

                var lastVersion = new Version(lastVersionInfo.versionInfo.version);

                var result = baseVersion.CompareTo(lastVersion);

                if (result < Version.VersionCompareResult.HigherForMinor)//制作的基础版本必须比上一次的版本在Minor级别更高
                {
                    throw new BuildAppVersionException(versionStr, lastVersionInfo.versionInfo.version);
                }
            }

            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            var appBuildContext = AppBuildContext;
            appBuildContext.makeVersionMode = Contexts.AppBuildContext.MakeVersionMode.MakeBaseVersion;

            GenerateCurVersionDataAndUpLoad(filter, input); 
            this.State = ActionState.Completed;
        }

        private bool GenerateCurVersionDataAndUpLoad(IFilter filter, IPipelineInput input)
        {
            var curVersion = GetBuildVersion(filter,input);

            var appBuildContext = AppBuildContext;
            appBuildContext.AppInfoManifest.version = curVersion.GetVersionString();

            //保存版本文件清单
            string localUnityResUpdateManifestPath = appBuildContext.GetLocalUnityResUpdateManifestPath();
            var versinManifestJson = appBuildContext.VersionManifestToJson(appBuildContext.VersionManifest);
            var versinManifestBytes = appBuildContext.TextEncoding.GetBytes(versinManifestJson);
            File.WriteAllText(localUnityResUpdateManifestPath, versinManifestJson, appBuildContext.TextEncoding);
            Logger.Info($"Save file \"{localUnityResUpdateManifestPath}\" completed");

            //保存AppInfo文件
            appBuildContext.AppInfoManifest.unityDataResVersion = EditorUtils.GetMD5(versinManifestBytes);
            var builtinAppInfoFilePath = appBuildContext.GetBuiltinAppInfoFilePath();
            var appInfoJson = appBuildContext.ToJson(appBuildContext.AppInfoManifest);
            File.WriteAllText(builtinAppInfoFilePath, appInfoJson,appBuildContext.TextEncoding);
            Logger.Info($"Save file \"{builtinAppInfoFilePath}\" completed");

            //获取包路径
            string packagePath = appBuildContext.GetPackageFolderPath();
            string sourceFolderPath = CreateCurVersionFolder(packagePath, curVersion.GetVersionFolderString());

            CraeteRemoteUnityResVersionManifestFile(filter,input,sourceFolderPath);

            appBuildContext.UploadFileFolder = sourceFolderPath;

            AssetDatabase.Refresh();
            return true;
        }

        private Version GetBuildVersion(IFilter filter, IPipelineInput input)
        {
            var appVersion = AppBuildConfig.GetAppBuildConfigInst().targetAppVersion;
            string versionStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
            Version curVersion = new Version(versionStr);
            //var lastVersionInfo = this.GetLastBuildInfo(filter,input);
            //if (lastVersionInfo != null)
            //{
            //    var lastVersion = new Version(lastVersionInfo.versionInfo.version);

            //    var result = curVersion.CompareTo(lastVersion);

            //    if (result < Version.VersionCompareResult.Equal)
            //        //如果发现将要制作的基础版本的版本号小于上次制作的版本号，则认为编译此版本的目标版本号是不合法的
            //    {
            //        throw new BuildAppVersionException(versionStr,lastVersionInfo.versionInfo.version);
            //    }
            //}

            return curVersion;
        }
        #endregion
    }
}
