using UnityEditor;
using TechCosmos.Metrology;

namespace TechCosmos.Metrology.Editor
{
    [CustomEditor(typeof(MeasureUnitSO))]
    sealed class MeasureUnitSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var unit = (MeasureUnitSO)target;
            EditorGUILayout.LabelField("单位", string.IsNullOrEmpty(unit.UnitId) ? "（未命名）" : unit.UnitId);
            EditorGUILayout.HelpBox("比例在 Tech-Cosmos → 度量衡 里改。单位名和生成的类对应，创建后不要改。", MessageType.None);
            MeasureUnitGUI.DrawScale(serializedObject);
        }
    }
}
