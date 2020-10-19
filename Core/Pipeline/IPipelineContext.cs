/***************************************************************

 *  类名称：        IPipelineContext

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/24 18:18:58

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


namespace CenturyGame.Core.Pipeline
{
    public interface IPipelineContext
    {
        string Error { get; }

        void AppendErrorLog(string message);
    }
}
