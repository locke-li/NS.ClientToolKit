using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System;
using System.Linq;

public sealed class ABBuildCache
{
    private static string currentCacheFileName = null;
    private static Dictionary<string, string> cache = null;
    public static void Open(string fileName)
    {
        currentCacheFileName = string.Concat(System.Environment.CurrentDirectory, Path.DirectorySeparatorChar, fileName);
        cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        List<string> keys = new List<string>();
        List<string> values = new List<string>();
        if (File.Exists(fileName))
        {
            ABBuildCacheList cacheStore = JsonUtility.FromJson<ABBuildCacheList>(File.ReadAllText(fileName));
            keys.AddRange(cacheStore.Keys);
            values.AddRange(cacheStore.Values);
            for (int i = 0; i < keys.Count; i++)
            {
                cache[keys[i]] = values[i];
            }
        }
    }
    public static bool ExistsBegin(string fileName, ref string sha1)
    {
        bool needCreate = false;
        sha1 = GetHash(fileName);
        if (cache.ContainsKey(fileName))
        {
            string oldSha1 = cache[fileName];
            if (sha1 != oldSha1)
            {
                needCreate = true;
            }
        }
        else
        {
            needCreate = true;
        }
        return needCreate;
    }
    public static void ExistsEnd(string fileName, string sha1)
    {
        if (sha1 != null)
        {
            cache[fileName] = sha1;
        }
    }
    public static void Close()
    {
        if (cache != null)
        {
            ABBuildCacheList cacheStore = new ABBuildCacheList();
            cacheStore.Keys = cache.Keys.ToArray();
            cacheStore.Values = cache.Values.ToArray();
            var dat = JsonUtility.ToJson(cacheStore, true);
            File.WriteAllBytes(currentCacheFileName, System.Text.Encoding.UTF8.GetBytes(dat));
            
            cache.Clear();
            cache = null;
            currentCacheFileName = null;
        }

    }
    public static string GetHash(string path, HashType type = HashType.SHA1)
    {
        byte[] retval = null;
        FileInfo file = new FileInfo(path);
        if (file.Attributes != FileAttributes.Normal)
        {
            file.Attributes = FileAttributes.Normal;
        }
        
        using (FileStream fs = file.OpenRead())
        {
            if(type == HashType.MD5)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                retval = md5.ComputeHash(fs);
            }
            else
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                retval = sha1.ComputeHash(fs);
            }
            fs.Close();
        }
        StringBuilder sc = new StringBuilder();
        for (int i = 0; i < retval.Length; i++)
        {
            sc.Append(retval[i].ToString("x2"));
        }
        return sc.ToString();
    }
    public enum HashType
    {
        SHA1,
        MD5
    }
    [Serializable]
    public class ABBuildCacheList
    {
        public string[] Keys;
        public string[] Values;
    }
}

