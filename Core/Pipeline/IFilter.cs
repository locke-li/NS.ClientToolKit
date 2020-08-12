/***************************************************************

 *  类名称：        IFiter

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/24 18:18:10

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

namespace CenturyGame.Core.Pipeline
{
    public enum FilterState : byte
    {
        /// <summary>
        /// Normal state
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Error state
        /// </summary>
        Error = 1,
        /// <summary>
        /// The filter is excute competed
        /// </summary>
        Completed = 2,
    }

    public interface IFilter
    {
        bool Enabled { get; }

        IPipelineProcessor Processor { set;get; }

        IFilter NextFilter { get; }

        FilterState State { get; }

        void OnPreProcess();

        bool Test(IPipelineInput input);

        void Execute(IPipelineInput input);

        void OnPostProcess();

        void Connect(IFilter nextFilter);
    }
}
