using System;

namespace Nanover.Frontend.Input
{
    /// <summary>
    /// An <see cref="IButton" /> that can be triggered using the
    /// <see cref="Press()" />, <see cref="Release()" /> and <see cref="Toggle()" />
    /// methods.
    /// </summary>
    public sealed class DirectButton : IButton
    {
        /// <inheritdoc cref="IButton.IsPressed" />
        public bool IsPressed { get; private set; }

        /// <inheritdoc cref="IButton.Pressed" />
        public event Action Pressed;

        /// <inheritdoc cref="IButton.Released" />
        public event Action Released;

        /// <summary>
        /// Put this button into the pressed state if it is not already
        /// pressed.
        /// </summary>
        public void Press()
        {
            if (!IsPressed)
            {
                IsPressed = true;
                Pressed?.Invoke();
            }
        }

        /// <summary>
        /// Put this button into the released state if it is not already
        /// released.
        /// </summary>
        public void Release()
        {
            if (IsPressed)
            {
                IsPressed = false;
                Released?.Invoke();
            }
        }

        /// <summary>
        /// Toggle this button between the pressed and released states.
        /// </summary>
        public void Toggle()
        {
            if (IsPressed)
            {
                Release();
            }
            else
            {
                Press();
            }
        }
    }
}