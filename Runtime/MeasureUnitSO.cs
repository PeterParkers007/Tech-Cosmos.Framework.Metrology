using System;
using UnityEngine;

namespace TechCosmos.Metrology
{
    /// <summary>
    /// 一种单位的全局比例。编辑器长度和定义单位两边都要大于 0。
    /// 改数改这份资源，不改生成出来的类。
    /// </summary>
    public sealed class MeasureUnitSO : ScriptableObject
    {
        [SerializeField] string unitId;
        [SerializeField] float editorUnits = 1f;
        [SerializeField] float definedUnits = 1f;

        public string UnitId => unitId;
        public float EditorUnits => editorUnits;
        public float DefinedUnits => definedUnits;

        public float ToDefined(float editorLength)
        {
            try
            {
                return MeasureScale.ToDefined(editorLength, editorUnits, definedUnits);
            }
            catch (InvalidOperationException)
            {
                throw InvalidScale();
            }
        }

        public float ToEditor(float definedValue)
        {
            try
            {
                return MeasureScale.ToEditor(definedValue, editorUnits, definedUnits);
            }
            catch (InvalidOperationException)
            {
                throw InvalidScale();
            }
        }

        internal void Initialize(string id, float editor, float defined)
        {
            MeasureUnitId.EnsureValid(id);
            MeasureScale.EnsurePositive(editor, defined);
            unitId = id;
            editorUnits = editor;
            definedUnits = defined;
        }

        InvalidOperationException InvalidScale()
        {
            var label = string.IsNullOrEmpty(unitId) ? name : unitId;
            return new InvalidOperationException($"度量单位「{label}」的比例无效。编辑器长度和定义单位都必须大于 0。");
        }
    }
}
