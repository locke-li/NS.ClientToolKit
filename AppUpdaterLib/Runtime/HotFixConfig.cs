using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public sealed class HotFixConfig
{
    public string FileSite = null;
    public string UpdatePath = null;
    public string AppVerison = null;
    public string MinVer = null;
}