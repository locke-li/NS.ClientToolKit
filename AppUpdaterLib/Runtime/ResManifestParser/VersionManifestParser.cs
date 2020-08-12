/***************************************************************

 *  类名称：        VersionManifestParser

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/12 10:12:43

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using System.Collections.Generic;
using CenturyGame.Core.ResourcePools;

namespace CenturyGame.AppUpdaterLib.Runtime.ResManifestParser
{
    public  static class VersionManifestParser
    {

        private static ResourcePool<FileDesc> s_mPools = null;

        public static ResourcePool<FileDesc> Pools
        {
            get
            {
                Init();
                return s_mPools;
            }
        }

        private static bool mInitialize = false;

        public static void Init()
        {
            if (mInitialize)
                return;
            mInitialize = true;
            s_mPools = new ResourcePool<FileDesc>(()=>new FileDesc(),null,null);
        }

        public static VersionManifest Parse(string manifestContent)
        {
            List<FileDesc> list = new List<FileDesc>();

            var doc = new JSONObject(manifestContent);

            const char wellNumChar = '#';
            var keys = doc.keys;
            foreach (var key in keys)
            {
                var val = doc[key].str;
                var splitStrs = val.Split(wellNumChar);
                var name = key.Trim();
                var hash = splitStrs[0];
                var size = Convert.ToInt32(splitStrs[1]);

                FileDesc desc = Pools.Obtain();
                desc.N = name;
                desc.H = hash;
                desc.S = size;
                list.Add(desc);
            }

            VersionManifest manifest = new VersionManifest()
            {
                Datas = list
            };
            return manifest;
        }

        public static string Serialize(VersionManifest manifest)
        {
            var doc = new JSONObject();

            for (int i = 0; i < manifest.Datas.Count; i++)
            {
                var desc = manifest.Datas[i];
                doc.AddField(desc.N, $"{desc.H}#{desc.S}");
            }

            return doc.ToString();
        }

    }
}