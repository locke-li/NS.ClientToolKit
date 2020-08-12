/***************************************************************

 *  类名称：        ILoggerProvider

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/2 20:58:50

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


namespace CenturyGame.LoggerModule.Runtime
{
    public interface ILoggerProvider
    {
        /// <summary>
        /// Logger名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 根据名字获取Logger
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ILogger GetLogger(string name);

        /// <summary>
        /// 终止Logger服务
        /// </summary>
        void Shutdown();
    }
}
