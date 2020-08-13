using CenturyGame.Core.Pipeline;
using UnityEngine;
using System;
using System.IO;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    public class LuaScriptsCopyAction : BaseBuildFilterAction
    {
        public override bool Test(IFilter filter, IPipelineInput input)
        {
            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            var context = AppBuildContext;
            string luaRootPath = context.GetLuaScriptsFolderPath();
            Logger.Info($"Lua Root Path : {luaRootPath}");
            string copyPath = EditorUtils.OptimazePath(Application.streamingAssetsPath + "/lua"); //(Application.streamingAssetsPath + "/lua").Replace('/', '\\');
            ClearOldScripts(filter, input, copyPath);
            CopyScripts(filter, input, luaRootPath, copyPath);
            this.State = ActionState.Completed;
        }

        private void ClearOldScripts(IFilter filter, IPipelineInput input, string scriptFolder)
        {
            if (Directory.Exists(scriptFolder))
            {
                Directory.Delete(scriptFolder, true);
            }
        }

        private void CopyScripts(IFilter filter, IPipelineInput input, string srcPath, string dstPath)
        {
            try
            {
                Logger.Info($"copy lua script, src:{srcPath}, dst:{dstPath}");
                if (!Directory.Exists(dstPath))
                {
                    Directory.CreateDirectory(dstPath);
                }

                string[] files = Directory.GetFiles(srcPath, "*.lua");
                for (int i = 0; i < files.Length; i++)
                {
                    File.Copy(srcPath + "/" + Path.GetFileName(files[i]), dstPath + "/" + Path.GetFileName(files[i]));
                }

                string[] dirs = Directory.GetDirectories(srcPath);
                for (int i = 0; i < dirs.Length; i++)
                {
                    dirs[i] = EditorUtils.OptimazePath(dirs[i]);
                    int idx = dirs[i].LastIndexOf('/');
                    string subFolder = dirs[i].Substring(idx + 1);
                    CopyScripts(filter, input, srcPath + "/" + subFolder, dstPath + "/" + subFolder);
                }
            }
            catch (Exception e)
            {
                Logger.Warn($"copy lua files throw exception : {e.Message}");
            }
        }
    }
}