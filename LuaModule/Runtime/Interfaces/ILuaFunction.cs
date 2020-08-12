using System;

namespace CenturyGame.LuaModule.Runtime.Interfaces
{
    public interface ILuaFunction : IDisposable
    {
        object[] Call(params object[] args);

        object[] InstCall(params object[] args);

    }
}
