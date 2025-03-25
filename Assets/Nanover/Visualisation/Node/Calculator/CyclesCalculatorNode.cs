using System;
using System.Collections.Generic;
using System.Linq;
using Nanover.Frame;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Calculator
{
    /// <summary>
    /// Locate cycles by looking at the bonding in a molecule, optionally limiting the search
    /// to only include cycles wholely within a single residue.
    /// </summary>
    [Serializable]
    public class CyclesCalculatorNode : GenericOutputNode
    {
        /// <summary>
        /// Bonds involved in the molecule.
        /// </summary>
        public IProperty<BondPair[]> Bonds => bonds;

        /// <inheritdoc cref="Bonds"/>
        [SerializeField]
        private BondArrayProperty bonds = new BondArrayProperty();


        /// <summary>
        /// Total number of particles involved in the system.
        /// </summary>
        public IProperty<int> ParticleCount => particleCount;

        /// <inheritdoc cref="ParticleCount"/>
        [SerializeField]
        private IntProperty particleCount = new IntProperty();

        /// <summary>
        /// List of set of indices that make up each cycle.
        /// </summary>
        public IReadOnlyProperty<Cycle[]> Cycles => cycles;

        /// <inheritdoc cref="Cycles"/>
        private readonly ArrayProperty<Cycle> cycles = new ArrayProperty<Cycle>();

        /// <summary>
        /// Set of particle residue indices.
        /// </summary>
        public IReadOnlyProperty<int[]> ParticleResidues => particleResidues;

        /// <inheritdoc cref="ParticleResidues"/>
        private readonly IntArrayProperty particleResidues = new IntArrayProperty();

        /// <summary>
        /// Number of cycles.
        /// </summary>
        public IReadOnlyProperty<int> CyclesCount => cyclesCount;

        /// <inheritdoc cref="CyclesCount"/>
        private readonly IntProperty cyclesCount = new IntProperty();

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => bonds.IsDirty || particleCount.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => bonds.HasNonEmptyValue();

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            bonds.IsDirty = false;
            particleCount.IsDirty = false;
        }

        /// <inheritdoc cref="GenericOutputNode.UpdateOutput"/>
        protected override void UpdateOutput()
        {
            var particleCount = this.particleCount.Value;
            var neighbourList = new List<int>[particleCount];
            for (var i = 0; i < particleCount; i++)
                neighbourList[i] = new List<int>();

            foreach (var bond in bonds)
            {
                // Only include bonds within residues.
                if (particleResidues.HasValue)
                {
                    var res1 = particleResidues.Value[bond.A];
                    var res2 = particleResidues.Value[bond.B];
                    if (res1 != res2)
                        continue;
                }
                neighbourList[bond.A].Add(bond.B);
                neighbourList[bond.B].Add(bond.A);
            }

            cycles.Value = ComputeChordlessCycles(particleCount, neighbourList).ToArray();
            cyclesCount.Value = cycles.Value.Length;
        }

        /// <inheritdoc cref="GenericOutputNode.ClearOutput"/>
        protected override void ClearOutput()
        {
            cycles.UndefineValue();
            cyclesCount.UndefineValue();
        }

        /// <summary>
        /// Chordless cycle computation using https://arxiv.org/pdf/1309.1051.pdf
        /// </summary>
        private IEnumerable<Cycle> ComputeChordlessCycles(int count, List<int>[] adjacency)
        {
            var T = new Queue<List<int>>();
            for (var u = 0; u < count; u++)
                foreach (var x in adjacency[u])
                foreach (var y in adjacency[u])
                {
                    // Ensure u < x
                    if (u >= x)
                        continue;

                    // Ensure x < y and hence u < x < y
                    if (x >= y)
                        continue;

                    if (adjacency[x].Contains(y))
                    {
                        yield return new Cycle(x, u, y);
                    }
                    else
                    {
                        T.Enqueue(new List<int>
                        {
                            x,
                            u,
                            y
                        });
                    }
                }

            while (T.Any())
            {
                var p = T.Dequeue();

                if (p.Count == 6)
                    continue;

                var u2 = p[1];
                var ut = p.Last();
                foreach (var v in adjacency[ut])
                {
                    if (v <= u2)
                        continue;
                    if (Enumerable.Range(1, p.Count - 2)
                                  .Any(i => adjacency[p[i]].Contains(v)))
                        continue;

                    var p2 = new List<int>();
                    p2.AddRange(p);
                    p2.Add(v);

                    if (adjacency[p[0]].Contains(v))
                        yield return new Cycle(p2.ToArray());
                    else
                        T.Enqueue(p2);
                }
            }
        }
    }
}