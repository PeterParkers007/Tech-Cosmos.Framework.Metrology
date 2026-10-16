using System;
using System.Collections.Generic;

namespace TechCosmos.Metrology
{
    /// <summary>单位名会变成生成类的类名，也是运行时查找这份比例的键。</summary>
    public static class MeasureUnitId
    {
        static readonly HashSet<string> Keywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
            "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
            "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
            "void", "volatile", "while",
            "record", "file", "required", "init", "scoped", "and", "or", "not", "unmanaged",
            "when", "where", "yield", "alias", "nameof", "partial", "async", "await", "nint", "nuint"
        };

        static readonly HashSet<string> ReservedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        public static bool IsValid(string unitId) => TryValidate(unitId, out _);

        public static bool TryValidate(string unitId, out string error)
        {
            if (string.IsNullOrWhiteSpace(unitId))
            {
                error = "单位名不能为空。";
                return false;
            }

            if (!IsIdentifier(unitId))
            {
                error = "单位名要能作为类名：以字母或下划线开头，后面只能是字母、数字或下划线。";
                return false;
            }

            if (unitId == "_")
            {
                error = "单位名不能是单个下划线。";
                return false;
            }

            if (Keywords.Contains(unitId))
            {
                error = "单位名和 C# 关键字冲突。";
                return false;
            }

            if (ReservedFileNames.Contains(unitId))
            {
                error = "单位名和系统保留文件名冲突。";
                return false;
            }

            error = null;
            return true;
        }

        public static void EnsureValid(string unitId)
        {
            if (!TryValidate(unitId, out var error))
                throw new ArgumentException(error, nameof(unitId));
        }

        static bool IsIdentifier(string value)
        {
            if (!IsStart(value[0]))
                return false;
            for (int i = 1; i < value.Length; i++)
            {
                if (!IsPart(value[i]))
                    return false;
            }
            return true;
        }

        static bool IsStart(char c) => c == '_' || char.IsLetter(c);

        static bool IsPart(char c) => c == '_' || char.IsLetterOrDigit(c);
    }
}
