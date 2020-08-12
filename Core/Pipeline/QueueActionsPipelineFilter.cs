/***************************************************************

 *  类名称：        QueueActionsPipelineFilter

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/26 11:58:00

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

namespace CenturyGame.Core.Pipeline
{
    public abstract class QueueActionsPipelineFilter : BasePipelineFilter, IFilter
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        protected bool autoAddActions = false;

        protected readonly BasePipelineFilterActionSequence pSequence = null;

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        public QueueActionsPipelineFilter()
        {
            this.pSequence = new BasePipelineFilterActionSequence(() => this.Processor.Context);
            this.pAction = pSequence;

        }

        public QueueActionsPipelineFilter(bool autoAddActions) : this()
        {
            this.autoAddActions = autoAddActions;

            if (this.autoAddActions)
            {
                this.OnAutoAddActions();
            }
        }

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public virtual void OnAutoAddActions()
        {

        }

        public void Enqueue(IPipelineFilterAction action)
        {
            action.Context = this.Processor.Context;
            this.pSequence.Enqueue(action);
        }

        #endregion

    }
}
