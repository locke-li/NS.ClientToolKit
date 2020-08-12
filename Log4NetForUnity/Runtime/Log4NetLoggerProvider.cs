/***************************************************************

 *  类名称：        Log4NetLoggerProvider

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/3 11:01:35

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.LoggerModule.Runtime;
using log4net;

namespace CenturyGame.Log4NetForUnity.Runtime
{
    public sealed class Log4NetLoggerProvider : ILoggerProvider
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

        public ILogger GetLogger(string name)
        {
            var logger = LogManager.GetLogger(name);

            return new Log4NetLoggerWrap(logger);
        }

        public void Shutdown()
        {
            LogManager.Shutdown();
        }

        public string Name => "Log4NetForUnity";
    }
}
