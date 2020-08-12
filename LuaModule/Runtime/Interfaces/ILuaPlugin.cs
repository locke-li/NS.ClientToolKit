/***************************************************************

 *  类名称：        ILuaPlugin

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/1 20:51:07

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenturyGame.LuaModule.Runtime.Interfaces
{
    public interface ILuaPlugin
    {
        string Name { get; }

        void OnInit();

        void Tick();

        void OnDispose();
    }
}
