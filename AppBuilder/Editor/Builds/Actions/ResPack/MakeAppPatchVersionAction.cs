/***************************************************************

 *  类名称：        MakeAppPatchVersionAction

 *  描述：		    制作当前App的Patch资源版本

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/6 13:46:52

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CenturyGame.AppBuilder.Editor.Builds.BuildInfos;
using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using CenturyGame.AppBuilder.Runtime.Exceptions;
using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.Editor.UploadUtilitis;
using UnityEditor;
using UnityEngine;
using Version = CenturyGame.AppUpdaterLib.Runtime.Version;
using CenturyGame.Core.Pipeline;
using CenturyGame.AppBuilder.Editor.Builds.Exceptions;
#if DEBUG_FILE_CRYPTION
using File = CenturyGame.Core.IO.File;
#else
using File = System.IO.File;
#endif

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    public class MakeAppPatchVersionAction : BaseMakeVersionAction
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
            var lastBuildInfo = AppBuildContext.GetLastBuildInfo();
            if (lastBuildInfo == null || lastBuildInfo.GetCurrentBuildInfo() == null) 
            {
                AppBuildContext.AppendErrorLog("You can't make patch version , because you has no last build info .");
                return false;
            }
            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            this.Save(filter, input);
            this.State = ActionState.Completed;
        }

        private bool Save(IFilter filter, IPipelineInput input)
        {
            var appBuildContext = AppBuildContext;
            var streamingPath = AppBuildContext.GetAssetsOutputPath();

            appBuildContext.AppInfoManifest.version = AppBuildContext.GetTargetAppVersion().GetVersionString();
            
            var resFileListPath = $"{streamingPath}/res_{AppBuildContext.GetPlatformStrForUpload()}.json";
            appBuildContext.AppInfoManifest.unityDataResVersion = EditorUtils.GetMD5(resFileListPath);

            var resDataFileListPath = $"{streamingPath}/res_data.json";
            appBuildContext.AppInfoManifest.dataResVersion = EditorUtils.GetMD5(resDataFileListPath);

            var builtinAppInfoFilePath = appBuildContext.GetBuiltinAppInfoFilePath();
            var appInfoJson = appBuildContext.ToJson(appBuildContext.AppInfoManifest);

            //保存AppInfo文件
            File.WriteAllText(builtinAppInfoFilePath, appInfoJson, appBuildContext.TextEncoding);
            Logger.Info($"Save file \"{builtinAppInfoFilePath}\" completed");

            //保存编译信息
            var lastBuildInfo = appBuildContext.GetLastBuildInfo();
            lastBuildInfo.AddAppInfo(null, appBuildContext.AppInfoManifest);
            appBuildContext.SaveLastBuildInfo(lastBuildInfo);

            AssetDatabase.Refresh();
            return true;
        }

        #endregion
    }
}
