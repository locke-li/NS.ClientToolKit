/**************************************************************
 *  类名称：          CopyLuaAssetBundleToUploadStorageAction
 *  描述：
 *  作者：            Chico(wuyuanbing)
 *  创建时间：        2020/12/7 14:29:19
 *  最后修改人：
 *  版权所有 （C）:   CenturyGames
 **************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenturyGame.Core.Pipeline;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    class CopyLuaAssetBundleToUploadStorageAction : BaseBuildFilterAction
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
            var luaprojectDir = input.GetData<string>("LuaProject", AppBuildContext.GetLuaProjectPath());

            if (string.IsNullOrEmpty(luaprojectDir))
            {
                AppBuildContext.ErrorSb.AppendLine($"The LuaProject not set!");
                return false;
            }

            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            var resStorage = AppBuildContext.GetResStoragePath();
            this.CopyLuaAssetBundleToUploadStorage(filter,input, resStorage);
            this.State = ActionState.Completed;
        }


        private void CopyLuaAssetBundleToUploadStorage(IFilter filter, IPipelineInput input, string resStorage)
        {
            var luaAbPath = input.GetData(EnvironmentVariables.LUA_AB_PATH_KEY, "");
            if (string.IsNullOrEmpty(luaAbPath))
            {
                throw new FileNotFoundException("Null path!");
            }

            if (!File.Exists(luaAbPath))
            {
                throw new FileNotFoundException(luaAbPath);
            }

            var targetPath = $"{resStorage}/lua.x";
            File.Copy(luaAbPath, targetPath, true);
        }

        #endregion

    }
}