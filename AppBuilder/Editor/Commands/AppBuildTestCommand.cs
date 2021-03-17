/***************************************************************

 *  类名称：        AppBuildTestCommand

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/26 11:51:48

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using UnityEditor;
using CenturyGame.AppBuilder.Editor.Builds;
using CenturyGame.AppBuilder.Editor.Builds.Actions.ResProcess;
using CenturyGame.Core.Pipeline;
using CenturyGame.AppBuilder.Editor.Builds.Filters.Concrete;
using CenturyGame.AppBuilder.Editor.Builds.PipelineInputs;
using UnityEngine;

namespace CenturyGame.AppBuilder.Editor.Commands
{
    public static class AppBuildTestCommand
    {
        //[MenuItem("CenturyGame/AppBuild/Commands/Test Build App")]
        static void TestBuild()
        {
            AppBuilderPipelineProcessor processor = new AppBuilderPipelineProcessor();

            ResourcesProcessFilter resourcesProcessFilter = new ResourcesProcessFilter(true);
            ResourcesPackageFilter resourcesPackageFilter = new ResourcesPackageFilter(true);

            processor.Register(resourcesProcessFilter);
            processor.Register(resourcesPackageFilter);

            AppBuildPipelineInput input = new AppBuildPipelineInput();
            var result = processor.Process(input);

            if (result.State == ProcessState.Error)
            {
                Debug.LogError($"Build app failure , error message : {result.Message}");
            }
            else
            {
                Debug.Log("Build app completed!");
            }
            
        }

        //[MenuItem("Commands/Test copy rules")]
        static void TestCopyRules()
        {
            AppBuilderPipelineProcessor processor = new AppBuilderPipelineProcessor();

            ResourcesProcessFilter resourcesProcessFilter = new ResourcesProcessFilter(false);
            //resourcesProcessFilter.Enqueue(new TestOtherFilesExportAction());
            //ResourcesPackageFilter resourcesPackageFilter = new ResourcesPackageFilter(true);

            processor.Register(resourcesProcessFilter);
            //processor.Register(resourcesPackageFilter);

            AppBuildPipelineInput input = new AppBuildPipelineInput();
            var result = processor.Process(input);

            if (result.State == ProcessState.Error)
            {
                Debug.LogError($"Build app failure , error message : {result.Message}");
            }
            else
            {
                Debug.Log("Build app completed!");
            }

        }

        //[MenuItem("test/copy standard assets")]
        static void CopyStandardAssetFolder()
        {
            string sourcePath = EditorUtils.OptimazePath(Application.dataPath+ "/Standard Assets");

            string targetPath = EditorUtils.OptimazePath(@"F:\CenturyGameGitRepositories\Standard Assets");


            EditorUtils.CopyDirecotryToDestination(sourcePath,targetPath, x =>
            {
                if (x.EndsWith(".meta"))
                {
                    return true;
                }

                if (x.Contains(".get"))
                {
                    return true;
                }
                return false;
            });

            Debug.LogError("Copy standard asset completed!");
        }
    }
}
