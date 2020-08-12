/////***************************************************************

//// *  类名称：        AppConfigMakeCommand

//// *  描述：				

//// *  作者：          Chico(wuyuanbing)

//// *  创建时间：      2020/5/7 17:57:37

//// *  最后修改人：

//// *  版权所有 （C）:   diandiangames

////***************************************************************/

////using System;
////using System.Collections.Generic;
////using System.IO;
////using System.Text;
////using CenturyGame.AppBuilder.Editor.Builds;
////using CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack;
////using CenturyGame.AppBuilder.Editor.Builds.Actions.ResProcess;
////using CenturyGame.AppBuilder.Editor.Builds.Configuration;
////using CenturyGame.AppBuilder.Editor.Builds.Filters.Concrete;
////using UnityEditor;
////using UnityEngine;
////using YamlDotNet;

////namespace CenturyGame.AssetBundleManager.Editor.Commands
////{
////    internal class AppConfigMakeCommand
////    {
////        [MenuItem("Commands/MakeAppConfig/Make a patch config")]

//using CenturyGame.AppBuilder.Editor.Builds;
//using CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack;
//using CenturyGame.AppBuilder.Editor.Builds.Actions.ResProcess;
//using CenturyGame.AppBuilder.Editor.Builds.Configuration;
//using CenturyGame.AppBuilder.Editor.Builds.Filters.Concrete;
//using UnityEditor;
//using YamlDotNet;
//using System;
//using System.Collections.Generic;

//static void MakePatchBuildConfig()
//{
//    AppBuildProcessConfig config = new AppBuildProcessConfig();
//    config.Filters = new List<AppBuildFilterInfo>();

//    Type resourceProcessFilterType = typeof(ResourcesProcessFilter);
//    AppBuildFilterInfo filterInfo = new AppBuildFilterInfo();
//    filterInfo.TypeFullName = resourceProcessFilterType.FullName;
//    filterInfo.Action.IsActionQueue = true;
//    config.Filters.Add(filterInfo);

//    AppBuildActionInfo testOtherFilesExportActionInfo = new AppBuildActionInfo();
//    testOtherFilesExportActionInfo.TypeFullName = typeof(TestOtherFilesExportAction).FullName;
//    filterInfo.Action.Childs.Add(testOtherFilesExportActionInfo);

//    AppBuildActionInfo assetBundleActionInfo = new AppBuildActionInfo();
//    assetBundleActionInfo.TypeFullName = typeof(AssetBundleAction).FullName;
//    filterInfo.Action.Childs.Add(assetBundleActionInfo);


//    Type resourcePackFilterType = typeof(ResourcesPackageFilter);
//    filterInfo = new AppBuildFilterInfo
//    {
//        TypeFullName = resourcePackFilterType.FullName
//    };
//    filterInfo.Action.IsActionQueue = true;
//    config.Filters.Add(filterInfo);

//    AppBuildActionInfo assetBundleFilesPackActionInfo = new AppBuildActionInfo();
//    assetBundleFilesPackActionInfo.TypeFullName = typeof(AssetBundleFilesPackAction).FullName;
//    filterInfo.Action.Childs.Add(assetBundleFilesPackActionInfo);

//    AppBuildActionInfo fileVersionManifestGenerateActionInfo = new AppBuildActionInfo();
//    fileVersionManifestGenerateActionInfo.TypeFullName = typeof(FileVersionManifestGenerateAction).FullName;
//    filterInfo.Action.Childs.Add(fileVersionManifestGenerateActionInfo);

//    AppBuildActionInfo makeAppPatchVersionActionInfo = new AppBuildActionInfo();
//    makeAppPatchVersionActionInfo.TypeFullName = typeof(MakeAppPatchVersionAction).FullName;
//    filterInfo.Action.Childs.Add(makeAppPatchVersionActionInfo);

//    AppBuildActionInfo saveLastBuildInfoActionInfo = new AppBuildActionInfo();
//    saveLastBuildInfoActionInfo.TypeFullName = typeof(SaveLastBuildInfoAction).FullName;
//    filterInfo.Action.Childs.Add(saveLastBuildInfoActionInfo);

//    AppBuildActionInfo uploadFilesActionInfo = new AppBuildActionInfo();
//    uploadFilesActionInfo.TypeFullName = typeof(UploadFilesAction).FullName;
//    filterInfo.Action.Childs.Add(uploadFilesActionInfo);

//    string yaml = YAMLSerializationHelper.Serialize(config);

//    var appBuildConfig = AppBuildConfig.GetAppBuildConfigInst();
//    string appBuildCacheFolder = appBuildConfig.AppBuildConfigFolder;
//    string pathFolder = appBuildCacheFolder;

//    pathFolder = EditorUtils.OptimazePath(pathFolder);
//    if (!Directory.Exists(pathFolder))
//        Directory.CreateDirectory(pathFolder);
//    string path = pathFolder + "/MakePatchVersion.yaml";

//    File.WriteAllText(path, yaml, new UTF8Encoding(false, true));
//    Debug.Log($"Write \"{path}\" success ! Info : \r\n" + yaml);
//    AssetDatabase.Refresh();
//}

////        [MenuItem("Commands/MakeAppConfig/Make a base config")]
////        static void MakeBaseBuildConfig()
////        {
////            AppBuildProcessConfig config = new AppBuildProcessConfig();
////            config.Filters = new List<AppBuildFilterInfo>();

////            Type resourceProcessFilterType = typeof(ResourcesProcessFilter);
////            AppBuildFilterInfo filterInfo = new AppBuildFilterInfo();
////            filterInfo.TypeFullName = resourceProcessFilterType.FullName;
////            filterInfo.Action.IsActionQueue = true;
////            config.Filters.Add(filterInfo);

////            AppBuildActionInfo testOtherFilesExportActionInfo = new AppBuildActionInfo();
////            testOtherFilesExportActionInfo.TypeFullName = typeof(TestOtherFilesExportAction).FullName;
////            filterInfo.Action.Childs.Add(testOtherFilesExportActionInfo);

////            AppBuildActionInfo assetBundleActionInfo = new AppBuildActionInfo();
////            assetBundleActionInfo.TypeFullName = typeof(AssetBundleAction).FullName;
////            filterInfo.Action.Childs.Add(assetBundleActionInfo);


////            Type resourcePackFilterType = typeof(ResourcesPackageFilter);
////            filterInfo = new AppBuildFilterInfo
////            {
////                TypeFullName = resourcePackFilterType.FullName
////            };
////            filterInfo.Action.IsActionQueue = true;
////            config.Filters.Add(filterInfo);

////            AppBuildActionInfo assetBundleFilesPackActionInfo = new AppBuildActionInfo();
////            assetBundleFilesPackActionInfo.TypeFullName = typeof(AssetBundleFilesPackAction).FullName;
////            filterInfo.Action.Childs.Add(assetBundleFilesPackActionInfo);

////            AppBuildActionInfo fileVersionManifestGenerateActionInfo = new AppBuildActionInfo();
////            fileVersionManifestGenerateActionInfo.TypeFullName = typeof(FileVersionManifestGenerateAction).FullName;
////            filterInfo.Action.Childs.Add(fileVersionManifestGenerateActionInfo);

////            AppBuildActionInfo makeAppBaseVersionActionInfo = new AppBuildActionInfo();
////            makeAppBaseVersionActionInfo.TypeFullName = typeof(MakeAppBaseVersionAction).FullName;
////            filterInfo.Action.Childs.Add(makeAppBaseVersionActionInfo);

////            AppBuildActionInfo saveBasicBuildVersionInfoActionInfo = new AppBuildActionInfo();
////            saveBasicBuildVersionInfoActionInfo.TypeFullName = typeof(SaveBasicBuildVersionInfoAction).FullName;
////            filterInfo.Action.Childs.Add(saveBasicBuildVersionInfoActionInfo);

////            AppBuildActionInfo saveLastBuildInfoActionInfo = new AppBuildActionInfo();
////            saveLastBuildInfoActionInfo.TypeFullName = typeof(SaveLastBuildInfoAction).FullName;
////            filterInfo.Action.Childs.Add(saveLastBuildInfoActionInfo);

////            AppBuildActionInfo uploadFilesActionInfo = new AppBuildActionInfo();
////            uploadFilesActionInfo.TypeFullName = typeof(UploadFilesAction).FullName;
////            filterInfo.Action.Childs.Add(uploadFilesActionInfo);

////            string yaml = YAMLSerializationHelper.Serialize(config);
////            var appBuildConfig = AppBuildConfig.GetAppBuildConfigInst();
////            string appBuildCacheFolder = appBuildConfig.AppBuildConfigFolder;
////            string pathFolder = appBuildCacheFolder;

////            pathFolder = EditorUtils.OptimazePath(pathFolder);
////            if (!Directory.Exists(pathFolder))
////                Directory.CreateDirectory(pathFolder);
////            string path = pathFolder + "/MakeBaseVersion.yaml";

////            File.WriteAllText(path, yaml, new UTF8Encoding(false, true));

////            Debug.Log($"Write \"{path}\" success ! Info : \r\n" + yaml);
////            AssetDatabase.Refresh();
////        }

////    }
////}
