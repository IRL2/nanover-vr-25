using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nanover.Visualisation;
using NanoverImd;
using UnityEngine;
using UnityEngine.XR;
//using UnityEngine.XR.Interaction.Toolkit;

using Nanover.Frontend.Input;
using Nanover.Frontend.XR;

using Text = TMPro.TextMeshProUGUI;
using TMPro;
using Nanover.Frontend.Controllers;
using System.Drawing;
using UnityEngine.UIElements;
using System.Reflection.Emit;

namespace NanoverImd.Interaction
{
    public class ReferencePointsManager : MonoBehaviour
    {
        //[SerializeField]
        //private NanoverImdSimulation simulation;

        //[SerializeField]
        //private SynchronisedFrameSource frameSource;

        [SerializeField]
        private LiveMotionTrails liveMotionTrails;

        [SerializeField]
        private Transform userPointer;

        /// <summary>
        /// Pointer object inside the simulation space. invisible
        /// </summary>
        [SerializeField]
        private Transform userPointerTarget;

        [SerializeField]
        /// <summary> item in the hierarchy that will be used to parent for the added reference points</summary>
        private Transform simulationParent;


        [SerializeField]
        private Transform destinationZone;

        private Renderer pointerRenderer;

        [SerializeField]
        private GameObject referencePointPrefab;

        [SerializeField]
        private TextMeshPro lineInfoLabel;
        [SerializeField]
        private TextMeshPro lineModeInstructions;

        const string DRAWING_DISABLED = "<b>Press [menu] to enable draw mode";
        const string DRAWING_INSTRUCTIONS = "<b>Hold [A]</b> to draw a line\r\n<b>Press [A]</b> to add points to the line\r\n<b>Press [B]</b> to delete the line\r\n\r\n<b>Press [Y]</b> to reset trail\r\n<b>Press [X]</b> to position destiny\r\n\r\n<b>Press [menu]</b> to disable drawing mode";

        [SerializeField]
        private LineRenderer line;

        float lineLength = 0.0f;
        float lineSmoothnessA = 0.0f;
        double lineSmoothnessB = 0.0f;

        private IButton secondaryButton;
        bool secondaryButtonPrevPressed = false;

        private IButton triggerButton;
        bool triggerButtonPrevPressed = false;

        private IButton primaryButton;
        bool primaryButtonPrevPressed = false;

        private IButton grabButton;
        bool grabButtonPrevPressed = false;

        private IButton menuButton;
        bool menuButtonPrevPressed = false;
        private IButton xButton;
        bool xButtonPrevPressed = false;
        private IButton yButton;
        bool yButtonPrevPressed = false;

        public float singlePointThreshold = 0.05f;

        InputDevice rightHandDevice;
        UnityEngine.XR.HapticCapabilities hapticCapabilities;


        bool modeActive = false;
        bool modeDrawing = true;
        bool modeTrailing = true;


        private void Start()
        {
            primaryButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.primaryButton);
            secondaryButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.secondaryButton);
            triggerButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.triggerButton);
            grabButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.gripButton);

            menuButton = InputDeviceCharacteristics.Left.WrapUsageAsButton(CommonUsages.menuButton);
            xButton = InputDeviceCharacteristics.Left.WrapUsageAsButton(CommonUsages.primaryButton);
            yButton = InputDeviceCharacteristics.Left.WrapUsageAsButton(CommonUsages.secondaryButton);

            RestartLine();

            lineInfoLabel.text = "";
            lineModeInstructions.text = DRAWING_DISABLED;
            //pointerRenderer.enabled = false;
            UnityEngine.Debug.Log("This requires a user pointer in the hierarchy, a gameobject named 'Cursor', and a grandfather named 'Right Controller'");
        }

        //int pointCount = 0;
        private List<Vector3> referencePoints = new List<Vector3>();
        private List<float> workSnapshots = new List<float>();

        public float snapshotFrequency = 2f;
        private float drawingElapsedTime = 0.0f;

        private void Update()
        {
            if (menuButtonPrevPressed && !menuButton.IsPressed)
            {
                if (!modeActive)
                {
                    modeActive = true;
                    pointerRenderer.enabled = true;
                    line.enabled = true;
                    pointerRenderer.material.color = new UnityEngine.Color(1f, 1f, 1f, 0.1f);
                    lineModeInstructions.text = DRAWING_INSTRUCTIONS;
                    UnityEngine.Debug.Log("Trajectory drawing mode activated");
                }
                else
                {
                    modeActive = false;
                    line.enabled = false;
                    pointerRenderer.enabled = false;    
                    lineInfoLabel.text = "";
                    lineModeInstructions.text = DRAWING_DISABLED;
                    UnityEngine.Debug.Log("Trajectory drawing mode deactivated");
                }
            }
            
            menuButtonPrevPressed = menuButton.IsPressed;
            yButtonPrevPressed = yButton.IsPressed;

            if (yButton.IsPressed && !yButtonPrevPressed)
            {
                liveMotionTrails.ResetLine();
            }

            if (!modeActive) return;


            // setting up the pointer and haptics
            if (userPointer == null)
            {
                GameObject.FindObjectsByType<ControllerPivot>(FindObjectsSortMode.None)
                    .Where(x => x.gameObject.name == "Cursor")
                    .Where(x => x.gameObject.transform.parent.transform.parent.name.Contains("Right"))
                    .ToList()
                    .LastOrDefault(x => userPointer = x.transform);

                UnityEngine.Debug.Log("User pointer found!");

                rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                rightHandDevice.TryGetHapticCapabilities(out hapticCapabilities);
                if (!hapticCapabilities.supportsImpulse)
                {
                    UnityEngine.Debug.LogWarning("Right hand device does not support haptic impulses.");
                }
                else
                {
                    UnityEngine.Debug.Log("Right hand device supports haptic impulses.");
                    rightHandDevice.SendHapticImpulse(0, .5f, .1f); // Test haptic feedback
                }

                // setting the pointer
                pointerRenderer = userPointerTarget.gameObject.GetComponentInChildren<Renderer>();
                pointerRenderer.material.color = new UnityEngine.Color(1f, 1f, 1f, 0.1f);
                pointerRenderer.enabled = false;

                return;
            }

            userPointerTarget.position = userPointer.position;

            lineInfoLabel.text = "\npointer at " + userPointerTarget.localPosition.ToString() + " \n";


            if (lineLength > 0.0f)
            {
                lineInfoLabel.text += "\n<u>trajectory reference line</u>" + (primaryButton.IsPressed ? " [drawing] " : "");
                lineInfoLabel.text += "\n   lenght is " + lineLength.ToString("F2") + " nm";
                lineInfoLabel.text += "\n   from " + line.GetPosition(0).ToString("F2");
                lineInfoLabel.text += "\n   to " + line.GetPosition(line.positionCount - 1).ToString("F2");
                lineInfoLabel.text += "\n   having " + line.positionCount.ToString() + " points";
                lineInfoLabel.text += "\n   angular triplets " + (lineSmoothnessA*100).ToString("F1") + "%";
                lineInfoLabel.text += "\n   path jagger " + lineSmoothnessB.ToString("F2");
                lineInfoLabel.text += "\n";
            }

            userPointerTarget.rotation = Quaternion.LookRotation(userPointer.transform.forward, userPointer.transform.up);

            // delete the line
            if (secondaryButton.IsPressed)
            {
                RestartLine();
                lineLength = 0.0f;
                return;
            }
            // draw
            else if (primaryButton.IsPressed)
            {

                // first click, first point
                if (!primaryButtonPrevPressed)
                {
                    primaryButtonPrevPressed = true;
                    drawingElapsedTime = snapshotFrequency;

                    pointerRenderer.material.color = new UnityEngine.Color(1f, 1f, 1f, 0.5f);

                    //RestartLine();
                    // after a line is drawn, the user can add points by clicking the trigger button
                    AddReferencePoint(userPointer);
                }

                drawingElapsedTime += Time.deltaTime;
                if (drawingElapsedTime >= snapshotFrequency)
                {
                    drawingElapsedTime = 0.0f;
                    AddReferencePoint(userPointer);
                }
                else
                {
                    DragLastPointOnLine(userPointer);
                }

                lineLength = GetLineLenght(line);
                lineSmoothnessA = CalculateAngularSmoothness(line) / Mathf.PI;
                lineSmoothnessB = CalculateSmoothness(line);
            }
            else if (primaryButtonPrevPressed)
            {
                line.Simplify(0.01f);
                lineLength = GetLineLenght(line);
                lineSmoothnessA = CalculateAngularSmoothness(line) / Mathf.PI;
                lineSmoothnessB = CalculateSmoothness(line);

                pointerRenderer.material.color = new UnityEngine.Color(1f, 1f, 1f, 0.1f);
                UnityEngine.Debug.Log("Finish drawing. Simplifiying the line");
            }


            if (xButton.IsPressed)
            {
                destinationZone.localScale = simulationParent.localScale * 0.1f;
                destinationZone.localPosition = userPointerTarget.localPosition;
                destinationZone.localRotation = userPointerTarget.localRotation;
            }


            primaryButtonPrevPressed = primaryButton.IsPressed;
            secondaryButtonPrevPressed = secondaryButton.IsPressed;
            triggerButtonPrevPressed = triggerButton.IsPressed;
            grabButtonPrevPressed = grabButton.IsPressed;
            menuButtonPrevPressed = menuButton.IsPressed;
            xButtonPrevPressed = xButton.IsPressed;
            yButtonPrevPressed = yButton.IsPressed;
        }

        /// <summary>
        /// Returns the length of the line renderer by summing the distance between each point.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        float GetLineLenght(LineRenderer l)
        {
            float length = 0.0f;

            for (int i = 0; i < l.positionCount - 1; i++)
            {
                length += Vector3.Distance(l.GetPosition(i), l.GetPosition(i + 1));
            }

            return length;
        }


        /// <summary>
        /// Calculates the smoothness of a line represented by a list of points
        /// sums the squared second differences of the points. (squared length of the second derivative)
        /// 
        public static float CalculateSmoothness(LineRenderer lineRenderer)
        {
            int pointCount = lineRenderer.positionCount;
            if (pointCount < 3)
                return 0f; // Not enough points to calculate second differences

            Vector3[] positions = new Vector3[pointCount];
            lineRenderer.GetPositions(positions);

            float sum = 0f;

            for (int i = 0; i < pointCount - 2; i++)
            {
                // v1 total variation of first derivatives
                //Vector3 firstDiff = positions[i + 1] - positions[i];
                //sum += firstDiff.magnitude;

                // v2 discrete curvature = sum of squared second derivatives
                Vector3 secondDiff = positions[i + 2] - 2 * positions[i + 1] + positions[i];
                sum += secondDiff.sqrMagnitude; // Squared length of the second derivative
            }

            return sum;
        }

        // angle between triplets
        public static float CalculateAngularSmoothness(LineRenderer lineRenderer)
        {
            int pointCount = lineRenderer.positionCount;
            if (pointCount < 3)
                return 0f; // Not enough segments to evaluate angles

            Vector3[] positions = new Vector3[pointCount];
            lineRenderer.GetPositions(positions);

            float totalDeviation = 0f;
            int angleCount = 0;

            for (int i = 1; i < pointCount - 1; i++)
            {
                Vector3 prev = positions[i] - positions[i - 1];
                Vector3 next = positions[i + 1] - positions[i];

                if (prev.sqrMagnitude == 0f || next.sqrMagnitude == 0f)
                    continue; // Skip degenerate segments

                float dot = Vector3.Dot(prev.normalized, next.normalized);
                dot = Mathf.Clamp(dot, -1f, 1f); // Clamp to avoid NaNs due to rounding

                float angle = Mathf.Acos(dot); // In radians
                float deviation = Mathf.Abs(Mathf.PI - angle); // π (180°) = perfectly straight

                totalDeviation += deviation;
                angleCount++;
            }

            return (angleCount > 0) ? totalDeviation / angleCount : 0f;
        }


        /// <summary>
        /// Restarts the line by clearing all points and destroying the reference points.
        /// </summary>
        private void RestartLine()
        {
            referencePoints.Clear();
            workSnapshots.Clear();
            line.positionCount = 0;
            //pointCount = 0;
            lineSmoothnessA = MathF.PI;
            lineLength = 0.0f;

            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag: "ReferencePoints");
            foreach (GameObject g in gameObjects)
            {
                Destroy(g);
            }
        }


        // refactor this pls!
        private void DragLastPointOnLine(Transform position)
        {
            if (line.positionCount <= 1) return;

            //GameObject empty = new GameObject("Empty");
            Transform newPosition = userPointerTarget.transform;
            newPosition.position = position.position;

            newPosition.position = userPointer.transform.position;
            newPosition.SetParent(simulationParent, true);

            line.SetPosition(line.positionCount - 1, newPosition.transform.localPosition);

            //Destroy(empty);
        }


        private void AddReferencePoint(Transform position)
        {
            //GameObject empty = new GameObject("Empty");
            Transform newPosition = userPointerTarget;
            newPosition.position = position.position;

            newPosition.position = userPointer.transform.position;
            newPosition.SetParent(simulationParent, true);

            if (line.positionCount == 2)
            {
                if (Vector3.Distance(line.GetPosition(line.positionCount - 1), newPosition.localPosition) > singlePointThreshold)
                {
                    line.SetPosition(0, line.GetPosition(1));
                }
            }

            if (line.positionCount >= 2)
            {
                if (Vector3.Distance(line.GetPosition(line.positionCount - 1), newPosition.localPosition) < singlePointThreshold)
                {
                    return;
                }
            }

            line.positionCount = line.positionCount + 1;

            line.SetPosition(line.positionCount - 1, newPosition.localPosition);

            rightHandDevice.SendHapticImpulse(0u, 0.05f, 0.005f);
        }

    }

}


public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
