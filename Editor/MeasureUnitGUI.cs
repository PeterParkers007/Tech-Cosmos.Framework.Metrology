using UnityEditor;
using UnityEngine;
using TechCosmos.Metrology;

namespace TechCosmos.Metrology.Editor
{
    static class MeasureUnitGUI
    {
        static readonly GUIContent EditorLabel = new GUIContent("编辑器长度", "这一侧是引擎里的长度。");
        static readonly GUIContent DefinedLabel = new GUIContent("定义单位", "这一侧是这种单位下的长度。两边合起来是一条全局比例。");

        public static void DrawScale(SerializedObject serialized)
        {
            serialized.Update();
            EditorGUILayout.PropertyField(serialized.FindProperty("editorUnits"), EditorLabel);
            EditorGUILayout.PropertyField(serialized.FindProperty("definedUnits"), DefinedLabel);
            serialized.ApplyModifiedProperties();

            var unit = (MeasureUnitSO)serialized.targetObject;
            if (unit.EditorUnits > 0f && unit.DefinedUnits > 0f && !float.IsInfinity(unit.EditorUnits) && !float.IsInfinity(unit.DefinedUnits))
            {
                EditorGUILayout.LabelField($"{Trim(unit.EditorUnits)} 编辑器长度 = {Trim(unit.DefinedUnits)} {unit.UnitId}");
                return;
            }

            EditorGUILayout.HelpBox("编辑器长度和定义单位都必须大于 0。", MessageType.Error);
        }

        static string Trim(float value) => value.ToString("0.###");
    }
}
