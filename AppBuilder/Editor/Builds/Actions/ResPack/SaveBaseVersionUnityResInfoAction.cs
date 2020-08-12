/***************************************************************

 *  类名称：        SaveBasicBuildVersionInfoAction

 *  描述：		    保留当前基础版本编译的unity资源文件清单信息，用于和后续文件对比差异得出Patch列表

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/27 12:24:19

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System.IO;
using UnityEngine;
using CenturyGame.Core.Pipeline;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.AppBuilder.Editor.Builds.Contexts;
#if DEBUG_FILE_CRYPTION
using File = CenturyGame.Core.IO.File;
#else
using File = System.IO.File;
#endif

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    public class SaveBaseVersionUnityResInfoAction : BaseBuildFilterAction
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
            this.SaveBulidVersionInfo(filter, input);
            this.State = ActionState.Completed;
        }
        public void SaveBulidVersionInfo(IFilter filter, IPipelineInput input)
        {
            var context = AppBuildContext;
            LastBuildVersion lastVersion = new LastBuildVersion();
            lastVersion.Version = context.AppInfoManifest.unityDataResVersion;

            lastVersion.Info = context.VersionManifest;
            string targetFile = context.LastBuildUnityResManifestPath;
            var dat = context.ToJson(lastVersion);
            File.WriteAllBytes(targetFile, System.Text.Encoding.UTF8.GetBytes(dat));
            Logger.Info($"Save file \"{targetFile}\" completed!");
        }

        #endregion


    }
}
