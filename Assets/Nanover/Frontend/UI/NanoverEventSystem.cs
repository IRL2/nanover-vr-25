using UnityEngine.EventSystems;

namespace Nanover.Frontend.UI
{
    /// <summary>
    /// Override for <see cref="EventSystem" /> so that losing application focus
    /// does not affect the UI.
    /// </summary>
    public class NanoverEventSystem : EventSystem
    {
        protected override void OnApplicationFocus(bool hasFocus)
        {
            // Prevent application focus from affecting the event system
        }
    }
}