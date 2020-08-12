/***************************************************************

 *  类名称：        WwiseResProcess

 *  描述：		    音频数据处理相关

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/26 20:15:21

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.Core.Pipeline;

namespace CenturyGame.AppBuilder.Editor.Builds.Actions.ResProcess
{
    public class WwiseResProcessAction : BaseBuildFilterAction
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

        public override bool Test(IFilter filter, IPipelineInput input)
        {
            return true;
        }

        public override void Execute(IFilter filter, IPipelineInput input)
        {
            throw new System.NotImplementedException();
        }
    }
}
