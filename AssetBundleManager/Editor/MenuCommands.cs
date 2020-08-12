using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CenturyGame.AssetBundleManager.Runtime;

#if UNITY_EDITOR
public class MenuCommands
{
    const string kSimulationMode = "CenturyGame/AssetBundleManager/Simulation Mode";

    [MenuItem(kSimulationMode)]
    public static void ToggleSimulationMode()
    {
        AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
    }

    [MenuItem(kSimulationMode, true)]
    public static bool ToggleSimulationModeValidate()
    {
        Menu.SetChecked(kSimulationMode, AssetBundleManager.SimulateAssetBundleInEditor);
        return true;
    }
}
#endif

