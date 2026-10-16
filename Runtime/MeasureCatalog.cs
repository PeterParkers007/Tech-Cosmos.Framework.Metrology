using System.Collections.Generic;
using UnityEngine;

namespace TechCosmos.Metrology
{
    /// <summary>
    /// 项目里全部单位资源的入口。放在 Resources 下，运行时才能加载到。
    /// 由主控面板写入，不要手改引用。
    /// </summary>
    public sealed class MeasureCatalog : ScriptableObject
    {
        [SerializeField] List<MeasureUnitSO> units = new List<MeasureUnitSO>();

        public IReadOnlyList<MeasureUnitSO> Units => units;
    }
}
