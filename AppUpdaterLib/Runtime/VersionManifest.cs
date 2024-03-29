/***************************************************************

 *  类名称：        VersionManifest

 *  描述：			版本比对文件清单

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/26 15:29:12

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.AppUpdaterLib.Runtime.ResManifestParser;
using System;
using System.Collections.Generic;

namespace CenturyGame.AppUpdaterLib.Runtime
{
    [Serializable]
    public class FileDesc
    {
        public string N;

        public int S;

        public string H;

        public string RN;//服务器目录前缀

        public FileDesc() { }

        public FileDesc(FileDesc info)
        {
            this.N = info.N;
            this.S = info.S;
            this.H = info.H;
            this.RN = info.RN;
        }

        public FileDesc Clone()
        {
            var desc = VersionManifestParser.Pools.Obtain();
            desc.N = this.N;
            desc.H = this.H;
            desc.S = this.S;
            desc.RN = this.RN;
            return desc;
        }

        public void CopyTo(FileDesc other)
        {
            other.N = this.N;
            other.S = this.S;
            other.H = this.H;
            other.RN = this.RN;
        }

        public string GetRNUTF8()
        {
            return this.RN.Replace("#", CommonConst.WellNumUtf8);
        }
    }

    [Serializable]
    public class ABTableItemInfoClient : FileDesc
    {
        public bool R;//replace

        public ABTableItemInfoClient()
        {
        }
        public ABTableItemInfoClient(FileDesc info) : base(info)
        {
            R = false;
        }
    }

    [Serializable]
    public sealed partial class VersionManifest
    {
        public List<FileDesc> Datas = new List<FileDesc>();
    }

    [Serializable]
    public sealed class ABConfigInfoClient
    {
        public ABConfigInfoClient(VersionManifest dat)
        {
            if (dat != null)
            {
                List = new ABTableItemInfoClient[dat.Datas.Count];
                for (int i = 0; i < dat.Datas.Count; i++)
                {
                    List[i] = new ABTableItemInfoClient(dat.Datas[i]);
                }
            }
        }
        public ABConfigInfoClient()
        {

        }
        public string Ver = null;
        public ABTableItemInfoClient[] List;

        [NonSerialized]
        private Version mVersion;
        public Version GetVersion()
        {
            if (mVersion == null)
                mVersion = new Version(Ver);
            return mVersion;
        }
    }
}
