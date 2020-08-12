/***************************************************************

 *  类名称：        AppVersion

 *  描述：			游戏的版本信息

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/23 10:27:07

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;

namespace CenturyGame.AppUpdaterLib.Runtime
{

    /// <summary>
    /// App 头信息
    /// </summary>
    [Serializable]
    public class AppHeader
    {
        
        public string Name = "HotfixDemo";
    }


    [Serializable]
    public class AppVersion
    {
        /// <summary>
        /// 主版本号(必须是数字)，具有如下特点：
        /// 1).游戏有重大功能性的更新或者认为强制定义该App的大版本号
        /// 2).游戏必须下载并安装新的安装包，API已无法兼容（产生了新的API或者API发生了变化，这里一般指C#代码或本地代码发生了重大变更导致
        /// 需要更新新的版本）
        /// </summary>
        public string Major = "0";

        /// <summary>
        /// 次版本号（必须是数字)，特点如下：
        /// 1).同上2，游戏必须下载并安装新的安装包
        /// </summary>
        public string Minor = "0";

        /// <summary>
        /// 修订号(必须是数字)
        /// 在主版本号和次版本号不变的情况下，API没有发生变化，但是修复了若干bug，此种情况下不需要更新安装包
        /// 对于使用Lua做开发语言的游戏来说，如果Native代码没有发生变化，理论上这个值会跟着升高
        /// </summary>
        public string Patch = "0";

        /// <summary>
        /// 版本号后缀
        /// </summary>
        public string VersionSuffix = "-alpha00";

        /// <summary>
        /// 版本编译信息
        /// </summary>
        public string BuildMetadata = "";


        public string GetBaseVersion()
        {
            return $"{Major}.{Minor}.{Patch}";
        }
    }


    /// <summary>
    /// App 配置信息
    /// </summary>
    [Serializable]
    public class AppConfig
    {
        /// <summary>
        /// App的头信息
        /// </summary>
        public AppHeader appHeader;

        /// <summary>
        /// App 版本信息
        /// </summary>
        public AppVersion appVersion;

    }
}
