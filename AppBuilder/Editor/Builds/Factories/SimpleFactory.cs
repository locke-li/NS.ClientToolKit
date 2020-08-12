/***************************************************************

 *  类名称：        Factories

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/22 20:15:56

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenturyGame.AppUpdaterLib.Runtime.Manifests;
using UnityEditor;

namespace CenturyGame.AppBuilder.Editor.Builds.Factories
{
    public static class SimpleFactory
    {

        public static AppInfoManifest CreateAppInfoManifest()
        {
            var appInfoManifest = new AppInfoManifest
            {
                TargetPlatform = EditorUserBuildSettings.activeBuildTarget.ToString()
            };
            return appInfoManifest;
        }
    }
}
