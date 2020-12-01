/**************************************************************
 *  类名称：          MakeBaseVerionSetupAction
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/11/27 14:32:31
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using System.IO;
using CenturyGame.AppBuilder.Runtime.Exceptions;
using CenturyGame.Core.Pipeline;
using CenturyGame.AppBuilder.Editor;
using Version = CenturyGame.AppUpdaterLib.Runtime.Version;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    class MakeBaseVerionSetupAction : BaseBuildFilterAction
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
            Logger.Info($"The version from build config is \"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}\" .");
            if (appVersion.Major == "0" && appVersion.Minor == "0" && appVersion.Patch == "0")
            {
                Logger.Error($"Invalid app base vesion : 0.0.0 .");
                return false;
            }

            var versionStr = $"{appVersion.Major}.{appVersion.Minor}.0";

            var targetVersion = new Version(versionStr);

            var lastVersionInfo = AppBuildContext.GetLastBuildInfo();
            if (lastVersionInfo != null)
            {
                var lastVersion = new Version(lastVersionInfo.versionInfo.version);
                Logger.Info($"The last app version :  {lastVersion.GetVersionString()} .");

                var result = targetVersion.CompareTo(lastVersion);

                if (result < Version.VersionCompareResult.HigherForMinor)//次版本（Minor）本次必须一样或更高
                {
                    Logger.Error($"The target version that value is \"{appVersion.Major}.{appVersion.Minor}.{appVersion.Patch}\" " +
                                 $"is lower or equal to last build ,last build verison is \"" +
                                 $"{lastVersionInfo.versionInfo.version}\" .");
                    return false;
                }
            }
            else
            {
                Logger.Info($"The last build info is not exist .");
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
            this.Setup(filter,input);
            this.State = ActionState.Completed;
        }


        private void Setup(IFilter filter, IPipelineInput input)
        {
            input.SetData(EnvironmentVariables.MAKE_BASE_APP_VERSION_KEY, true);
        }

        #endregion

    }
}