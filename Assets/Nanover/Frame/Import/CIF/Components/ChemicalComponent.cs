using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nanover.Frame.Import.CIF.Components
{
    /// <summary>
    /// Serializable representation of a chemical component
    /// </summary>
    [Serializable]
    public class ChemicalComponent
    {
        [SerializeField]
        private string resId;

        [SerializeField]
        internal List<Bond> bonds = new List<Bond>();

        public IEnumerable<Bond> Bonds => bonds;

        [Serializable]
        public struct Bond
        {
            [SerializeField]
            public string a;

            [SerializeField]
            public string b;

            [SerializeField]
            public int order;
        }

        public string ResId
        {
            get => resId;
            set => resId = value;
        }

        public void AddBond(string a, string b, int order)
        {
            bonds.Add(new Bond()
            {
                a = a,
                b = b,
                order = order
            });
        }
    }
}