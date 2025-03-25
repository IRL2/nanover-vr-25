using System.Collections.Generic;

namespace Nanover.Core.Science
{
    /// <summary>
    /// Covalent radii of the atomic elements
    /// </summary>
    /// <remarks>
    /// The value of the Ccovalent radius are from
    /// Cordero, B., Gómez, V., Platero-Prats, A. E., Revés, M., Echeverría, J., Cremades, E., Alvarez, S. (2008). Covalent radii revisited. Dalton Transactions, (21), 2832. doi:10.1039/b801115j 
    /// Some elements do not have a Covalent radii provided, as there is
    /// insufficient literature to provide a value.
    /// </remarks>
    public static class ElementCovalentRadii
    {
        private static readonly Dictionary<Element, float> Radii =
            new Dictionary<Element, float>
            {
                [Element.Hydrogen] = 0.31f,
                [Element.Helium] = 0.28f,
                
                [Element.Lithium] = 1.28f,
                [Element.Beryllium] = 0.96f,
                
                [Element.Boron] = 0.84f,
                [Element.Carbon] = 0.76f,
                [Element.Nitrogen] = 0.71f,
                [Element.Oxygen] = 0.66f,
                [Element.Fluorine] = 0.57f,
                [Element.Neon] = 0.58f,
                
                [Element.Sodium] = 1.66f,
                [Element.Magnesium] = 1.41f,
                
                [Element.Aluminum] = 1.21f,
                [Element.Silicon] = 1.11f,
                [Element.Phosphorus] = 1.07f,
                [Element.Sulfur] = 1.05f,
                [Element.Chlorine] = 1.02f,
                [Element.Argon] = 1.06f,
                
                [Element.Potassium] = 2.03f,
                [Element.Calcium] = 1.76f,
                
                [Element.Scandium] = 1.7f,
                [Element.Titanium] = 1.6f,
                [Element.Vanadium] = 1.53f,
                [Element.Chromium] = 1.39f,
                [Element.Manganese] = 1.39f,
                [Element.Iron] = 1.32f,
                [Element.Cobalt] = 1.26f,
                [Element.Nickel] = 1.24f,
                [Element.Copper] = 1.32f,
                [Element.Zinc] = 1.22f,
                
                [Element.Gallium] = 1.22f,
                [Element.Germanium] = 1.19f,
                [Element.Arsenic] = 1.19f,
                [Element.Selenium] = 1.20f,
                [Element.Bromine] = 1.20f,
                [Element.Krypton] = 1.16f,
                
                [Element.Rubidium] = 1.2f,
                [Element.Strontium] = 1.95f,
                
                [Element.Yttrium] = 1.90f,
                [Element.Zirconium] = 1.75f,
                [Element.Niobium] = 1.64f,
                [Element.Molybdenum] = 1.54f,
                [Element.Technetium] = 1.47f,
                [Element.Ruthenium] = 1.46f,
                [Element.Rhodium] = 1.42f,
                [Element.Palladium] = 1.39f,
                [Element.Silver] = 1.45f,
                [Element.Cadmium] = 1.44f,
                
                [Element.Indium] = 1.42f,
                [Element.Tin] = 1.39f,
                [Element.Antimony] = 1.39f,
                [Element.Tellurium] = 1.38f,
                [Element.Iodine] = 1.39f,
                [Element.Xenon] = 1.40f,
                
                [Element.Cesium] = 2.44f,
                [Element.Barium] = 2.15f,
                
                [Element.Lanthanum] = 2.07f,
                [Element.Cerium] = 2.04f,
                [Element.Praseodymium] = 2.03f,
                [Element.Neodymium] = 2.01f,
                [Element.Promethium] = 1.99f,
                [Element.Samarium] = 1.98f,
                [Element.Europium] = 1.98f,
                [Element.Gadolinium] = 1.96f,
                [Element.Terbium] = 1.94f,
                [Element.Dysprosium] = 1.92f,
                [Element.Holmium] = 1.92f,
                [Element.Erbium] = 1.89f,
                [Element.Thulium] = 1.90f,
                [Element.Ytterbium] = 1.87f,
                [Element.Lutetium] = 1.87f,
                
                [Element.Hafnium] = 1.75f,
                [Element.Tantalum] = 1.7f,
                [Element.Tungsten] = 1.62f,
                [Element.Rhenium] = 1.51f,
                [Element.Osmium] = 1.44f,
                [Element.Iridium] = 1.41f,
                [Element.Platinum] = 1.36f,
                [Element.Gold] = 1.36f,
                [Element.Mercury] = 1.32f,
                
                [Element.Thallium] = 1.45f,
                [Element.Lead] = 1.46f,
                [Element.Bismuth] = 1.48f,
                [Element.Polonium] = 1.40f,
                [Element.Astatine] = 1.50f,
                [Element.Radon] = 1.50f,
                
                [Element.Francium] = 2.6f,
                [Element.Radium] = 2.21f,
                
                [Element.Actinium] = 2.15f,
                [Element.Thulium] = 2.06f,
                [Element.Protactinium] = 2f,
                [Element.Uranium] = 1.96f,
                [Element.Neptunium] = 1.9f,
                [Element.Plutonium] = 1.87f,
                [Element.Americium] = 1.80f,
                [Element.Curium] = 1.69f
                
                
            };

        /// <summary>
        /// Get the covalent radius of the element, returning null if that data does
        /// not exist.
        /// </summary>
        public static float? GetCovalentRadius(this Element element)
        {
            return Radii.TryGetValue(element, out var value) ? (float?) value : null;
        }
    }
}