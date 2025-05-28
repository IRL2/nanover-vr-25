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

namespace NanoverImd.Interaction
{
    public class ReferencePointsManager : MonoBehaviour
    {
        [SerializeField]
        private NanoverImdSimulation simulation;

        [SerializeField]
        private SynchronisedFrameSource frameSource;

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
        private GameObject referencePointPrefab;

        [SerializeField]
        private TextMeshPro label;

        [SerializeField]
        private LineRenderer line;

        float lineLength = 0.0f;
        double lineSmoothness = 0.0f;

        private IButton secondaryButton;
        bool secondaryButtonPrevPressed = false;

        private IButton triggerButton;
        bool triggerButtonPrevPressed = false;

        private IButton primaryButton;
        bool primaryButtonPrevPressed = false;

        private IButton grabButton;
        bool grabButtonPrevPressed = false;


        bool drawingMode = false;
        
        public float singlePointThreshold = 0.05f;

        InputDevice rightHandDevice;
        UnityEngine.XR.HapticCapabilities hapticCapabilities;



        private void Start()
        {
            primaryButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.primaryButton);
            secondaryButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.secondaryButton);
            triggerButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.triggerButton);
            grabButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.gripButton);

            RestartLine();

            UnityEngine.Debug.Log("This requires a user pointer in the hierarchy, a gameobject named 'Cursor', and a grandfather named 'Right Controller'");
        }

        int pointCount = 0;
        private List<Vector3> referencePoints = new List<Vector3>();
        private List<float> workSnapshots = new List<float>();

        public float snapshotFrequency = 2f;
        private float drawingElapsedTime = 0.0f;

        private void Update()
        {
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

                userPointerTarget.gameObject.GetComponentInChildren<Renderer>().material.color = new UnityEngine.Color(1f, 1f, 1f, 0.1f);
                userPointerTarget.gameObject.GetComponentInChildren<Renderer>().enabled = false;

                

                return;
            }

            userPointerTarget.position = userPointer.position;

            label.text = "\npointer at " + userPointerTarget.localPosition.ToString() + " \n";

            if (primaryButton.IsPressed)
            {
                label.text += "\n[drawing]";
            } else
            {
                label.text += "\n";
            }

            if (lineLength > 0.0f)
            {
                label.text += "\n<u>trajectory reference line</u>";
                label.text += "\n   length is " + lineLength.ToString("F2") + " nm";
                label.text += "\n   from " + line.GetPosition(0).ToString("F2");
                label.text += "\n   to   " + line.GetPosition(line.positionCount - 1).ToString("F2");
                label.text += "\n   having " + line.positionCount.ToString() + " points";
                label.text += "\n   smooth index is " + (lineSmoothness/ line.positionCount).ToString("F2") + " *";
                label.text += "\n";
            }

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
                userPointerTarget.rotation = Quaternion.LookRotation(userPointer.transform.forward, userPointer.transform.up);

                // first click, first point
                if (!primaryButtonPrevPressed)
                {
                    primaryButtonPrevPressed = true;
                    drawingElapsedTime = snapshotFrequency;

                    userPointerTarget.gameObject.GetComponentInChildren<Renderer>().material.color = new UnityEngine.Color(1f, 1f, 1f, 0.5f);
                    userPointerTarget.gameObject.GetComponentInChildren<Renderer>().enabled = true;

                    //RestartLine();
                    // after a line is drawn, the user can add points by clicking the trigger button
                    AddReferencePoint(userPointer);
                }

                drawingElapsedTime += Time.deltaTime;
                if (drawingElapsedTime >= snapshotFrequency)
                {
                    drawingElapsedTime = 0.0f;
                    AddReferencePoint(userPointer);
                } else
                {
                    DragLastPointOnLine(userPointer);
                }

                lineLength = GetLineLenght(line);
                lineSmoothness = CalculateSmoothness(line);
            }
            else if (primaryButtonPrevPressed)
            {
                line.Simplify(0.02f);
                lineLength = GetLineLenght(line);
                UnityEngine.Debug.Log("Finish drawing. Simplifiying the line");

                userPointerTarget.gameObject.GetComponentInChildren<Renderer>().material.color = new UnityEngine.Color(1f,1f,1f, 0f);
            }

            primaryButtonPrevPressed = primaryButton.IsPressed;
            secondaryButtonPrevPressed = secondaryButton.IsPressed;
            triggerButtonPrevPressed = triggerButton.IsPressed;
            grabButtonPrevPressed = grabButton.IsPressed;
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
        /// Calculates the smoothness of a line represented by a list of points.
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
                Vector3 secondDiff = positions[i + 2] - 2 * positions[i + 1] + positions[i];
                sum += secondDiff.sqrMagnitude; // Squared length of the second derivative
            }

            return sum;
        }


        /// <summary>
        /// Restarts the line by clearing all points and destroying the reference points.
        /// </summary>
        private void RestartLine()
        {
            referencePoints.Clear();
            workSnapshots.Clear();
            line.positionCount = 0;
            pointCount = 0;
            
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

            line.SetPosition(line.positionCount-1 , newPosition.transform.localPosition);

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


        private void addReferencePointFromCursor()
        {
            label.text = userPointer.transform.position.ToString();

            GameObject g = Instantiate(referencePointPrefab, userPointer.transform.position, userPointer.transform.rotation);    
            g.transform.SetParent(simulationParent, true);
            g.transform.localScale = simulation.transform.localScale * 0.05f;

            pointCount++;
            line.positionCount = pointCount + 1;

            line.SetPosition(pointCount, g.transform.localPosition);
            if (pointCount == 1)
            {
                line.SetPosition(0, g.transform.localPosition);
            } else
            {
                g.transform.rotation = Quaternion.LookRotation(line.GetPosition(pointCount) - line.GetPosition(pointCount - 1));
            }

            Destroy(g);
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


