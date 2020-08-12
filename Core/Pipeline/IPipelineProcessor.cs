/***************************************************************

 *  类名称：        IPipelineProcessor

 *  描述：		    管线处理器

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/24 18:10:16

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

namespace CenturyGame.Core.Pipeline
{
    public interface IPipelineProcessor
    {
        IPipelineContext Context { get; }

        IPipelineProcessor Register(IFilter filter);

        bool TestAll(IPipelineInput input);

        ProcessResult Process(IPipelineInput input);
    }
}
