using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using TechCosmos.Metrology;

namespace TechCosmos.Metrology.Tests
{
    public class MeasureScaleTests
    {
        [Test]
        public void ToDefined_ScalesEditorLength()
        {
            Assert.AreEqual(250f, MeasureScale.ToDefined(2.5f, 1f, 100f), 0.001f);
        }

        [Test]
        public void ToEditor_ScalesDefinedValue()
        {
            Assert.AreEqual(8f, MeasureScale.ToEditor(800f, 1f, 100f), 0.001f);
        }

        [Test]
        public void RoundTrip_ReturnsOriginal()
        {
            float editor = MeasureScale.ToEditor(800f, 3f, 7f);
            Assert.AreEqual(800f, MeasureScale.ToDefined(editor, 3f, 7f), 0.001f);
        }

        [Test]
        public void NegativeLength_StaysNegative()
        {
            Assert.AreEqual(-250f, MeasureScale.ToDefined(-2.5f, 1f, 100f), 0.001f);
        }

        [Test]
        public void NonPositiveScale_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => MeasureScale.ToDefined(1f, 0f, 1f));
            Assert.Throws<InvalidOperationException>(() => MeasureScale.ToEditor(1f, 1f, -2f));
            Assert.Throws<InvalidOperationException>(() => MeasureScale.ToDefined(1f, float.PositiveInfinity, 1f));
        }

        [Test]
        public void UnitAsset_UsesItsScale()
        {
            var unit = ScriptableObject.CreateInstance<MeasureUnitSO>();
            try
            {
                unit.Initialize("码", 1f, 100f);
                Assert.AreEqual("码", unit.UnitId);
                Assert.AreEqual(8f, unit.ToEditor(800f), 0.001f);
                Assert.AreEqual(250f, unit.ToDefined(2.5f), 0.001f);
            }
            finally
            {
                Object.DestroyImmediate(unit);
            }
        }

        [Test]
        public void UnitId_AcceptsLetters_AndRejectsKeywords()
        {
            Assert.IsTrue(MeasureUnitId.IsValid("码"));
            Assert.IsTrue(MeasureUnitId.IsValid("BodySize"));
            Assert.IsFalse(MeasureUnitId.IsValid("1码"));
            Assert.IsFalse(MeasureUnitId.IsValid("class"));
            Assert.IsFalse(MeasureUnitId.IsValid(""));
            Assert.IsFalse(MeasureUnitId.IsValid("_"));
        }

        [Test]
        public void BuildMap_DropsDuplicateIds_AndKeepsTheOthers()
        {
            var first = ScriptableObject.CreateInstance<MeasureUnitSO>();
            var second = ScriptableObject.CreateInstance<MeasureUnitSO>();
            var other = ScriptableObject.CreateInstance<MeasureUnitSO>();
            try
            {
                first.Initialize("码", 1f, 100f);
                second.Initialize("码", 1f, 50f);
                other.Initialize("体型", 1f, 2f);
                var map = MeasureUnits.BuildMap(new List<MeasureUnitSO> { first, second, other }, out var duplicates);
                Assert.IsTrue(duplicates.Contains("码"));
                Assert.IsFalse(map.ContainsKey("码"));
                Assert.IsTrue(map.ContainsKey("体型"));
            }
            finally
            {
                Object.DestroyImmediate(first);
                Object.DestroyImmediate(second);
                Object.DestroyImmediate(other);
            }
        }

        [Test]
        public void BuildMap_SkipsNullAndEmpty()
        {
            var empty = ScriptableObject.CreateInstance<MeasureUnitSO>();
            var unit = ScriptableObject.CreateInstance<MeasureUnitSO>();
            try
            {
                unit.Initialize("体型", 1f, 2f);
                var map = MeasureUnits.BuildMap(new List<MeasureUnitSO> { null, empty, unit }, out _);
                Assert.AreEqual(1, map.Count);
                Assert.IsTrue(map.ContainsKey("体型"));
            }
            finally
            {
                Object.DestroyImmediate(empty);
                Object.DestroyImmediate(unit);
            }
        }
    }
}
