/***************************************************************

 *  类名称：        ILuaEnvironment

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/1 11:07:52

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;

namespace CenturyGame.LuaModule.Runtime.Interfaces
{ 
    public interface ILuaEnvironment : IDisposable
    {
       ILuaTable Global { get; }

       object[] DoString(byte[] chunk, string chunkName = "chunk");

       object[] DoString(string chunk, string chunkName = "chunk");

       void Tick();

       void GC();

       ILuaTable NewTable();
    }
}
