using System.IO;
using Nanover.Frame.Import.CIF;
using Nanover.Frame.Import.CIF.Components;
using Nanover.Frame.Import.CIF.Structures;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Nanover.Trajectory.Import.Tests
{
    /// <summary>
    /// Contains non-standard monomer 9XQ, covalent bond, disulphide bond, multiple PDB
    /// models
    /// </summary>
    public class Protein5OLF
    {
        private CifSystem component;

        [SetUp]
        public void Setup()
        {
            var file = Resources.Load("5OLF.cif");
            component = CifImport.ImportSystem(new StringReader(file.ToString()), ChemicalComponentDictionary.Instance);
        }

        [Test]
        public void DisulphideBond()
        {
            var atom1 = component.FindAtomById("SG", "CYS", 3, "A");
            var atom2 = component.FindAtomById("SG", "CYS", 7, "A");
            Assert.IsNotNull(component.GetBond(atom1, atom2));
        }

        [Test]
        public void CovalentBond()
        {
            var atom1 = component.FindAtomById("C", "9XQ", 1, "A");
            var atom2 = component.FindAtomById("N", "ALA", 2, "A");
            Assert.IsNotNull(component.GetBond(atom1, atom2));
        }
    }
}