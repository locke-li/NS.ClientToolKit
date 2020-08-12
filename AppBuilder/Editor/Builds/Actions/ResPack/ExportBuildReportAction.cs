/***************************************************************

 *  类名称：        ExportBuildReportAction

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/21 18:08:02

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.IO;
using CenturyGame.AppBuilder.Editor.Builds.BuildInfos;
using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.Core.Pipeline;
using UnityEditor;
using UnityEngine;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    public class ExportBuildReportAction : BaseMakeVersionAction
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
            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            this.WriteBuildReporterFile(filter,input);
            this.State = ActionState.Completed;
        }

        private void WriteBuildReporterFile(IFilter filter, IPipelineInput input)
        {
            var reporter = new BuildReportInfo();

            var now = DateTime.Now;
            string timeStr = now.ToString("yyyy-MM-dd HH-mm-ss");
            reporter.meta.buildTime = timeStr;
            reporter.meta.unityVersion = Application.unityVersion;
            reporter.meta.machineName = System.Environment.MachineName;

            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            reporter.buildTarget = activeBuildTarget.ToString();

            var appBuildContext = AppBuildContext;
            reporter.unityResVerison = appBuildContext.AppInfoManifest.unityDataResVersion;
            reporter.dataResVersion = appBuildContext.AppInfoManifest.dataResVersion;
            string json = appBuildContext.ToJson(reporter);

            string timeStr2 = now.ToString("yyyyMMddHHmmss");
            string targetFileName = string.Format(BuildReportInfo.BuildReportFileNamePattern,
                timeStr2,
                appBuildContext.AppInfoManifest.version);

            string targetPath = appBuildContext.GetBuildReportsPath(targetFileName);
            File.WriteAllText(targetPath,json, appBuildContext.TextEncoding);
            AssetDatabase.Refresh();
            Logger.Info("Write build report complted!");
        }

        #endregion
    }
}
