/***************************************************************

 *  类名称：        ILogger

 *  描述：			基础日志输出接口

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/2 20:46:45

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;

namespace CenturyGame.LoggerModule.Runtime
{
    public interface ILogger
    {
        #region Debug

        void Debug(object message);

        void DebugFormat(string format, params object[] args);

        #endregion


        #region Info

        void Info(object message);

        void InfoFormat(string format, params object[] args);

        #endregion

        #region Warn

        void Warn(object message);

        void WarnFormat(string format, params object[] args);

        #endregion


        #region Error

        void Error(object message);

        void Error(object message, Exception exception);

        void ErrorFormat(string format, params object[] args);

        #endregion


        #region Fatal

        void Fatal(object message);

        void Fatal(object message, Exception exception);

        void FatalFormat(string format, params object[] args);


        #endregion
    }
}
