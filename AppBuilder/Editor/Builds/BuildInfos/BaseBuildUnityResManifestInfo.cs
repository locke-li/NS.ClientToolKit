/***************************************************************

 *  类名称：        BaseBuildUnityResManifestInfo

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/22 20:45:07

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.AppUpdaterLib.Runtime;
using CenturyGame.Core.Functional;
using CenturyGame.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenturyGame.AppBuilder.Editor.Builds.BuildInfos
{
    [Serializable]
    public class BaseBuildUnityResManifestInfo
    {
        public string appVersion = string.Empty;

        public string unityResVersion = string.Empty;

        public VersionManifest data = new VersionManifest();

        public void Copy(VersionManifest other)
        {
            other.Datas.ForEach(x => { other.Datas.Add(x.Clone()); });
        }

        public void Union(VersionManifest other)
        {
            Dictionary<string, FileDesc> tempDic = new Dictionary<string, FileDesc>();
            data.Datas.ForEach(x=>tempDic[x.N] = x);

            other.Datas.ForEach(x =>
            {
                FileDesc targetFileDesc;
                if (tempDic.TryGetValue(x.N , out targetFileDesc))
                {
                    if (targetFileDesc != null && !string.Equals(targetFileDesc.H, x.H, StringComparison.Ordinal))
                    {
                        targetFileDesc.H = x.H;
                    }
                }
                else
                {
                    tempDic.Add(x.N,x);
                }
            });

            List<FileDesc> fileDescList = new List<FileDesc>();
            tempDic.ForeachCall(x => { fileDescList.Add(x.Value); });
            data.Datas = fileDescList;
        }
    }
}
