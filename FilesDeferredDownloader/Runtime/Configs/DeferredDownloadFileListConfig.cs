/***************************************************************

 *  类名称：        AppUpdaterDeferredFileListConfig

 *  描述：			

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/12/22 20:46:26

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using System.Collections.Generic;
using CenturyGame.Core;

namespace CenturyGame.FilesDeferredDownloader.Runtime.Configs
{
    [Serializable]
    public class FileMD5Pair
    {
        public string fileName;
        public string md5;
    }

    [Serializable]
    public class DeferredDownloadFileListConfig
    {
        public List<FileMD5Pair> files = new List<FileMD5Pair>();

        public bool Exist(string fileSetName)
        {
            foreach (var file in files)
            {
                if (string.Equals(file.fileName, fileSetName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}