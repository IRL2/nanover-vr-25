using System;
using System.Collections.Generic;
using Nanover.Frame;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Calculator
{
    /// <summary>
    /// Generates all internal bonds requires for a cycle.
    /// </summary>
    [Serializable]
    public class InteriorCyclesBondsNode
    {
        [SerializeField]
        private SelectionArrayProperty cycles = new SelectionArrayProperty();

        private readonly BondArrayProperty interiorBonds = new BondArrayProperty();

        public void Refresh()
        {
            if (cycles.IsDirty && cycles.HasValue)
            {
                var bonds = new List<BondPair>();
                
                foreach (var cycle in cycles.Value)
                {
                    for (var i = 0; i < cycle.Count - 2; i++)
                    {
                        for (var j = i + 2; j < cycle.Count; j++)
                        {
                            bonds.Add(new BondPair(cycle[i], cycle[j]));
                        }
                    }
                }

                interiorBonds.Value = bonds.ToArray();
                cycles.IsDirty = false;
            }
        }
    }
}