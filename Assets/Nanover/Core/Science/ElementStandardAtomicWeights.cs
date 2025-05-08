using System.Collections.Generic;

namespace Nanover.Core.Science
{
    /// <summary>
    /// Standard atomic weights of the atomic elements.
    /// </summary>
    /// <remarks>
    /// The standard atomic weights provided are the CIAAW standard, originally
    /// presented in 2013 and revised in 2015 and 2017.
    /// </remarks>
    public static class ElementStandardAtomicWeights
    {
        private static readonly Dictionary<Element, float> Weights =
            new Dictionary<Element, float>
            {
                { Element.Hydrogen, 1.008f },
                { Element.Helium, 4.003f },
                { Element.Lithium, 6.970f },
                { Element.Beryllium, 9.012f },
                { Element.Boron, 10.82f },
                { Element.Carbon, 12.01f },
                { Element.Nitrogen, 14.01f },
                { Element.Oxygen, 16.00f },
                { Element.Neon, 20.18f },
                { Element.Sodium, 22.99f },
                { Element.Magnesium, 24.30f },
                { Element.Aluminum, 26.98f },
                { Element.Silicon, 28.09f },
                { Element.Phosphorus, 30.97f },
                { Element.Sulfur, 32.07f },
                { Element.Chlorine, 35.46f },
                { Element.Argon, 39.85f },
                { Element.Potassium, 39.10f },
                { Element.Calcium, 40.08f },
                { Element.Scandium, 44.96f },
                { Element.Titanium, 47.87f },
                { Element.Vanadium, 50.94f },
                { Element.Chromium, 52.00f },
                { Element.Manganese, 54.94f },
                { Element.Iron, 55.85f },
                { Element.Cobalt, 58.93f },
                { Element.Nickel, 58.69f },
                { Element.Copper, 63.55f },
                { Element.Zinc, 65.38f },
                { Element.Gallium, 69.72f },
                { Element.Germanium, 72.63f },
                { Element.Arsenic, 74.92f },
                { Element.Selenium, 78.97f },
                { Element.Bromine, 79.91f },
                { Element.Krypton, 83.80f },
                { Element.Rubidium, 85.47f },
                { Element.Strontium, 87.62f },
                { Element.Yttrium, 88.91f },
                { Element.Zirconium, 91.22f },
                { Element.Niobium, 92.91f },
                { Element.Molybdenum, 95.95f },
                { Element.Ruthenium, 101.1f },
                { Element.Rhodium, 102.9f },
                { Element.Palladium, 106.4f },
                { Element.Silver, 107.9f },
                { Element.Cadmium, 112.4f },
                { Element.Indium, 114.8f },
                { Element.Tin, 118.7f },
                { Element.Antimony, 121.8f },
                { Element.Tellurium, 127.6f },
                { Element.Iodine, 126.9f },
                { Element.Xenon, 131.3f },
                { Element.Cesium, 132.9f },
                { Element.Barium, 137.3f },
                { Element.Lanthanum, 138.9f },
                { Element.Cerium, 140.1f },
                { Element.Praseodymium, 140.9f },
                { Element.Neodymium, 144.2f },
                { Element.Samarium, 150.4f },
                { Element.Europium, 152.0f },
                { Element.Gadolinium, 157.3f },
                { Element.Terbium, 159.0f },
                { Element.Dysprosium, 162.5f },
                { Element.Holmium, 164.9f },
                { Element.Erbium, 167.3f },
                { Element.Thulium, 168.9f },
                { Element.Ytterbium, 173.0f },
                { Element.Lutetium, 175.0f },
                { Element.Hafnium, 178.5f },
                { Element.Tantalum, 180.9f },
                { Element.Tungsten, 183.8f },
                { Element.Rhenium, 186.2f },
                { Element.Osmium, 190.2f },
                { Element.Iridium, 192.2f },
                { Element.Platinum, 195.1f },
                { Element.Gold, 197.0f },
                { Element.Mercury, 200.6f },
                { Element.Thallium, 204.4f },
                { Element.Lead, 207.2f },
                { Element.Bismuth, 209.0f },
                { Element.Thorium, 232.0f },
                { Element.Protactinium, 231.0f },
                { Element.Uranium, 238.0f }
            };

        /// <summary>
        /// Get the standard atomic weight of the element in u, returning null if that data
        /// does not exist.
        /// </summary>
        public static float? GetStandardAtomicWeight(this Element element)
        {
            return Weights.TryGetValue(element, out var weight) ? (float?) weight : null;
        }
    }
}