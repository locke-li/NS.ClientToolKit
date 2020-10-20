/***************************************************************

 *  类名称：        AssetBundleAction

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/26 14:20:16

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using UnityEngine;
using CenturyGame.Core.Pipeline;
using System.IO;
using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using CenturyGame.ClientToolKit.AppSetting.Runtime;
using UnityEngine.Video;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResProcess
{
    public class AssetBundleAction : BaseBuildFilterAction
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

        #endregion

        public override bool Test(IFilter filter, IPipelineInput input)
        {
            string configPath = GetCurrentAssetGraphProjectConfigPath();

            Logger.Info($"AssetsGraph config path : \"{configPath}\" .");

            if (!File.Exists(configPath))
            {
                var appBuildContext = AppBuildContext;
                appBuildContext.ErrorSb.AppendLine($"The assetgraph config that path is \"{configPath}\" is not exist!");
                return false;
            }

            //var appSetting = Resources.Load<AppSetting>("AppSetting");
            //if (appSetting == null)
            //{
            //    var appBuildContext = AppBuildContext;
            //    appBuildContext.AppendErrorLog($"Pleause create a appSetting asset !");
            //    return false;
            //}
            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            string configPath = GetCurrentAssetGraphProjectConfigPath(true);
            Logger.Info($"AssetGraph configPath : \"{configPath}\" .");
            UnityEngine.AssetGraph.AssetGraphUtility.ExecuteGraph(configPath);
            this.State = ActionState.Completed;
        }


        private string GetCurrentAssetGraphProjectConfigPath(bool relative = false)
        {
            string targetConfigPath = AppBuildConfig.GetAppBuildConfigInst().TargetAssetGraphConfigAssetsPath;
            targetConfigPath = EditorUtils.OptimazePath(targetConfigPath, false);

            string filePath;
            if (relative)
            {
                filePath = targetConfigPath;
            }
            else
            {
                filePath = $"{Application.dataPath}/../{targetConfigPath}";
                filePath = EditorUtils.OptimazePath(filePath);
            }

            return filePath;
        }
    }
}
