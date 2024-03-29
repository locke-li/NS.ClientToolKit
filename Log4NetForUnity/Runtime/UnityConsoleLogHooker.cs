﻿using UnityEngine;
using System;
using System.Text;
using CenturyGame.LoggerModule.Runtime;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;

namespace CenturyGame.Log4NetForUnity.Runtime
{
    public class UnityConsoleLogHooker
    {
        private static readonly Lazy<ILogger> s_mExceptionLogger = new Lazy<ILogger>(() =>
            LoggerManager.GetLogger("Exception"));
        private static readonly Lazy<ILogger> s_mAssertLogger = new Lazy<ILogger>(() =>
            LoggerManager.GetLogger("Assert"));

        private static StringBuilder sb = new StringBuilder();

        private static void LogHandler(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Assert)
            {
                sb.Length = 0;
                sb.Append(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]"));
                sb.Append(condition);
                sb.Append(stackTrace);
                s_mAssertLogger.Value.Error(sb.ToString());
            }
            else if (type == LogType.Exception)
            {
                sb.Length = 0;
                sb.Append(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]"));
                sb.AppendLine(condition);
                sb.Append(stackTrace);
                s_mExceptionLogger.Value.Error(sb.ToString());
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Application.logMessageReceived += LogHandler;
        }
    }
}