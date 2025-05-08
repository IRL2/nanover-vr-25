using System;
using Nanover.Core.Science;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Color
{
    /// <summary>
    /// Base code for a Visualiser node which generates colors based upon atomic
    /// elements.
    /// </summary>
    [Serializable]
    public class GoodsellColorNode : VisualiserColorNode
    {
#pragma warning disable 0649
        [SerializeField]
        private ElementArrayProperty elements = new ElementArrayProperty();

        [SerializeField]
        private IntArrayProperty particleResidues = new IntArrayProperty();

        [SerializeField]
        private IntArrayProperty residueEntities = new IntArrayProperty();

        [SerializeField]
        private StringArrayProperty residueNames = new StringArrayProperty();

        [Serializable]
        struct GoodsellChainColor
        {
            [SerializeField]
            public UnityEngine.Color NonHeteroColor;

            [SerializeField]
            public UnityEngine.Color HeteroColor;
        }

        [SerializeField]
        private UnityEngine.Color waterColor;

        [SerializeField]
        private GoodsellChainColor[] chainColors;
#pragma warning restore 0649

        protected override bool IsInputDirty => elements.IsDirty
                                             || particleResidues.IsDirty
                                             || residueEntities.IsDirty
                                             || residueNames.IsDirty;

        protected override bool IsInputValid => elements.HasNonEmptyValue()
                                             && particleResidues.HasNonEmptyValue()
                                             && residueEntities.HasNonEmptyValue()
                                             && residueNames.HasNonEmptyValue();

        protected override void UpdateOutput()
        {
            var colorArray = colors.HasNonNullValue() ? colors.Value : new UnityEngine.Color[0];
            Array.Resize(ref colorArray, elements.Value.Length);

            var elementArray = elements.Value;
            var residueArray = particleResidues.Value;
            var entityArray = residueEntities.Value;
            var nameArray = residueNames.Value;

            for (var i = 0; i < elements.Value.Length; i++)
            {
                var resid = residueArray[i];
                colorArray[i] = GetColor(elementArray[i], entityArray[resid], nameArray[resid]);
            }

            colors.Value = colorArray;
        }

        protected override void ClearOutput()
        {
            colors.UndefineValue();
        }

        protected override void ClearDirty()
        {
            elements.IsDirty = false;
            particleResidues.IsDirty = false;
            residueEntities.IsDirty = false;
            residueNames.IsDirty = false;
        }

        private UnityEngine.Color GetColor(Element element, int entityId, string resname)
        {
            if (resname == "HOH")
                return waterColor;
            var i = entityId % chainColors.Length;
            return element == Element.Carbon
                       ? chainColors[i].NonHeteroColor
                       : chainColors[i].HeteroColor;
        }
    }
}