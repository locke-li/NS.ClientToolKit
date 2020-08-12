/***************************************************************

 *  类名称：          IFSMOwner

 *  描述：

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 17:50:27

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

namespace CenturyGame.Core.FSM
{

    public interface IFSMOwner { }

    public interface IFSMOwner<T> where T : IFSMOwner, new()
    {

        StateMachine<T> FSM { get; }

        void InitializeFSM();

        void Update();

        bool HandleMessage(in IRoutedEventArgs msg);
    }
}
