/***************************************************************

 *  类名称：          RoutedEventArgs

 *  描述：

 *  作者：            Chico(wuyuanbing)

 *  创建时间：        2020/4/20 17:32:37

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

namespace CenturyGame.Core.FSM
{

    public interface IRoutedEventArgs
    {
        int EventType { set; get; }
    }

    public struct RoutedEventArgs : IRoutedEventArgs
    {
        public int EventType { set; get; }
    }

    public struct RoutedEventArgs<T> : IRoutedEventArgs
    {
        public int EventType { set; get; }

        public T arg { set; get; }
    }
}
