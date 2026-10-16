using UnityEditor;
using TechCosmos.Metrology;

namespace TechCosmos.Metrology.Editor
{
    [CustomEditor(typeof(MeasureCatalog))]
    sealed class MeasureCatalogEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("这份目录由度量衡主控面板写入。运行时靠它找到全部单位。", MessageType.Info);
            var catalog = (MeasureCatalog)target;
            var units = catalog.Units;
            if (units == null || units.Count == 0)
            {
                EditorGUILayout.LabelField("还没有单位。");
                return;
            }

            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (unit == null)
                    EditorGUILayout.LabelField("丢失引用");
                else
                    EditorGUILayout.LabelField(unit.UnitId, AssetDatabase.GetAssetPath(unit));
            }
        }
    }
}
