using System;
using System.Collections.Generic;
using UnityEngine;

namespace TechCosmos.Metrology
{
    /// <summary>按单位名找到它的全局比例。</summary>
    public static class MeasureUnits
    {
        public const string CatalogResourceName = "TechCosmos.Metrology/MeasureCatalog";

        static Dictionary<string, MeasureUnitSO> _map;
        static HashSet<string> _duplicates;

        public static MeasureUnitSO Find(string unitId)
        {
            if (string.IsNullOrEmpty(unitId))
                return null;
            EnsureLoaded();
            if (_duplicates.Contains(unitId))
                throw new InvalidOperationException($"度量单位「{unitId}」有多份资源。");
            _map.TryGetValue(unitId, out var unit);
            return unit;
        }

        public static MeasureUnitSO Require(string unitId)
        {
            var unit = Find(unitId);
            if (unit != null)
                return unit;
            throw new InvalidOperationException(
                $"找不到度量单位「{unitId}」。在 Tech-Cosmos → 度量衡 里新建它，或点写入运行时目录。");
        }

        public static void ClearCache()
        {
            _map = null;
            _duplicates = null;
        }

        internal static Dictionary<string, MeasureUnitSO> BuildMap(IEnumerable<MeasureUnitSO> units, out HashSet<string> duplicates)
        {
            var map = new Dictionary<string, MeasureUnitSO>(StringComparer.Ordinal);
            duplicates = new HashSet<string>(StringComparer.Ordinal);
            if (units == null)
                return map;

            foreach (var unit in units)
            {
                if (unit == null || string.IsNullOrEmpty(unit.UnitId))
                    continue;
                if (duplicates.Contains(unit.UnitId))
                    continue;
                if (map.ContainsKey(unit.UnitId))
                {
                    map.Remove(unit.UnitId);
                    duplicates.Add(unit.UnitId);
                    continue;
                }
                map.Add(unit.UnitId, unit);
            }

            return map;
        }

        static void EnsureLoaded()
        {
            if (_map != null)
                return;
            var catalog = Resources.Load<MeasureCatalog>(CatalogResourceName);
            _map = BuildMap(catalog != null ? catalog.Units : null, out _duplicates);
        }
    }
}
