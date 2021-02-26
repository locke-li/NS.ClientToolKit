/***************************************************************

 *  类名称：        BaseResManifestPaser

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/18 16:48:22

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


namespace CenturyGame.AppUpdaterLib.Runtime.ResManifestParser
{
    public abstract class BaseResManifestParser
    {
        public abstract VersionManifest Parse(string manifestContent);

        public abstract string Serialize(VersionManifest manifest);

        public abstract void WriteToAppInfo(string resVersion , string resVersionNum = null);

        public abstract UpdateResourceType GetUpdateResourceType();

    }
}
