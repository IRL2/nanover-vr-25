using Nanover.Frontend.Controllers;
using Nanover.Frontend.Input;
using UnityEngine;
using UnityEngine.XR;
using Nanover.Frontend.XR;

namespace Nanover.Frontend.UI
{
    /// <summary>
    /// Component required to register a Unity canvas such that the given controller
    /// can interact with it.
    /// </summary>
    /// <remarks>
    /// All canvases that would like to be interacted with by physical controllers
    /// should have a script that derives from <see cref="PhysicalCanvasInput" />. This should
    /// provide the controller and the action which is counted as a 'click'. The
    /// <see cref="RegisterCanvas" /> method can be overriden to provide a custom
    /// <see cref="IPosedObject" /> and <see cref="IButton" /> to provide the cursor
    /// location and click button.
    /// </remarks>
    [RequireComponent(typeof(Canvas))]
    public class PhysicalCanvasInput : MonoBehaviour
    {
#pragma warning disable 0649
        /// <summary>
        /// The controller manager that provides the left/right controllers
        /// </summary>
        [SerializeField]
        private ControllerManager controllers;

        /// <summary>
        /// The input source to use for <see cref="inputAction" />.
        /// </summary>
        [SerializeField]
        private InputDeviceCharacteristics inputSource;
#pragma warning restore 0649

        private Canvas canvas;

        protected Canvas Canvas => canvas;

        public VrController Controller => controllers.GetController(inputSource);

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        private void OnEnable()
        {
            RegisterCanvas();
        }

        /// <summary>
        /// Register the canvas with the cursor input system.
        /// </summary>
        protected virtual void RegisterCanvas()
        {
            var trigger = inputSource.WrapUsageAsButton(CommonUsages.triggerButton);

            WorldSpaceCursorInput.SetCanvasAndCursor(canvas,
                                                     Controller.CursorPose,
                                                     trigger);
            controllers.SetDominantHand(inputSource);
        }

        private void OnDisable()
        {
            WorldSpaceCursorInput.ReleaseCanvas(canvas);
        }
    }
}