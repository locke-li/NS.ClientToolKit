/***************************************************************

 *  类名称：        IRecyclable

 *  描述：			可被回收的对象接口

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/11 10:56:15

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

namespace CenturyGame.Core.ResourcePools
{
    public interface IRecyclable
    {
        void Recycle();
    }
}
