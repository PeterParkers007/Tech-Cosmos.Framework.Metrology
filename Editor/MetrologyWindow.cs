using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using TechCosmos.Metrology;

namespace TechCosmos.Metrology.Editor
{
    public sealed class MetrologyWindow : EditorWindow
    {
        readonly List<UnitRow> _rows = new List<UnitRow>();
        Vector2 _scroll;
        bool _catalogMatches;
        MeasureUnitSO _pendingDelete;

        [MenuItem("Tech-Cosmos/度量衡")]
        public static void Open()
        {
            var window = GetWindow<MetrologyWindow>("度量衡");
            window.minSize = new Vector2(420f, 320f);
            window.Show();
        }

        public static void NotifyChanged()
        {
            var windows = Resources.FindObjectsOfTypeAll<MetrologyWindow>();
            for (int i = 0; i < windows.Length; i++)
                windows[i].Rescan();
        }

        void OnEnable() => Rescan();

        void OnGUI()
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.HelpBox("这里改每种单位的全局比例。单位类只负责换算，数写在各自的资源上。", MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("新建单位", GUILayout.Height(24f)))
                    MeasureUnitCreateWindow.Open();
                if (GUILayout.Button("重新扫描", GUILayout.Height(24f)))
                    Rescan();
                using (new EditorGUI.DisabledScope(_catalogMatches))
                {
                    if (GUILayout.Button("写入运行时目录", GUILayout.Height(24f)))
                        WriteCatalog();
                }
            }

            if (!_catalogMatches)
                EditorGUILayout.HelpBox("运行时目录还没跟上。播放和打包前要点一次写入运行时目录。", MessageType.Warning);

            DrawDuplicateWarning();

            EditorGUILayout.Space(4f);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            if (_rows.Count == 0)
                EditorGUILayout.LabelField("还没有单位。");

            bool refresh = false;
            for (int i = 0; i < _rows.Count; i++)
            {
                if (DrawRow(_rows[i]))
                {
                    refresh = true;
                    break;
                }
            }
            EditorGUILayout.EndScrollView();
            if (refresh)
                Rescan();
            ConfirmPendingDelete();
        }

        void DrawDuplicateWarning()
        {
            var duplicates = _rows
                .Where(row => !string.IsNullOrEmpty(row.Unit.UnitId))
                .GroupBy(row => row.Unit.UnitId, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();
            if (duplicates.Length == 0)
                return;
            EditorGUILayout.HelpBox("这些单位名有多份资源，运行时无法确定用哪一份：" + string.Join("、", duplicates), MessageType.Error);
        }

        bool DrawRow(UnitRow row)
        {
            var refresh = false;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(string.IsNullOrEmpty(row.Unit.UnitId) ? "（未命名）" : row.Unit.UnitId, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(row.AssetPath, EditorStyles.miniLabel);

            if (string.IsNullOrEmpty(row.Unit.UnitId))
                EditorGUILayout.HelpBox("这份资源没有单位名，不能生成类。", MessageType.Warning);
            else if (!row.HasClassFile)
                EditorGUILayout.HelpBox("还没有对应的单位类。", MessageType.Warning);

            MeasureUnitGUI.DrawScale(row.SerializedObject);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("定位", GUILayout.Width(64f)))
                {
                    EditorGUIUtility.PingObject(row.Unit);
                    Selection.activeObject = row.Unit;
                }

                using (new EditorGUI.DisabledScope(!MeasureUnitId.IsValid(row.Unit.UnitId) || IsDuplicate(row.Unit.UnitId)))
                {
                    if (!row.HasClassFile && GUILayout.Button("生成类", GUILayout.Width(80f)))
                    {
                        MeasureUnitGenerator.WriteClass(row.Unit.UnitId);
                        refresh = true;
                    }
                }

                if (GUILayout.Button("删除", GUILayout.Width(64f)))
                    _pendingDelete = row.Unit;
            }

            EditorGUILayout.EndVertical();
            return refresh;
        }

        bool IsDuplicate(string unitId)
        {
            if (string.IsNullOrEmpty(unitId))
                return false;
            int count = 0;
            for (int i = 0; i < _rows.Count; i++)
            {
                if (_rows[i].Unit.UnitId == unitId)
                    count++;
            }
            return count > 1;
        }

        void ConfirmPendingDelete()
        {
            var unit = _pendingDelete;
            if (unit == null)
                return;
            _pendingDelete = null;

            var label = string.IsNullOrEmpty(unit.UnitId) ? AssetDatabase.GetAssetPath(unit) : unit.UnitId;
            if (!EditorUtility.DisplayDialog("删除单位", $"删除「{label}」和它的单位类？", "删除", "取消"))
                return;
            MeasureUnitGenerator.Delete(unit);
            Rescan();
        }

        void WriteCatalog()
        {
            MeasureCatalogSync.Rewrite(MeasureCatalogSync.Scan());
            Rescan();
        }

        void Rescan()
        {
            _rows.Clear();
            var units = MeasureCatalogSync.Scan();
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                _rows.Add(new UnitRow
                {
                    Unit = unit,
                    AssetPath = AssetDatabase.GetAssetPath(unit),
                    SerializedObject = new SerializedObject(unit),
                    HasClassFile = MeasureUnitGenerator.ClassExists(unit.UnitId)
                });
            }

            _catalogMatches = MeasureCatalogSync.Matches(MeasureCatalogSync.Load(), units);
            Repaint();
        }

        sealed class UnitRow
        {
            public MeasureUnitSO Unit;
            public string AssetPath;
            public SerializedObject SerializedObject;
            public bool HasClassFile;
        }
    }
}
