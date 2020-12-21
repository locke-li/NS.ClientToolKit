/***************************************************************

 *  类名称：        LastBuildInfo

 *  描述：			记录上次编译的相关信息

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/7 10:40:58

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using CenturyGame.AppBuilder.Editor.Builds.Factories;
using CenturyGame.AppUpdaterLib.Runtime.Manifests;
using UnityEditor;

namespace CenturyGame.AppBuilder.Editor.Builds.BuildInfos
{
    [Serializable]
    public class BuildInfo
    {
        /// <summary>
        /// 目标平台
        /// </summary>
        public string TargetPlatform;

        public AppInfoManifest baseVersionInfo = SimpleFactory.CreateAppInfoManifest();

        public AppInfoManifest versionInfo = SimpleFactory.CreateAppInfoManifest();

        public BuildInfo(string targetPlatformStr)
        {
            this.TargetPlatform = targetPlatformStr;
        }
    }

    [Serializable]
    public class LastBuildInfo
    {
        public List<BuildInfo> buildInfos = new List<BuildInfo>();

        public BuildInfo GetBuildInfo(BuildTarget target)
        {
            return buildInfos.FirstOrDefault(x => { return x.TargetPlatform == target.ToString(); });
        }

        public BuildInfo GetCurrentBuildInfo()
        {
            return GetBuildInfo(EditorUserBuildSettings.activeBuildTarget);
        }

        public void AddAppInfo(AppInfoManifest baseVersionInfo, AppInfoManifest versionInfo)
        {
            var buildInfo = GetCurrentBuildInfo();
            if (buildInfo == null)
            {
                buildInfo = new BuildInfo(EditorUserBuildSettings.activeBuildTarget.ToString());
                buildInfos.Add(buildInfo);
            }

            if (baseVersionInfo != null)
            {
                buildInfo.baseVersionInfo = baseVersionInfo;
            }

            buildInfo.versionInfo = versionInfo;
        }
    }

}
