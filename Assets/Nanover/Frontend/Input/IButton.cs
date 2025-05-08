using System;

namespace Nanover.Frontend.Input
{
    /// <summary>
    /// Represents a button input that can be pressed, held, then released. Allows
    /// polling of the current state and listening to press and release events.
    /// </summary>
    public interface IButton
    {
        /// <summary>
        /// Is the button in a pressed state?
        /// </summary>
        bool IsPressed { get; }

        /// <summary>
        /// Callback when the button is pressed.
        /// </summary>
        event Action Pressed;

        /// <summary>
        /// Callback when the button is released.
        /// </summary>
        event Action Released;
    }
}