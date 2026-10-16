using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TechCosmos.Metrology;

namespace TechCosmos.Metrology.Editor
{
    static class MeasureCatalogSync
    {
        public static List<MeasureUnitSO> Scan()
        {
            var units = new List<MeasureUnitSO>();
            foreach (var guid in AssetDatabase.FindAssets("t:MeasureUnitSO"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var unit = AssetDatabase.LoadAssetAtPath<MeasureUnitSO>(path);
                if (unit != null)
                    units.Add(unit);
            }

            units.Sort((a, b) =>
            {
                var byId = string.Compare(a.UnitId, b.UnitId, StringComparison.Ordinal);
                if (byId != 0)
                    return byId;
                return string.Compare(AssetDatabase.GetAssetPath(a), AssetDatabase.GetAssetPath(b), StringComparison.Ordinal);
            });
            return units;
        }

        public static MeasureCatalog Load()
        {
            return AssetDatabase.LoadAssetAtPath<MeasureCatalog>(MeasurePaths.CatalogAssetPath);
        }

        public static bool Matches(MeasureCatalog catalog, IReadOnlyList<MeasureUnitSO> units)
        {
            if (catalog == null)
                return units == null || units.Count == 0;
            var recorded = catalog.Units;
            if (recorded == null)
                return units.Count == 0;
            if (recorded.Count != units.Count)
                return false;
            for (int i = 0; i < units.Count; i++)
            {
                if (recorded[i] != units[i])
                    return false;
            }
            return true;
        }

        public static void Rewrite(IReadOnlyList<MeasureUnitSO> units)
        {
            MeasureFolders.Ensure("Assets/Resources");
            MeasureFolders.Ensure("Assets/Resources/TechCosmos.Metrology");

            var catalog = Load();
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<MeasureCatalog>();
                AssetDatabase.CreateAsset(catalog, MeasurePaths.CatalogAssetPath);
            }

            var serialized = new SerializedObject(catalog);
            var list = serialized.FindProperty("units");
            list.arraySize = units.Count;
            for (int i = 0; i < units.Count; i++)
                list.GetArrayElementAtIndex(i).objectReferenceValue = units[i];

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            MeasureUnits.ClearCache();
        }
    }
}
