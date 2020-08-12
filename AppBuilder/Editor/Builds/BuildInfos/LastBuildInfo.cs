/***************************************************************

 *  类名称：        LastBuildInfo

 *  描述：			记录上次编译的相关信息

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/7 10:40:58

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using CenturyGame.AppBuilder.Editor.Builds.Factories;
using CenturyGame.AppUpdaterLib.Runtime.Manifests;

namespace CenturyGame.AppBuilder.Editor.Builds.BuildInfos
{

    [Serializable]
    public class LastBuildInfo
    {
        public AppInfoManifest baseVersionInfo = SimpleFactory.CreateAppInfoManifest();

        public AppInfoManifest versionInfo = SimpleFactory.CreateAppInfoManifest();
    }

}
