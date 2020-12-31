/**************************************************************
 *  类名称：          MakePitchVerionSetupAction
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/11/27 15:00:03
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenturyGame.Core.Pipeline;
using Version = CenturyGame.AppUpdaterLib.Runtime.Version;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.AppPrepare
{
    class MakePatchVerionSetupAction : BaseBuildFilterAction
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

        private bool CheckAppVersionValid()
        {
            var appVersion = AppBuildConfig.GetAppBuildConfigInst().targetAppVersion;
            var versionStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
            var targetVersion = new Version(versionStr);
            if (!AppBuildConfig.GetAppBuildConfigInst().incrementRevisionNumberForPatchBuild && (targetVersion.PatchNum== 0))
            {
                Logger.Error($"In patch build mode and not be auto increment revision number , the patch value can not be zero , target version is \"{targetVersion.GetVersionString()}\".");
                return false;
            }

            var lastVersionInfo = AppBuildContext.GetLastBuildInfo();
            if (lastVersionInfo?.GetCurrentBuildInfo() != null)
            {
                var buildInfo = lastVersionInfo.GetCurrentBuildInfo();
                var lastVersion = new Version(buildInfo.versionInfo.version);
                Logger.Info($"The last app version :  {lastVersion.GetVersionString()} .");
                var result = targetVersion.CompareTo(lastVersion);

                //首先，目标版本必须和上一次build的版本处在同一个大版本上
                if (result > Version.VersionCompareResult.LowerForMinor && result < Version.VersionCompareResult.HigherForMinor)
                {
                    //如果不是自增Patch的build，那么目标版本的Patch必须大于上一次build的Patch
                    if (!AppBuildConfig.GetAppBuildConfigInst().incrementRevisionNumberForPatchBuild &&
                        result < Version.VersionCompareResult.HigherForPatch)
                    {
                        Logger.Error($"Version is invalid , targetVersion : {targetVersion.GetVersionString()} " +
                                     $"lastBuildVersion : {lastVersion.GetVersionString()} .");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Logger.Error($"No last build info , You can't make app patch build !");
                return false;
            }
            
            return true;
        }

        private bool CheckGameConfigs()
        {
            var configsPath = AppBuildConfig.GetAppBuildConfigInst().upLoadInfo.dataResAbsolutePath;
            if (!Directory.Exists(configsPath))
            {
                Logger.Error($"The table config git repo that path is \"{configsPath}\" is not exist !" +
                             $" Pleause specify a valid path!");
                return false;
            }

            return true;
        }

        public override bool Test(IFilter filter, IPipelineInput input)
        {
            if (!CheckAppVersionValid())
            {
                var appVersion = AppBuildConfig.GetAppBuildConfigInst().targetAppVersion;
                var versionStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
                AppBuildContext.AppendErrorLog($"Invalid target version : {versionStr}.");

                return false;
            }

            if (!CheckGameConfigs())
            {
                return false;
            }

            return true;
        }


        public override void Execute(IFilter filter, IPipelineInput input)
        {
            this.Setup(filter, input);
            this.State = ActionState.Completed;
        }

        private void Setup(IFilter filter, IPipelineInput input)
        {
            input.SetData(EnvironmentVariables.MAKE_BASE_APP_VERSION_KEY, false);
        }


        #endregion

    }
}