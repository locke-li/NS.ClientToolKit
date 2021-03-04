/***************************************************************

 *  类名称：        IFileUpdateRuleFilter

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2021/3/1 16:25:17

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace CenturyGame.AppUpdaterLib.Runtime.Interfaces
{
    public interface IResUpdateRuleFilter
    {
        bool Filter(ref string remoteName);
    }
}
