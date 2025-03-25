using System;

namespace Nanover.Visualisation.Node.Protein
{
    [Flags]
    public enum SecondaryStructureAssignment
    {
        None = 0,
        AlphaHelix = 0x1,
        ThreeTenHelix = 0x2,
        PiHelix = 0x4,
        Sheet = 0x8,
        Bridge = 0x10,
        Turn = 0x20,
        Bend = 0x40,
        Helix = AlphaHelix | PiHelix | ThreeTenHelix,
        Strand = Sheet | Bridge,
        Loop = Turn | Bend
    }
}