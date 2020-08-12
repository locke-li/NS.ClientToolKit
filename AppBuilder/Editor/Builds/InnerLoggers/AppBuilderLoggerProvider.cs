/***************************************************************

 *  类名称：        AppBuilderLoggerProvider

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/11 16:25:51

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/


using CenturyGame.LoggerModule.Runtime;

namespace CenturyGame.AppBuilder.Editor.Builds.InnerLoggers
{
    class AppBuilderLoggerProvider : ILoggerProvider
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
            return new AppBuilderLogger();
        }

        public void Shutdown()
        {
            
        }

        public string Name { get; } = $"{nameof(AppBuilderLoggerProvider)}";
    }
}
