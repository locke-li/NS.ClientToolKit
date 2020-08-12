/***************************************************************

 *  类名称：        ILuaTable

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/1 11:05:54

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections;

namespace CenturyGame.LuaModule.Runtime.Interfaces
{
    public interface ILuaTable : IDisposable
    {
        #region Get
        ILuaTable GetTable(string key);
        void GetTable(string key, out ILuaTable value);

        ILuaFunction GetFunction(string key);
        void GetFunction(string key, out ILuaFunction value);

        T GetVal<T>(string key);
        void GetVal<T>(string key,out T value);

        IEnumerable GetAllKeys();

        #endregion


        #region Set

        void SetVal<T>(string key , T value);

        void SetMetaTable(ILuaTable metaTable);

        #endregion


    }
}
