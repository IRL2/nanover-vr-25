using Nanover.Visualisation.Node.Filter;
using NUnit.Framework;

namespace Nanover.Visualisation.Tests.Node.Filter
{
    public class ResidueNameFilterTests
    {
        [Test]
        public void DoesWork()
        {
            var residueNames = new[]
            {
                "AAA", "BBB", "AAA", "BBB"
            };
            var particleResidues = new[]
            {
                0, 0, 0, 1, 1, 2, 2, 3, 3, 3
            };
            var pattern = "AAA";
            
            var node = new ResidueNameFilterNode();
            node.Pattern.Value = pattern;
            node.ParticleResidues.Value = particleResidues;
            node.ResidueNames.Value = residueNames;

            node.Refresh();
            CollectionAssert.AreEqual(new[]
                                      {
                                          0, 1, 2, 5, 6
                                      },
                                      node.ParticleFilter.Value);
        }
    }
}