using UnityEditor;
using UnityEngine;
using TechCosmos.Metrology;

namespace TechCosmos.Metrology.Editor
{
    sealed class MeasureUnitCreateWindow : EditorWindow
    {
        string _unitId = "";
        float _editorUnits = 1f;
        float _definedUnits = 1f;
        string _error;

        public static void Open()
        {
            var window = CreateInstance<MeasureUnitCreateWindow>();
            window.titleContent = new GUIContent("新建单位");
            window.minSize = new Vector2(360f, 210f);
            window.maxSize = new Vector2(520f, 260f);
            window.ShowUtility();
        }

        void OnGUI()
        {
            EditorGUILayout.Space(8f);
            _unitId = EditorGUILayout.TextField("单位名", _unitId);
            _editorUnits = EditorGUILayout.FloatField("编辑器长度", _editorUnits);
            _definedUnits = EditorGUILayout.FloatField("定义单位", _definedUnits);

            var id = _unitId == null ? "" : _unitId.Trim();
            if (MeasureUnitId.IsValid(id) && _editorUnits > 0f && _definedUnits > 0f)
                EditorGUILayout.LabelField($"{Trim(_editorUnits)} 编辑器长度 = {Trim(_definedUnits)} {id}");

            if (!string.IsNullOrEmpty(_error))
                EditorGUILayout.HelpBox(_error, MessageType.Error);

            EditorGUILayout.Space(8f);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("创建"))
                    Create();
                if (GUILayout.Button("取消"))
                    Close();
            }
        }

        void Create()
        {
            var id = _unitId == null ? "" : _unitId.Trim();
            if (!MeasureUnitId.TryValidate(id, out _error))
                return;
            if (_editorUnits <= 0f || _definedUnits <= 0f || float.IsInfinity(_editorUnits) || float.IsInfinity(_definedUnits))
            {
                _error = "编辑器长度和定义单位都必须大于 0。";
                return;
            }

            try
            {
                MeasureUnitGenerator.Create(id, _editorUnits, _definedUnits);
            }
            catch (System.Exception exception)
            {
                _error = exception.Message;
                return;
            }

            MetrologyWindow.NotifyChanged();
            Close();
        }

        static string Trim(float value) => value.ToString("0.###");
    }
}
