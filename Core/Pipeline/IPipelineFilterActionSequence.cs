/***************************************************************

 *  类名称：        IPipelineFilterActionSequence

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/24 18:39:10

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

****pu***********************************************************/


using System.Collections.Generic;

namespace CenturyGame.Core.Pipeline
{
    public interface IPipelineFilterActionSequence : IPipelineFilterAction
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        Queue<IPipelineFilterAction> Actions { get; }

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        void Enqueue(IPipelineFilterAction action);


        #endregion
    }
}
