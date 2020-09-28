/***************************************************************

 *  类名称：        LoggerManager

 *  描述：			日志实例获取管理器

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/2 20:52:53

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;

namespace CenturyGame.LoggerModule.Runtime
{
    public sealed class LoggerManager
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        private static ILoggerProvider _sMLoggerProvider = null;

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

        /// <summary>
        ///  设置当前的Logger的提供者
        /// </summary>
        /// <param name="provider"></param>
        public static void SetCurrentLoggerProvider(ILoggerProvider provider)
        {
            if (provider == null)
                throw new NullReferenceException("The current logger provider that you want to set is null!");
            _sMLoggerProvider = provider;
        }

        /// <summary>
        /// LoggerProvider是否存在
        /// </summary>
        /// <returns></returns>
        public static bool IsLoggerProviderExist()
        {
            return _sMLoggerProvider != null;
        }

        /// <summary>
        /// 获取指定名字的Logger
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILogger GetLogger(string name)
        {
            return _sMLoggerProvider?.GetLogger(name);
        }

        /// <summary>
        /// 种植Logger服务
        /// </summary>
        public static void Shutdown()
        {
            _sMLoggerProvider?.Shutdown();
        }

        #endregion
    }
}
