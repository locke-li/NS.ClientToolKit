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
    public class UnityResManifestParser : BaseResManifestParser
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

        public override string GetRemotePath(FileDesc desc)
        {
            return desc.GetRNUTF8();
        }

        public override string GetLocalRoot()
        {
            //return AssetsFileSystem.AppUnityResLocalRoot;
            return string.Empty;
        }

        public override void WriteToAppInfo(string resVersion)
        {
            AppVersionManager.AppInfo.unityDataResVersion = resVersion;
            AppVersionManager.SaveCurrentAppInfo();
        }

        public override UpdateResourceType GetUpdateResourceType()
        {
            return UpdateResourceType.NormalResource;
        }

        #endregion


    }
}
