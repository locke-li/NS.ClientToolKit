/***************************************************************

 *  类名称：        AppBuildConfig

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/26 17:39:23

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.IO;
using CenturyGame.ClientToolKit.AppSetting.Runtime;
using UnityEditor;
using UnityEngine;

namespace CenturyGame.AppBuilder.Editor.Builds
{
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
        public string Patch = "1";

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


    public class AppBuildConfig : ScriptableObject
    {

        public string buildCacheRelativePath = "/../AppBuildCache";

        public string LastBuildVersionName = "LastBuildVersion.json";
        public string LastBuildInfoName = "LastBuildInfo.json";

        public string BuildInfoFolder
        {
            get
            {
                var result = GetBuildCacheFolderPath() + "/BuildInfos";
                if (!Directory.Exists(result))
                    Directory.CreateDirectory(result);
                return result;
            }
        }

        [Header("当前工程资源根目录名(相对于Assets文件夹)")]
        public string ResourcesFolder = "ResourcesAB";

        [Header("AssetsBundle资源临时输出目录名")]
        public string AssetBundleExportFolderName = "AB";

        public string LastBuildVersionPath => BuildInfoFolder + @"/"+ LastBuildVersionName;
        public string LastBuildInfoPath => BuildInfoFolder + @"/" + LastBuildInfoName;

        public string AppBuildConfigFolder => Application.dataPath + $"{GetRelativeToAssetsPath()}/Editor/BuildConfigs";

        public AppVersion targetAppVersion;

        [Header("此路径需要包含“Assets/”前缀，举例：Assets/AssetGraphConfig/demo.asset")]
        public string targetAssetGraphConfigRelativeToProjectPath = "";

        public string TargetAssetGraphConfigAssetsPath => $"{targetAssetGraphConfigRelativeToProjectPath}";

        /// <summary>
        /// 远端拉取下来的数据表路径。
        /// </summary>
        public string dataResAbsolutePath = "";

        [Header("制作Patch版本时自增修订号")]
        public bool incrementRevisionNumberForPatchBuild = true;

        [Header("负责亚马逊S3上传的可执行文件路径，相对于Assets目录，举例：/../UploadTools/xxx.bat")]
        public string AmazonS3UpLoadEngineRelativeToAssetsPath = "";

        [Header("负责腾讯云上传的可执行文件路径，相对于Assets目录，举例：/../UploadTools/xxx.bat")]
        public string TencentCloudUpLoadEngineRelativeToAssetsPath = "";


        public static AppBuildConfig GetAppBuildConfigInst()
        {
            var config = AssetDatabase.LoadAssetAtPath<AppBuildConfig>(GetConfigRelativeToProjectPath());

            if (!config)
            {
                throw new FileNotFoundException($"The file that path is \"{GetConfigRelativeToProjectPath()}\" is not found!");
            }
            return config;
        }

        public string GetBuildCacheFolderPath()
        {
            string pathFolder = Application.dataPath + buildCacheRelativePath;
            pathFolder = EditorUtils.OptimazePath(pathFolder);

            if(!Directory.Exists(pathFolder))
                Directory.CreateDirectory(pathFolder);
            return pathFolder;
        }

        private static string GetRelativeToAssetsPath()
        {
            return "/CenturyGamePackageRes/AppBuilder";
        }

        private static string GetRootPath()
        {
            return EditorUtils.OptimazePath(Application.dataPath + GetRelativeToAssetsPath());
        }

        public static string GetConfigAbsoluteFolderPath()
        {
            return $"{GetRootPath()}/Editor";
        }

        public static string GetConfigAbsolutePath()
        {
            return $"{GetRootPath()}/Editor/AppBuilderConfig.asset";
        }

        public static string GetConfigRelativeToProjectPath()
        {
            return $"Assets{GetRelativeToAssetsPath()}/Editor/AppBuilderConfig.asset";
        }
    }
}
