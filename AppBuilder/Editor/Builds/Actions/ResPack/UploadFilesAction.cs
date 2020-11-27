/***************************************************************

 *  类名称：        UploadFilesAction

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/27 13:21:00

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Channels;
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
    class UploadFilesAction : BaseBuildFilterAction
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
            string pythonScripPath = $"{Application.dataPath}/../Tools/ProtokitUpload/ProtokitGoUploader.py";

            pythonScripPath = EditorUtils.OptimazePath(pythonScripPath);
            Logger.Info($"Lua projecet root path : {pythonScripPath} .");

            if (!File.Exists(pythonScripPath))
            {
                AppBuildContext.ErrorSb.AppendLine($"The target upload script that path is \"{pythonScripPath}\" is not exist!");
                return false;
            }
            
            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            var resStorage = AppBuildContext.GetResStoragePath();

            Logger.Info($"Start upload files , path is \" {resStorage}\" .");

            var result = this.UploadFiles(resStorage,input);
            if (result)
            {
                this.State = ActionState.Completed;
            }
            else
            {
                this.State = ActionState.Error;
            }
        }

        private bool UploadFiles(string sourceFolder, IPipelineInput input)
        {
            string pythonScripPath = $"{Application.dataPath}/../Tools/ProtokitUpload/ProtokitGoUploader.py";
            
            pythonScripPath = EditorUtils.OptimazePath(pythonScripPath);
            Debug.Log($"Lua projecet root path : {pythonScripPath} .");

            var configRepoPath = AppBuildConfig.GetAppBuildConfigInst().upLoadInfo.dataResAbsolutePath;
            if (!Directory.Exists(configRepoPath))
            {
                throw new DirectoryNotFoundException(configRepoPath);
            }

            //var versionInfoFilePath = $"{configRepoPath}/gen/rawdata/server/resource_versions.release";
            var uploadFolder = sourceFolder;

            string platformName = string.Empty;
#if UNITY_EDITOR && UNITY_ANDROID
            platformName = "android";
#elif UNITY_EDITOR && UNITY_IPHONE
            platformName = "ios";
#else
            throw new InvalidOperationException($"Unsupport build platform : {EditorUserBuildSettings.activeBuildTarget} .");
#endif
            var uploadFilesPattern = AppBuildConfig.GetAppBuildConfigInst().upLoadInfo.uploadFilesPattern;

            var remoteDir = AppBuildConfig.GetAppBuildConfigInst().upLoadInfo.remoteDir;

            var makeBaseVersion = input.GetData(EnvironmentVariables.MAKE_BASE_APP_VERSION_KEY, false);
            var appVersion = AppBuildContext.GetTargetAppVersion(makeBaseVersion);
            var resVersion = appVersion.Patch;

            string commandLineArgs = $"{pythonScripPath} {configRepoPath} {platformName} {uploadFilesPattern} {uploadFolder} {remoteDir} {appVersion.GetVersionString()} {resVersion}";
            
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

            pStartInfo.RedirectStandardInput = true;
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
            proces.ErrorDataReceived += (s, e) =>
            {
                Logger.Info(e.Data);
            };
            proces.OutputDataReceived += (s, e) =>
            {
                Logger.Debug(e.Data);
            };
            proces.BeginOutputReadLine();
            proces.BeginErrorReadLine();
            proces.WaitForExit();
            var exitCode = proces.ExitCode;
            if (exitCode != 0)
            {
                AppBuildContext.AppendErrorLog($"Exit code : {proces.ExitCode}!");
                Logger.Error($"Exit code : {proces.ExitCode}!");
            }
            else
            {
                Logger.Debug("Upload files successful!");
            }
            proces.Close();

            AssetDatabase.Refresh();

            return exitCode == 0;
        }


        #endregion

    }
}
