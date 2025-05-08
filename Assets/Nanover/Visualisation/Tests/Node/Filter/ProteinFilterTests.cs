using Nanover.Core.Science;
using Nanover.Visualisation.Node.Filter;
using NUnit.Framework;

namespace Nanover.Visualisation.Tests.Node.Filter
{
    public class ProteinFilterTests
    {
        [Test]
        public void DoesWork()
        {
            var residueNames = new[]
            {
                "ALA", "XXX", "ILE", "ALE"
            };
            var particleResidues = new[]
            {
                0, 0, 0, 1, 1, 2, 2, 3, 3, 3
            };
            var elements = new[]
            {
                Element.Carbon,
                Element.Hydrogen,
                Element.Oxygen,
                Element.Carbon,
                Element.Hydrogen,
                Element.Carbon,
                Element.Hydrogen,
                Element.Carbon,
                Element.Hydrogen,
                Element.Oxygen
            };
            var node = new ProteinFilterNode();
            node.IncludeHydrogens.Value = true;
            node.IncludeNonStandardResidues.Value = false;
            node.IncludeWater.Value = true;
            node.ParticleResidues.Value = particleResidues;
            node.ResidueNames.Value = residueNames;
            node.ParticleElements.Value = elements;

            node.Refresh();
            CollectionAssert.AreEqual(new[]
                                      {
                                          0, 1, 2, 5, 6
                                      },
                                      node.ParticleFilter.Value);
        }
        
        [Test]
        public void DoesWork_IgnoreHydrogens()
        {
            var residueNames = new[]
            {
                "ALA", "XXX", "ILE", "ALE"
            };
            var particleResidues = new[]
            {
                0, 0, 0, 1, 1, 2, 2, 3, 3, 3
            };
            var elements = new[]
            {
                Element.Carbon,
                Element.Hydrogen,
                Element.Oxygen,
                Element.Carbon,
                Element.Hydrogen,
                Element.Carbon,
                Element.Hydrogen,
                Element.Carbon,
                Element.Hydrogen,
                Element.Oxygen
            };
            var node = new ProteinFilterNode();
            node.IncludeHydrogens.Value = false;
            node.IncludeWater.Value = true;
            node.IncludeNonStandardResidues.Value = false;
            node.ParticleResidues.Value = particleResidues;
            node.ResidueNames.Value = residueNames;
            node.ParticleElements.Value = elements;

            node.Refresh();
            CollectionAssert.AreEqual(new[]
                                      {
                                          0, 2, 5
                                      },
                                      node.ParticleFilter.Value);
        }
    }
}