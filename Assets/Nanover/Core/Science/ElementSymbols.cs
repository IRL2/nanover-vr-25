using System;
using System.Collections.Generic;

namespace Nanover.Core.Science
{
    /// <summary>
    /// Mapping between atomic elements and their symbols.
    /// </summary>
    public static class ElementSymbols
    {
        private static readonly Dictionary<Element, string> Symbols =
            new Dictionary<Element, string>
            {
                { Element.Virtual, "??" },
                { Element.Hydrogen, "H" },
                { Element.Helium, "He" },
                { Element.Lithium, "Li" },
                { Element.Beryllium, "Be" },
                { Element.Boron, "B" },
                { Element.Carbon, "C" },
                { Element.Nitrogen, "N" },
                { Element.Oxygen, "O" },
                { Element.Fluorine, "F" },
                { Element.Neon, "Ne" },
                { Element.Sodium, "Na" },
                { Element.Magnesium, "Mg" },
                { Element.Aluminum, "Al" },
                { Element.Silicon, "Si" },
                { Element.Phosphorus, "P" },
                { Element.Sulfur, "S" },
                { Element.Chlorine, "Cl" },
                { Element.Argon, "Ar" },
                { Element.Potassium, "K" },
                { Element.Calcium, "Ca" },
                { Element.Scandium, "Sc" },
                { Element.Titanium, "Ti" },
                { Element.Vanadium, "V" },
                { Element.Chromium, "Cr" },
                { Element.Manganese, "Mn" },
                { Element.Iron, "Fe" },
                { Element.Cobalt, "Co" },
                { Element.Nickel, "Ni" },
                { Element.Copper, "Cu" },
                { Element.Zinc, "Zn" },
                { Element.Gallium, "Ga" },
                { Element.Germanium, "Ge" },
                { Element.Arsenic, "As" },
                { Element.Selenium, "Se" },
                { Element.Bromine, "Br" },
                { Element.Krypton, "Kr" },
                { Element.Rubidium, "Rb" },
                { Element.Strontium, "Sr" },
                { Element.Yttrium, "Y" },
                { Element.Zirconium, "Zr" },
                { Element.Niobium, "Nb" },
                { Element.Molybdenum, "Mo" },
                { Element.Technetium, "Tc" },
                { Element.Ruthenium, "Ru" },
                { Element.Rhodium, "Rh" },
                { Element.Palladium, "Pd" },
                { Element.Silver, "Ag" },
                { Element.Cadmium, "Cd" },
                { Element.Indium, "In" },
                { Element.Tin, "Sn" },
                { Element.Antimony, "Sb" },
                { Element.Tellurium, "Te" },
                { Element.Iodine, "I" },
                { Element.Xenon, "Xe" },
                { Element.Cesium, "Cs" },
                { Element.Barium, "Ba" },
                { Element.Lanthanum, "La" },
                { Element.Cerium, "Ce" },
                { Element.Praseodymium, "Pr" },
                { Element.Neodymium, "Nd" },
                { Element.Promethium, "Pm" },
                { Element.Samarium, "Sm" },
                { Element.Europium, "Eu" },
                { Element.Gadolinium, "Gd" },
                { Element.Terbium, "Tb" },
                { Element.Dysprosium, "Dy" },
                { Element.Holmium, "Ho" },
                { Element.Erbium, "Er" },
                { Element.Thulium, "Tu" },
                { Element.Ytterbium, "Yb" },
                { Element.Lutetium, "Lu" },
                { Element.Hafnium, "Hf" },
                { Element.Tantalum, "Ta" },
                { Element.Tungsten, "W" },
                { Element.Rhenium, "Re" },
                { Element.Osmium, "Os" },
                { Element.Iridium, "Ir" },
                { Element.Platinum, "Pt" },
                { Element.Gold, "Au" },
                { Element.Mercury, "Hg" },
                { Element.Thallium, "Tl" },
                { Element.Lead, "Pb" },
                { Element.Bismuth, "Bi" },
                { Element.Polonium, "Po" },
                { Element.Astatine, "At" },
                { Element.Radon, "Rn" },
                { Element.Francium, "Fr" },
                { Element.Radium, "Ra" },
                { Element.Actinium, "Ac" },
                { Element.Thorium, "Th" },
                { Element.Protactinium, "Pa" },
                { Element.Uranium, "U" },
                { Element.Neptunium, "Np" },
                { Element.Plutonium, "Pu" },
                { Element.Americium, "Am" },
                { Element.Curium, "Cm" },
                { Element.Berkelium, "Bk" },
                { Element.Californium, "Cf" },
                { Element.Einsteinium, "Es" },
                { Element.Fermium, "Fm" },
                { Element.Mendelevium, "Md" },
                { Element.Nobelium, "No" },
                { Element.Lawrencium, "Lr" },
                { Element.Rutherfordium, "Rf" },
                { Element.Dubnium, "Db" },
                { Element.Seaborgium, "Sg" },
                { Element.Bohrium, "Bh" },
                { Element.Hassium, "Hs" },
                { Element.Meitnerium, "Mt" },
                { Element.Darmstadtium, "Ds" },
                { Element.Roentgenium, "Rg" },
                { Element.Copernicium, "Cn" },
                { Element.Nihonium, "Nh" },
                { Element.Flerovium, "Fv" },
                { Element.Moscovium, "Ms" },
                { Element.Livermorium, "Lv" },
                { Element.Tennessine, "Ts" },
                { Element.Oganesson, "Og" }
            };


        /// <summary>
        /// Get the atomic symbol of the element.
        /// </summary>
        public static string GetSymbol(this Element element)
        {
            return Symbols.TryGetValue(element, out var value) ? value : null;
        }

        /// <summary>
        /// Get an atomic element based on its atomic symbol.
        /// </summary>
        public static Element? GetFromSymbol(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return null;
            symbol = symbol.Trim();
            foreach (var (element, potentialSymbol) in Symbols)
                if (potentialSymbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))
                    return element;
            return null;
        }
    }
}