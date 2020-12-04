/***************************************************************

 *  类名称：        MakeBaseVersionAction

 *  描述：			制作当前App的基础资源版本

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/6 13:34:15

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using CenturyGame.AppBuilder.Editor.Builds.BuildInfos;
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
    class MakeAppBaseVersionAction : BaseMakeVersionAction
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
            Save(filter, input); 
            this.State = ActionState.Completed;
        }

        private bool Save(IFilter filter, IPipelineInput input)
        {
            var appBuildContext = AppBuildContext;
            appBuildContext.AppInfoManifest.version = AppBuildContext.GetTargetAppVersion(true).GetVersionString();
            var streamingPath = AppBuildContext.GetAssetsOutputPath();
            
            var resFileListPath = $"{streamingPath}/res_{AppBuildContext.GetPlatformStrForUpload()}.json";
            appBuildContext.AppInfoManifest.unityDataResVersion = EditorUtils.GetMD5(resFileListPath);

            var resDataFileListPath = $"{streamingPath}/res_data.json";
            appBuildContext.AppInfoManifest.dataResVersion = EditorUtils.GetMD5(resDataFileListPath);

            var builtinAppInfoFilePath = appBuildContext.GetBuiltinAppInfoFilePath();
            var appInfoJson = appBuildContext.ToJson(appBuildContext.AppInfoManifest);
            File.WriteAllText(builtinAppInfoFilePath, appInfoJson,appBuildContext.TextEncoding);
            Logger.Info($"Save file \"{builtinAppInfoFilePath}\" completed");

            //保存编译信息
            var lastBuildInfo = appBuildContext.GetLastBuildInfo();
            if (lastBuildInfo == null) // 
            {
                lastBuildInfo = new LastBuildInfo();
                lastBuildInfo.baseVersionInfo = appBuildContext.AppInfoManifest;
                lastBuildInfo.versionInfo = appBuildContext.AppInfoManifest;
            }
            else
            {
                lastBuildInfo.versionInfo = appBuildContext.AppInfoManifest;
            }
            appBuildContext.SaveLastBuildInfo(lastBuildInfo);

            AssetDatabase.Refresh();
            return true;
        }

        #endregion
    }
}
