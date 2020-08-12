using System.Collections;
using System.Collections.Generic;
using System;
using CenturyGame.AppUpdaterLib.Runtime;

[Serializable]
public sealed class LastBuildVersion
{
    public string Version = null;
    public VersionManifest Info;
}