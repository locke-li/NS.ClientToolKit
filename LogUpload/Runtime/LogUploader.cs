using UnityEngine;
using System;
using System.Net.Http;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using CenturyGame.LoggerModule.Runtime;
using CenturyGame.Log4NetForUnity.Runtime;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;

namespace CenturyGame.LogUpload.Runtime
{
    public static class LogUploader
    {
        private static readonly Lazy<ILogger> s_mLogger = new Lazy<ILogger>(() =>
            LoggerManager.GetLogger("LogUploader"));

        /// <summary>
        /// 是否有需要提交的日志文件
        /// </summary>
        /// <returns></returns>
        public static bool CheckNeedUpload()
        {
            string LogPath = GetGameLogPath();
            var logFiles = Directory.EnumerateFiles(LogPath, "*.log");
            var e = logFiles.GetEnumerator();
            while (e.MoveNext())
            {
                string logFileName = Path.GetFileName(e.Current);
                if (!logFileName.Equals(UnityDefaultLogFileAppender.CurrentLogName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试自动提交
        /// </summary>
        /// <param name="url"></param>
        public static void TryAutoUpload(string url)
        {
            if (CheckNeedUpload())
            {
                ZipLogFile(out string zipFilePath, out long zipFileSize);
                ManualUpload(url, zipFilePath);
            }
        }

        /// <summary>
        /// 压缩日志文件
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <param name="zipFileSize"></param>
        public static void ZipLogFile(out string zipFilePath, out long zipFileSize)
        {
            try
            {
                string LogPath = GetGameLogPath();
                string UploadPath = GetUploadPath();
                if (!Directory.Exists(UploadPath))
                    Directory.CreateDirectory(UploadPath);
                string zipName = $"{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
                string zipPath = Path.Combine(UploadPath, zipName);
                var logFiles = Directory.EnumerateFiles(LogPath, "*.log");
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipPath)))
                {
                    s.SetLevel(9);
                    byte[] buffer = new byte[4096];
                    var e = logFiles.GetEnumerator();
                    while (e.MoveNext())
                    {
                        string logFileName = Path.GetFileName(e.Current);
                        if (logFileName.Equals(UnityDefaultLogFileAppender.CurrentLogName))
                            continue;
                        var entry = new ZipEntry(logFileName)
                        {
                            DateTime = DateTime.Now
                        };
                        s.PutNextEntry(entry);
                        using (FileStream fileStream = File.OpenRead(e.Current))
                        {
                            int sourceByte;
                            do
                            {
                                sourceByte = fileStream.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceByte);
                            } while (sourceByte > 0);
                        }
                    }
                    s.Finish();
                    s.Close();                 
                }
                zipFilePath = zipPath;
                FileInfo fileInfo = new FileInfo(zipFilePath);
                zipFileSize = fileInfo.Length;
            }
            catch (Exception e)
            {
                s_mLogger.Value.Error($"Exception during ZipLogFile: {e.Message}");
                zipFilePath = string.Empty;
                zipFileSize = 0;
            }
        }

        private static void ClearZips()
        {
            string UploadPath = GetUploadPath();
            var zipFiles = Directory.EnumerateFiles(UploadPath, "*.zip");
            var e = zipFiles.GetEnumerator();
            while (e.MoveNext())
            {
                File.Delete(e.Current);
            }
        }

        private static void ClearLogs()
        {
            string LogPath = GetGameLogPath();
            var logFiles = Directory.EnumerateFiles(LogPath, "*.log");
            var e = logFiles.GetEnumerator();
            while (e.MoveNext())
            {
                File.Delete(e.Current);
            }
        }

        private static string s_UploadPath = string.Empty;
        private static string GetUploadPath()
        {
            if (string.IsNullOrEmpty(s_UploadPath))
            {
                s_UploadPath = Path.Combine(Application.persistentDataPath, "UploadLogs");
                s_UploadPath.Replace("\\", "/");
            }
            return s_UploadPath;
        }

        private static string s_GameLogPath = string.Empty;
        private static string GetGameLogPath()
        {
            if (string.IsNullOrEmpty(s_GameLogPath))
            {
                s_GameLogPath = Path.Combine(Application.persistentDataPath, "GameLogs");
                s_GameLogPath.Replace("\\", "/");
            }
            return s_GameLogPath;
        }

        public static void ManualUpload(string url, string path)
        {
            AsyncHttpUpload(url, path);
        }

        /// <summary>
        /// 异步HttpPut提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        private async static void AsyncHttpUpload(string url, string path)
        {
            byte[] buffer = new byte[0];
            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                buffer = new byte[fileStream.Length];
                await fileStream.ReadAsync(buffer, 0, (int)fileStream.Length);
            }
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = new ByteArrayContent(buffer)
                };
                int timeOut = 5 * 60 * 1000;
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeOut);
                HttpResponseMessage rsp = await httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token);
                if (rsp.IsSuccessStatusCode)
                {
                    s_mLogger.Value.Debug($"Upload Success:{path}");
                    ClearLogs();
                }
                else
                {
                    s_mLogger.Value.Debug($"Upload Failed:{path}");
                }
                ClearZips();
            }
        }
    }
}
