using System.Collections;
using Nanover.Frontend.Input;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Nanover.Frontend.UI
{
    /// <summary>
    /// Implementation of <see cref="BaseInput" /> that uses a physical object's near a
    /// canvas as a mouse pointer.
    /// </summary>
    public class WorldSpaceCursorInput : BaseInput
    {
        private static WorldSpaceCursorInput Instance { get; set; }

#pragma warning disable 0649, 0109
        [SerializeField]
        private new Camera camera;
#pragma warning restore 0649, 0109

        private IPosedObject cursor;
        private Canvas canvas;
        private IButton clickButton;

        private float distanceToCanvas;
        private Vector2 screenPosition;
        private bool isCursorOnCanvas;

        private bool previousClickState = false;
        private bool currentClickState = false;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNull(Instance, $"Only one instance of {nameof(WorldSpaceCursorInput)} should exist in the scene.");
            Instance = this;
        }

        protected override void Start()
        {
            base.Start();
            Assert.IsNotNull(camera, $"{nameof(WorldSpaceCursorInput)} must have a non-null {nameof(camera)}");
            StartCoroutine(InitialiseWhenInputModuleReady());
        }

        public static void ClearSelection()
        {
            (EventSystem.current.currentInputModule as NanoverInputModule).ClearSelection();
        }

        private void Update()
        {
            Vector3? worldPoint;
            worldPoint = GetProjectedCursorPoint();

            var newCursorState = worldPoint.HasValue;
            if (!newCursorState && isCursorOnCanvas)
            {
                ClearSelection();
            }
            
            isCursorOnCanvas = newCursorState;
            if (worldPoint.HasValue)
            {
                screenPosition = camera.WorldToScreenPoint(worldPoint.Value);
            }

            previousClickState = currentClickState;
            currentClickState = mousePresent && IsClickPressed;
        }

        public bool IsClickPressed => clickButton != null && clickButton.IsPressed;

        /// <summary>
        /// Sets the canvas with an <see cref="IPosedObject" /> to provide the location
        /// of the physical cursor and an <see cref="IButton" /> to provide information on
        /// if a click is occuring.
        /// </summary>
        public static void SetCanvasAndCursor(Canvas canvas,
                                              IPosedObject cursor,
                                              IButton click = null)
        {
            Assert.IsNotNull(Instance, $"There is no instance of {nameof(WorldSpaceCursorInput)} in the scene.");
            Instance.canvas = canvas;
            Instance.cursor = cursor;
            Instance.clickButton = click;
        }

        /// <summary>
        /// Coroutine that waits until the <see cref="EventSystem" /> has prepared the
        /// input module before overriding the input.
        /// </summary>
        private IEnumerator InitialiseWhenInputModuleReady()
        {
            while (EventSystem.current.currentInputModule == null)
                yield return new WaitForEndOfFrame();

            var eventSystem = EventSystem.current;
            var inputModule = eventSystem.currentInputModule;

            inputModule.inputOverride = this;
        }

        /// <summary>
        /// Get the projection of the cursor onto the canvas, returning null if it is too
        /// far away.
        /// </summary>
        private Vector3? GetProjectedCursorPoint()
        {
            if (cursor?.Pose == null)
                return null;
            var cursorRadius = cursor.Pose.Value.Scale.x * 1.5f;
            var planeTransform = canvas.transform;
            var local = planeTransform.InverseTransformPoint(cursor.Pose.Value.Position);
            local.z = 0;
            var world = planeTransform.TransformPoint(local);
            var projSqrDistance = Vector3.SqrMagnitude(world - cursor.Pose.Value.Position);
            if (projSqrDistance > cursorRadius * cursorRadius)
                return null;
            return world;
        }

        /// <inheritdoc cref="BaseInput.mousePosition" />
        public override Vector2 mousePosition => screenPosition;

        /// <inheritdoc cref="BaseInput.mousePresent" />
        public override bool mousePresent => isCursorOnCanvas;

        /// <inheritdoc cref="BaseInput.GetMouseButtonDown" />
        public override bool GetMouseButtonDown(int button)
        {
            return button == 0 && currentClickState && !previousClickState;
        }

        /// <inheritdoc cref="BaseInput.GetMouseButton" />
        public override bool GetMouseButton(int button)
        {
            return button == 0 && currentClickState;
        }

        /// <inheritdoc cref="BaseInput.GetMouseButtonUp" />
        public override bool GetMouseButtonUp(int button)
        {
            return button == 0 && !currentClickState && previousClickState;
        }

        public static void ReleaseCanvas(Canvas canvas)
        {
            if (Instance != null && Instance.canvas == canvas)
            {
                Instance.canvas = null;
                Instance.cursor = null;
            }
        }
        
        public static void TriggerClick()
        {
            var hovered = (EventSystem.current.currentInputModule as NanoverInputModule)
                .CurrentHoverTarget;
            if (hovered != null)
            {
                ExecuteEvents.ExecuteHierarchy(hovered, new BaseEventData(EventSystem.current),
                                               ExecuteEvents.submitHandler);
            }
        }
    }
}