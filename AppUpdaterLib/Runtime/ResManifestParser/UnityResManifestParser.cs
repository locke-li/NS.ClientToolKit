/***************************************************************

 *  类名称：        UnityResManifestParser

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/18 17:08:49

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.AppUpdaterLib.Runtime.Managers;
using UnityEngine;

namespace CenturyGame.AppUpdaterLib.Runtime.ResManifestParser
{
    internal class UnityResManifestParser : BaseResManifestParser
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

        public override VersionManifest Parse(string manifestContent)
        {
            var manifest = VersionManifestParser.Parse(manifestContent);

            return manifest;
        }

        public override string Serialize(VersionManifest manifest)
        {
            var content = VersionManifestParser.Serialize(manifest);

            return content;
        }

        public override void WriteToAppInfo(string resVersion, string resVersionNum = null)
        {
            AppVersionManager.AppInfo.unityDataResVersion = resVersion;

            if (!string.IsNullOrEmpty(resVersionNum))
            {
                Version version = new Version(AppVersionManager.AppInfo.version);
                version.Patch = resVersionNum;
                AppVersionManager.AppInfo.version = version.GetVersionString();
            }
            AppVersionManager.SaveCurrentAppInfo();
        }

        public override UpdateResourceType GetUpdateResourceType()
        {
            return UpdateResourceType.NormalResource;
        }

        #endregion


    }
}
