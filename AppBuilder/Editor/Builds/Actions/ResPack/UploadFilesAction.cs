/***************************************************************

 *  类名称：        UploadFilesAction

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/27 13:21:00

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using UnityEngine;
using CenturyGame.Core.Pipeline;
using CenturyGame.Editor.UploadUtilitis.WinSCP;
using CenturyGame.AppBuilder.Editor.UploadUtilitis.AmazonS3;
using CenturyGame.Core.IO;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    public class UploadFilesAction : BaseBuildFilterAction
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
            var localUploadEnginePath = AmazonS3UploadUtility.TARGET_UPLOAD_ENGINE_PATH;

            if (!File.Exists(localUploadEnginePath))
            {
                var appBuildContext = AppBuildContext;
                appBuildContext.ErrorSb.AppendLine($"The target upload engine that path is \"{localUploadEnginePath}\" is not exist!");
                return false;
            }

            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            var appBuildContext = AppBuildContext;
            var sourceFolderPath = appBuildContext.UploadFileFolder;
            
            //sourceFolderPath = EditorUtils.OptimazePath($"{Application.dataPath}/../Package/ABCD");
            Logger.Info($"Start upload files , path is \" {sourceFolderPath}\" .");

            this.UploadFiles(sourceFolderPath);

            this.State = ActionState.Completed;
        }

        private void UploadFiles(string sourceFolder)
        {
            //AmazonS3UploadUtility.Start(sourceFolder);
            AmazonS3UploadUtility.Start(sourceFolder);
        }

        #endregion

    }
}
