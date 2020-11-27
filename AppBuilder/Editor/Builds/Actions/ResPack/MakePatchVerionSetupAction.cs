/**************************************************************
 *  类名称：          MakePitchVerionSetupAction
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/11/27 15:00:03
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenturyGame.Core.Pipeline;
using Version = CenturyGame.AppUpdaterLib.Runtime.Version;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
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
            Logger.Info($"The target app version from build config is \"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}\" .");
            if (appVersion.Major == "0" && appVersion.Minor == "0" && appVersion.Patch == "0")
            {
                Logger.Error($"Invalid app base vesion : 0.0.0 .");
                return false;
            }

            var versionStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
            var targetVersion = new Version(versionStr);

            var lastVersionInfo = AppBuildContext.GetLastBuildInfo();
            if (lastVersionInfo != null)
            {
                var lastVersion = new Version(lastVersionInfo.versionInfo.version);
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


        public override bool Test(IFilter filter, IPipelineInput input)
        {
            if (!CheckAppVersionValid())
            {
                var appVersion = AppBuildConfig.GetAppBuildConfigInst().targetAppVersion;
                var versionStr = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}";
                AppBuildContext.AppendErrorLog($"Invalid target version : {versionStr}.");

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