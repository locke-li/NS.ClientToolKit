/***************************************************************

 *  类名称：        ProcessResult

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/24 18:26:59

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


namespace CenturyGame.Core.Pipeline
{

    /// <summary>
    /// The processing state
    /// </summary>
    public enum ProcessState : byte
    {
        /// <summary>
        /// The being processed data was processed completely
        /// </summary>
        Completed,
        /// <summary>
        /// The processor is cancled
        /// </summary>
        Cancled,
        /// <summary>
        /// The processor is in error state
        /// </summary>
        Error
    }

    public struct ProcessResult
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        public ProcessState State { private set; get; }


        public string Message { private set; get; }

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        public static ProcessResult Create(ProcessState state)
        {
            ProcessResult result = new ProcessResult();
            result.State = state;

            return result;
        }

        public static ProcessResult Create(ProcessState state, string message)
        {
            ProcessResult result = new ProcessResult();
            result.State = state;
            result.Message = message;

            return result;
        }

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        #endregion

    }
}
