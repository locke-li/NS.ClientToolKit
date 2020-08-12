/***************************************************************

 *  类名称：        NamespaceTypesNodeDisplayNodeEditor

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/9 16:15:47

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using UnityEditor;
using XNodeEditor;

[NodeEditor.CustomNodeEditorAttribute(typeof(NamespaceTypesNodeDisplayNode))]
class NamespaceTypesNodeDisplayNodeEditor : NodeEditor
{

    public override void OnBodyGUI()
    {
        base.OnBodyGUI();

        var node = target as NamespaceTypesNodeDisplayNode;

        var fullNames = node.GetValue();
        if (fullNames != null && fullNames.Length > 0)
        {

            for (int i = 0; i < fullNames.Length; i++)
            {
                var fullName = fullNames[i];
                var typeName = fullName.Substring(fullName.LastIndexOf('.') + 1);


                EditorGUILayout.LabelField(typeName);
            }

        }
    }
}
