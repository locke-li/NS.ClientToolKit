/***************************************************************

 *  类名称：        UploadFilesAction

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/27 13:21:00

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System.Diagnostics;
using System.IO;
using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using UnityEngine;
using CenturyGame.Core.Pipeline;
using CenturyGame.Editor.UploadUtilitis.WinSCP;
using CenturyGame.AppBuilder.Editor.UploadUtilitis.AmazonS3;
using UnityEditor;
using Debug = UnityEngine.Debug;
using File = CenturyGame.Core.IO.File;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResPack
{
    public class UploadFilesAction : BaseBuildFilterAction
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

        public override bool Test(IFilter filter, IPipelineInput input)
        {
            var localUploadEnginePath = AmazonS3UploadUtility.TARGET_UPLOAD_ENGINE_PATH;

            if (!File.Exists(localUploadEnginePath))
            {
                var appBuildContext = AppBuildContext;
                appBuildContext.ErrorSb.AppendLine($"The target upload engine that path is \"{localUploadEnginePath}\" is not exist!");
                return false;
            }

            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            var appBuildContext = AppBuildContext;
            var sourceFolderPath = appBuildContext.UploadFileFolder;
            
            //sourceFolderPath = EditorUtils.OptimazePath($"{Application.dataPath}/../Package/ABCD");
            Logger.Info($"Start upload files , path is \" {sourceFolderPath}\" .");

            this.UploadFiles(sourceFolderPath);

            this.State = ActionState.Completed;
        }

        private void UploadFiles(string sourceFolder)
        {
            string pythonScripPath = $"{Application.dataPath}/../Tools/ProtokitUpload/ProtokitGoUploader.py";
            
            pythonScripPath = EditorUtils.OptimazePath(pythonScripPath);
            Debug.Log($"Lua projecet root path : {pythonScripPath} .");

            var configRepoPath = AppBuildConfig.GetAppBuildConfigInst().upLoadInfo.dataResAbsolutePath;
            if (!Directory.Exists(configRepoPath))
            {
                throw new DirectoryNotFoundException(configRepoPath);
            }

            var versionInfoFilePath = $"{configRepoPath}/gen/rawdata/server/resource_versions.release";
            var uploadFolder = sourceFolder;

            string platformName = string.Empty;
            platformName = "ios";

#if UNITY_EDITOR && UNITY_ANDROID
            platformName = "android";
#elif UNITY_EDITOR && UNITY_IPHONE
            platformName = "ios";
#endif
            var uploadFilesPattern = AppBuildConfig.GetAppBuildConfigInst().upLoadInfo.uploadFilesPattern;

            var remoteDir = AppBuildConfig.GetAppBuildConfigInst().upLoadInfo.remoteDir;

            string commandLineArgs = $"{pythonScripPath} {versionInfoFilePath} {platformName} {uploadFilesPattern} {uploadFolder} {remoteDir}";
            
            Debug.Log($"commandline args : {commandLineArgs}");

            var pStartInfo = new ProcessStartInfo();

#if UNITY_EDITOR_WIN
            pStartInfo.FileName = @"python.exe";
#elif UNITY_EDITOR_OSX
            pStartInfo.FileName = @"python";
#else
        throw new InvalidOperationException($"Unsupport build platform : {EditorUserBuildSettings.activeBuildTarget} .");
#endif


            pStartInfo.UseShellExecute = false;

            pStartInfo.RedirectStandardInput = false;
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.RedirectStandardError = true;
            var workDir = Path.GetDirectoryName(pythonScripPath);
            workDir = EditorUtils.OptimazePath(workDir);
            pStartInfo.WorkingDirectory = workDir;

            pStartInfo.CreateNoWindow = true;
            pStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            pStartInfo.Arguments = commandLineArgs;

            pStartInfo.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
            pStartInfo.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;

            var proces = Process.Start(pStartInfo);
            string standardOutput = proces.StandardOutput.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(standardOutput))
                UnityEngine.Debug.Log(standardOutput);

            string standardErroOutput = proces.StandardError.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(standardErroOutput))
                UnityEngine.Debug.LogError(standardErroOutput);

            proces.WaitForExit();
            proces.Close();
            Debug.Log("Upload lua file successful!");

            AssetDatabase.Refresh();
        }

#endregion

    }
}
