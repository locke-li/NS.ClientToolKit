/***************************************************************

 *  类名称：        UnityDefaultLogFileAppender

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/22 12:10:28

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using System.IO;
using log4net.Appender;
using log4net.Core;
using UnityEngine;

namespace CenturyGame.Log4NetForUnity.Runtime
{
    public class UnityDefaultLogFileAppender : AppenderSkeleton
    {
        private object mLockObj = new object();
        private StreamWriter mWriter;

        public static string CurrentLogName { get; private set; }

        public UnityDefaultLogFileAppender()
        {
            this.Init();
        }


        private void Init()
        {
            string logFileName = $"{System.DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.log";
            CurrentLogName = logFileName;
            string logFilePath = Path.Combine(Application.persistentDataPath, $"GameLogs/{logFileName}");
            string dirPath = Path.GetDirectoryName(logFilePath);
            dirPath = dirPath.Replace("\\", "/");
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            logFilePath = logFilePath.Replace("\\", "/");
            this.mWriter = new StreamWriter(logFilePath);
            this.mWriter.AutoFlush = true;

            this.WriteHeader();
        }

        private void WriteHeader()
        {
            this.mWriter.WriteLine($"UnityVersion : {Application.unityVersion} ," +
                                   $"app verison : {Application.version} , " +
                                   $"platform : {Application.platform} ,"+
                                   $"isdevelopbuild : {Debug.isDebugBuild} .");
        }

        protected override void OnClose()
        {
            lock (mLockObj)
            {
                this.mWriter?.Close();
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            string message;

            try
            {
                message = RenderLoggingEvent(loggingEvent);

                lock (this.mLockObj)
                {
                    mWriter.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e,null);

                return;
            }
        }
    }
}
