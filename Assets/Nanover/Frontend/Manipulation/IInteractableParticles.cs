using Nanover.Core.Math;

namespace Nanover.Frontend.Manipulation
{
    public interface IInteractableParticles
    {
        ActiveParticleGrab GetParticleGrab(Transformation grabber);
    }
}