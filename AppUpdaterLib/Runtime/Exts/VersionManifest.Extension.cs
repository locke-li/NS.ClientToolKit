/***************************************************************

 *  类名称：        VersionManifest

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/5/18 20:54:55

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using CenturyGame.Core.Functional;
using CenturyGame.LoggerModule.Runtime;

namespace CenturyGame.AppUpdaterLib.Runtime
{
	public sealed partial class VersionManifest
    {

        private static readonly ILogger s_mLogger = LoggerManager.GetLogger("VersionManifest");

        private readonly Dictionary<string,FileDesc> mFileDscsDic = new Dictionary<string, FileDesc>();

        private bool mDicIsInitialized = false;

        public void InitDic()
        {
            if (this.mDicIsInitialized)
            {
                return;
            }
            this.Datas.ForCall((x, index) =>
            {
                var data = this.Datas[index];
                var key = data.N;
                mFileDscsDic[key] = data;
            });
            this.mDicIsInitialized = true;
        }

        public List<FileDesc> CalculateDifference(VersionManifest other)
        {
            this.InitDic();
            List<FileDesc> diff = null;

            foreach (var fileDesc in other.Datas)
            {
                if (this.mFileDscsDic.TryGetValue(fileDesc.N,out var desc))
                {
                    if (String.Compare(desc.H, fileDesc.H, StringComparison.Ordinal) != 0)
                    {
                        if (diff == null)
                            diff = new List<FileDesc>();
                        diff.Add(fileDesc);
                    }
                }
                else
                {
                    if(diff == null)
                        diff = new List<FileDesc>();
                    diff.Add(fileDesc);
                }
            }

            return diff;
        }


        public void UpdateInnerFile(FileDesc desc)
        {
            this.InitDic();
            FileDesc foundDesc = null;

            s_mLogger.Debug($"Update inner file that name is \"{desc.N}\" .");
            if (this.mFileDscsDic.TryGetValue(desc.N, out foundDesc))
            {
                desc.CopyTo(foundDesc);
            }
            else
            {
                this.mFileDscsDic.Add(desc.N,desc);
                this.Datas.Add(desc);
            }
        }

    }
}
