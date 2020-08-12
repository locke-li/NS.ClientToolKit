using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System;

public partial class ABMenu
{
    private static readonly string LuaFolder = "Assets/LuaScripts";
    private static readonly string LuaExt = "*.lua";
    private static readonly string LuaToBytes = ".bytes";
    public static List<FileInfo> CopyLua()
    {
        List<FileInfo> luaList = new List<FileInfo>();
        DirectoryInfo luaFolder = new DirectoryInfo(string.Concat(System.Environment.CurrentDirectory, Path.DirectorySeparatorChar, LuaFolder));
        if(luaFolder.Exists)
        {
            FileInfo[] luaFile = luaFolder.GetFiles(LuaExt, SearchOption.AllDirectories);
            ABBuildCache.Open("LuaCache.x");
            for(int i = 0; i < luaFile.Length; i++)
            {
                FileInfo tmpFile = luaFile[i];
                int index = tmpFile.FullName.LastIndexOf(".");
                string sha1 = null;
                if (index > -1 && ABBuildCache.ExistsBegin(tmpFile.FullName, ref sha1))
                {
                    string newName = string.Concat(tmpFile.FullName.Substring(0, index), LuaToBytes);
                    tmpFile.CopyTo(newName, true);
                    FileInfo tmp = new FileInfo(newName);
                    tmp.Attributes = FileAttributes.Normal;
                    //luaList.Add(tmp);
                }
                ABBuildCache.ExistsEnd(tmpFile.FullName, sha1);
            }
            ABBuildCache.Close();
        }
        return luaList;
    }
}
