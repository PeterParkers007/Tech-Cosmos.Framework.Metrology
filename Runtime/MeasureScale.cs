using System;

namespace TechCosmos.Metrology
{
    /// <summary>
    /// 一组比例：若干编辑器长度对应若干定义单位。
    /// </summary>
    public static class MeasureScale
    {
        public static float ToDefined(float editorLength, float editorUnits, float definedUnits)
        {
            EnsurePositive(editorUnits, definedUnits);
            return editorLength * (definedUnits / editorUnits);
        }

        public static float ToEditor(float definedValue, float editorUnits, float definedUnits)
        {
            EnsurePositive(editorUnits, definedUnits);
            return definedValue * (editorUnits / definedUnits);
        }

        internal static void EnsurePositive(float editorUnits, float definedUnits)
        {
            if (IsPositive(editorUnits) && IsPositive(definedUnits))
                return;
            throw new InvalidOperationException("编辑器长度和定义单位都必须大于 0。");
        }

        static bool IsPositive(float value) => value > 0f && !float.IsInfinity(value);
    }
}
