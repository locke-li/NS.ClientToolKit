/***************************************************************

 *  类名称：        AppBuilderLogger

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/11 16:17:25

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/


using System;
using CenturyGame.LoggerModule.Runtime;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace CenturyGame.AppBuilder.Editor.Builds.InnerLoggers
{
    public sealed class AppBuilderLogger : ILogger
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

        #endregion

        public void Debug(object message)
        {
            UnityDebug.Log(message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            UnityDebug.LogFormat(format,args);
        }

        public void Info(object message)
        {
            UnityDebug.Log(message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            UnityDebug.LogFormat(format,args);
        }

        public void Warn(object message)
        {
            UnityDebug.LogWarning(message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            UnityDebug.LogWarningFormat(format,args);
        }

        public void Error(object message)
        {
            UnityDebug.LogError(message);
        }

        public void Error(object message, Exception exception)
        {
            UnityDebug.LogError(message);
            UnityDebug.LogException(exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            UnityDebug.LogErrorFormat(format,args);
        }

        public void Fatal(object message)
        {
            UnityDebug.LogError(message);
        }

        public void Fatal(object message, Exception exception)
        {
            UnityDebug.LogError(message);
            UnityDebug.LogException(exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            UnityDebug.LogErrorFormat(format,args);
        }
    }
}
