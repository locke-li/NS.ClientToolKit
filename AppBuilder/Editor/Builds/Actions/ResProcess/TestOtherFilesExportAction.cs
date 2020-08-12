/***************************************************************

 *  类名称：        TestOtherFilesExportAction

 *  描述：			测试非AB文件的加入

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/26 20:21:57

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System.IO;
using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using CenturyGame.Core.Pipeline;
using UnityEngine;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResProcess
{
    public class TestOtherFilesExportAction : BaseBuildFilterAction
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
            var context = this.Context as AppBuildContext;
            var sourcePath = GetRulesPath(filter, input);

            if (!Directory.Exists(sourcePath))
            {
                context.ErrorSb.AppendLine($"The rules folder that path is \"{sourcePath}\" is not exist .");

                return false;
            }
            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            this.State = ActionState.Normal;
            this.CopyRules(filter, input);
            this.State = ActionState.Completed;
        }

        private string GetRulesPath(IFilter filter, IPipelineInput input)
        {
            var context = AppBuildContext;
            string sourcePath = Path.Combine(Application.dataPath, context.ResourcesFolder + "/" + context.Rules);
            sourcePath = Path.GetFullPath(sourcePath);
            sourcePath = sourcePath.Replace(@"\", @"/");
            return sourcePath;
        }


        private void CopyRules(IFilter filter, IPipelineInput input)
        {
            var context = AppBuildContext;

            var sourcePath = GetRulesPath(filter,input);
            string streamFolder = string.Concat(System.Environment.CurrentDirectory
                , Path.DirectorySeparatorChar
                , context.StreamingAssetsFolder);
            streamFolder = EditorUtils.OptimazePath(streamFolder);

            string desFolder = streamFolder + "/" + context.Rules;
            desFolder = Path.GetFullPath(desFolder);
            desFolder = desFolder.Replace(@"\", @"/");

            //Copy rules
            EditorUtils.CopyDirecotryToDestination(sourcePath, desFolder, x =>
            {
                if (x.EndsWith(".meta"))
                {
                    return true;
                }
                return false;
            });
        }


        #endregion

    }
}
