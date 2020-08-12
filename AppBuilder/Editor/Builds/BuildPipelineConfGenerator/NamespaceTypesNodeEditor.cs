/***************************************************************

 *  类名称：        NamespaceTypesNodeEditor

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/9 15:59:04

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using Boo.Lang;
using CenturyGame.AppBuilder.Editor.Builds.BuildPipelineConfGenerator;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(NamespaceTypesNode))]
class NamespaceTypesNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        base.OnBodyGUI();

        var node = target as NamespaceTypesNode;

        if (GUILayout.Button("Refresh"))
        {
            var assembly = this.GetType().Assembly;
            List<string> fullNames = new List<string>();

            var baseType = BuildNodeBaseTypeDic.Data[node.nodeType];
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(baseType))
                {
                    fullNames.Add(type.FullName);
                }
            }
            node.outPut.typeNames = fullNames.ToArray();
        }
    }

    public override int GetWidth()
    {
        
        return base.GetWidth();
    }

    
}
