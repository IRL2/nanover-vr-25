using System;
using System.Collections.Generic;
using System.IO;
using Nanover.Frame.Import.CIF.Components;

namespace Nanover.Frame.Import.CIF
{
    /// <summary>
    /// mmCIF importer for reading in the chemical component dictionary.
    /// </summary>
    internal class CifChemicalComponentBondsImport : CifBaseImport
    {
        protected override void ParseDataBlockLine()
        {
            if (chemicalComponent != null)
                chemicalComponents.Add(chemicalComponent);
            chemicalComponent = new ChemicalComponent();
            chemicalComponent.ResId = CurrentLine.Substring(5);
        }

        private readonly List<ChemicalComponent> chemicalComponents = new List<ChemicalComponent>();

        private ChemicalComponent chemicalComponent;

        public static IReadOnlyList<ChemicalComponent> ImportMultiple(TextReader reader)
        {
            var parser = new CifChemicalComponentBondsImport();
            parser.Parse(reader);
            parser.chemicalComponents.Add(parser.chemicalComponent);
            return parser.chemicalComponents;
        }

        protected override ParseTableRow GetTableHandler(string category, List<string> keywords)
        {
            if (category == "chem_comp_bond")
            {
                return GetBondHandler(keywords);
            }

            return null;
        }

        protected override bool ShouldParseCategory(string category)
        {
            return category == "chem_comp_bond";
        }

        private ParseTableRow GetBondHandler(IList<string> keywords)
        {
            var indexAtom1 = keywords.IndexOf("atom_id_1");
            var indexAtom2 = keywords.IndexOf("atom_id_2");
            var indexOrder = keywords.IndexOf("value_order");

            return data =>
            {
                var atom1 = data[indexAtom1].Trim();
                var atom2 = data[indexAtom2].Trim();
                var order = ParseBondOrder(data[indexOrder].Trim());

                chemicalComponent.AddBond(atom1, atom2, order);
            };
        }

        private int ParseBondOrder(string value)
        {
            switch (value)
            {
                case "SING":
                    return 1;
                case "DOUB":
                    return 2;
                case "TRIP":
                    return 3;
                default:
                    throw new ArgumentException($"Unknown bond type {value}");
            }
        }
    }
}