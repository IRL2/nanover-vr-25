using System.Collections.Generic;
using System.Linq;

namespace Nanover.Frame.Import.CIF.Structures
{
    internal class CifAsymmetricUnit
    {
        public string AsymId { get; set; }

        public CifEntity Entity { get; set; }

        private readonly List<CifResidue> residues = new List<CifResidue>();

        public IReadOnlyList<CifResidue> Residues => residues;
        public int AbsoluteIndex { get; set; }

        public CifResidue FindResidue(string compId, int? seqId)
        {
            return seqId.HasValue
                       ? residues.FirstOrDefault(
                           res => res.ComponentId == compId && res.SequenceId == seqId)
                       : residues.FirstOrDefault(res => res.ComponentId == compId);
        }

        public void AddResidue(CifResidue residue)
        {
            residues.Add(residue);
        }
    }
}