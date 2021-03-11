using System.Collections.Generic;
using UnityEngine;
using CenturyGame.Framework.Base;

namespace CenturyGame.RuntimeAtlas.Runtime
{
    public class RuntimeAtlasManager : Singleton<RuntimeAtlasManager>
    {
        private Dictionary<RuntimeAtlasGroup, RuntimeAtlas> RuntimeAtlasMap = new Dictionary<RuntimeAtlasGroup, RuntimeAtlas>();

        public RuntimeAtlas GetRuntimeAtlas(RuntimeAtlasGroup group, bool topFirst)
        {
            RuntimeAtlas atlas;
            if (RuntimeAtlasMap.ContainsKey(group))
                atlas = RuntimeAtlasMap[group];
            else
            {
                atlas = new RuntimeAtlas(group, topFirst);
                RuntimeAtlasMap[group] = atlas;
            }
            return atlas;
        }
    }
}