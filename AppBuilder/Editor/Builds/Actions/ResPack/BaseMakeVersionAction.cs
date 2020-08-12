/***************************************************************

 *  类名称：        BaseMakeVersionAction

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/8 10:38:53

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using CenturyGame.AppBuilder.Editor.Builds.BuildInfos;
using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using CenturyGame.AppUpdaterLib.Runtime.Manifests;
using CenturyGame.Core.Pipeline;
using UnityEngine;
#if DEBUG_FILE_CRYPTION
using File = CenturyGame.Core.IO.File;
#else
using File = System.IO.File;
#endif

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    public abstract class BaseMakeVersionAction : BaseBuildFilterAction
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

        protected LastBuildInfo GetLastBuildInfo(IFilter filter, IPipelineInput input)
        {
            var appBuildContext = AppBuildContext;
            return appBuildContext.GetLastBuildInfo();
        }

        protected string CreateCurVersionFolder(string packagePath, string currentVer)
        {
            string sourceFolderPath = string.Concat(packagePath
                , Path.DirectorySeparatorChar,
                currentVer, Path.DirectorySeparatorChar, AbHelp.GetPlatformNameNoSlash());
            sourceFolderPath = EditorUtils.OptimazePath(sourceFolderPath);
            DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(sourceFolderPath);
            if (sourceDirectoryInfo.Exists)
                sourceDirectoryInfo.Delete(true);
            sourceDirectoryInfo.Create();

            return sourceFolderPath;
        }

        protected string CreateFilesFolder(string parentPath, string currentVer)
        {
            string filesSourcePath = string.Concat(parentPath, Path.DirectorySeparatorChar,
                currentVer);
            DirectoryInfo filesSourceDirectoryInfo = new DirectoryInfo(filesSourcePath);
            if (filesSourceDirectoryInfo.Exists)
                filesSourceDirectoryInfo.Delete(true);
            filesSourceDirectoryInfo.Create();
            return filesSourcePath;
        }


        /// <summary>
        /// 生成远端unity资源版本文件
        /// </summary>
        /// <param name="targetFolder"></param>
        protected void CraeteRemoteUnityResVersionManifestFile(IFilter filter, IPipelineInput input, string targetFolder)
        {
            var appBuildContext = AppBuildContext;
            string upLoadVersionManifestPath = appBuildContext.GetWillUploadedUnityResUpdateManifestPath(
                appBuildContext.AppInfoManifest.unityDataResVersion, targetFolder);
            string directory = System.IO.Path.GetDirectoryName(upLoadVersionManifestPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var versinManifestJson = appBuildContext.VersionManifestToJson(appBuildContext.VersionManifest);
            File.WriteAllText(upLoadVersionManifestPath, versinManifestJson, appBuildContext.TextEncoding);
            Debug.Log($"Save file \"{upLoadVersionManifestPath}\" completed");
        }

        #endregion


    }
}
