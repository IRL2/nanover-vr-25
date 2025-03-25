using System.Collections.Generic;

namespace Nanover.Core.Science
{
    /// <summary>
    /// Van der Waals radii of the atomic elements
    /// </summary>
    /// <remarks>
    /// The value of the Van der Waals radius is from three sources
    /// [1] Bondi, A. (1964). "Van der Waals Volumes and Radii".  J. Phys. Chem. 68
    /// (3): 441-451. doi:10.1021/j100785a001.
    /// [2] Rowland and Taylor (1996). "Intermolecular Nonbonded Contact Distances in
    /// Organic Crystal Structures: Comparison with Distances Expected from van der
    /// Waals Radii". J. Phys. Chem., 1996, 100 (18), 7384.7391. doi:10.1021/jp953141+.
    /// [3] Mantina, et al. (2009). "Consistent van der Waals Radii for the Whole Main
    /// Group". J. Phys. Chem. A, 2009, 113 (19), 5806-5812. doi:10.1021/jp8111556.
    /// Some elements do not have a Van der Waals radii provided, as there is
    /// insufficient literature to provide a value.
    /// </remarks>
    public static class ElementVdwRadii
    {
        private static readonly Dictionary<Element, float> Radii =
            new Dictionary<Element, float>
            {
                { Element.Hydrogen, 0.110f },
                { Element.Helium, 0.140f },
                { Element.Lithium, 0.182f },
                { Element.Beryllium, 0.153f },
                { Element.Boron, 0.192f },
                { Element.Carbon, 0.170f },
                { Element.Nitrogen, 0.155f },
                { Element.Oxygen, 0.152f },
                { Element.Fluorine, 0.147f },
                { Element.Neon, 0.154f },
                { Element.Sodium, 0.227f },
                { Element.Magnesium, 0.173f },
                { Element.Aluminum, 0.184f },
                { Element.Silicon, 0.210f },
                { Element.Phosphorus, 0.180f },
                { Element.Sulfur, 0.180f },
                { Element.Chlorine, 0.175f },
                { Element.Argon, 0.188f },
                { Element.Potassium, 0.275f },
                { Element.Calcium, 0.231f },
                { Element.Nickel, 0.163f },
                { Element.Copper, 0.140f },
                { Element.Zinc, 0.139f },
                { Element.Gallium, 0.187f },
                { Element.Germanium, 0.211f },
                { Element.Arsenic, 0.185f },
                { Element.Selenium, 0.190f },
                { Element.Bromine, 0.185f },
                { Element.Krypton, 0.202f },
                { Element.Rubidium, 0.303f },
                { Element.Strontium, 0.249f },
                { Element.Palladium, 0.163f },
                { Element.Silver, 0.172f },
                { Element.Cadmium, 0.158f },
                { Element.Indium, 0.193f },
                { Element.Tin, 0.217f },
                { Element.Antimony, 0.206f },
                { Element.Tellurium, 0.206f },
                { Element.Iodine, 0.198f },
                { Element.Xenon, 0.216f },
                { Element.Cesium, 0.343f },
                { Element.Barium, 0.268f },
                { Element.Platinum, 0.175f },
                { Element.Gold, 0.166f },
                { Element.Mercury, 0.155f },
                { Element.Thallium, 0.196f },
                { Element.Lead, 0.202f },
                { Element.Bismuth, 0.207f },
                { Element.Polonium, 0.197f },
                { Element.Astatine, 0.202f },
                { Element.Radon, 0.220f },
                { Element.Francium, 0.348f },
                { Element.Radium, 0.283f },
                { Element.Uranium, 0.186f }
            };

        /// <summary>
        /// Get the Van der Waals radius of the element, returning null if that data does
        /// not exist.
        /// </summary>
        public static float? GetVdwRadius(this Element element)
        {
            return Radii.TryGetValue(element, out var value) ? (float?) value : null;
        }
    }
}