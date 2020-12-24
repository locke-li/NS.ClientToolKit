using CenturyGame.LuaModule.Runtime;
using CenturyGame.LuaModule.Runtime.Interfaces;

namespace CenturyGame.Framework.Lua
{
    public class LuaManager : FrameworkModule
    {
        internal ILuaPlugin luaPlugin;

        static readonly float GCInterval = 10.0f;
        float gcTime;

        internal override void Init()
        {
        }

        internal override void Update(float elapseTime, float realElapseTime)
        {
            if (luaPlugin != null)
            {
                gcTime += elapseTime;
                if (gcTime >= GCInterval)
                {
                    luaPlugin.Tick();
                    gcTime = 0;
                }
            }
        }

        internal override void LateUpdate()
        {
        }

        internal override void Shutdown()
        {
        }

        public void SetLuaPlugin(ILuaPlugin plugin)
        {
            luaPlugin = plugin;
        }
    }
}