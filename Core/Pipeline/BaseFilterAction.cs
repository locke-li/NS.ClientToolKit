/***************************************************************

 *  类名称：        BaseFilterAction

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/24 20:52:37

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using CenturyGame.LoggerModule.Runtime;

namespace CenturyGame.Core.Pipeline
{
    public abstract class BaseFilterAction : IPipelineFilterAction
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        protected virtual ILogger Logger { get; }

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------
        public ActionState State { get; set; }

        public virtual IPipelineContext Context { set; get; }

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public virtual void BeforeExcute(IFilter filter, IPipelineInput input)
        {
            Logger?.Debug($"Begin excute action :{this.GetType().Name} ");
        }

        public abstract bool Test(IFilter filter, IPipelineInput input);

        public abstract void Execute(IFilter filter, IPipelineInput input);

        public virtual void EndExcute(IFilter filter, IPipelineInput input)
        {
            Logger?.Debug($"End excute action :{this.GetType().Name}");
        }

        #endregion
    }
}
