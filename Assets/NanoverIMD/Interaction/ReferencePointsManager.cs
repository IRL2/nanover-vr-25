using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nanover.Visualisation;
using NanoverImd;
using UnityEngine;
using UnityEngine.XR;
using Nanover.Frontend.Input;
using Nanover.Frontend.XR;

using Text = TMPro.TextMeshProUGUI;
using TMPro;
using UnityEngine.UIElements;
using Nanover.Frontend.Controllers;

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

        [SerializeField]
        /// <summary> item in the hierarchy that will be used to parent for the added reference points</summary>
        private Transform simulationParent;


        [SerializeField]
        private GameObject referencePointPrefab;

        [SerializeField]
        private TextMeshPro label;

        [SerializeField]
        private LineRenderer line;

        private IButton secondaryButton;
        bool secondaryButtonPrevPressed = false;

        private IButton triggerButton;
        bool triggerButtonPrevPressed = false;

        private IButton primaryButton;
        bool primaryButtonPrevPressed = false;

        private IButton grabButton;
        bool grabButtonPrevPressed = false;



        private void Start()
        {
            primaryButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.primaryButton);
            secondaryButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.secondaryButton);

            triggerButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.triggerButton);
            grabButton = InputDeviceCharacteristics.Right.WrapUsageAsButton(CommonUsages.gripButton);

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
                return;
            }

            label.text = "pointer at " + userPointer.transform.position.ToString();

            if (secondaryButton.IsPressed)
            {
                RestartLine();
                return;
            }

            else if (primaryButton.IsPressed)
            {
                label.text += "\nline contains " + line.positionCount + " points";
                label.text += "\n[drawing]";

                if (!primaryButtonPrevPressed)
                {
                    primaryButtonPrevPressed = true;
                    drawingElapsedTime = snapshotFrequency;
                    RestartLine();
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
            }
            else if (primaryButtonPrevPressed)
            {
                line.Simplify(0.02f);
                UnityEngine.Debug.Log("Finish drawing. Simplifiying the line");
            }

            primaryButtonPrevPressed = primaryButton.IsPressed;
            secondaryButtonPrevPressed = secondaryButton.IsPressed;
            triggerButtonPrevPressed = triggerButton.IsPressed;
            grabButtonPrevPressed = grabButton.IsPressed;
        }

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
    
        private void DragLastPointOnLine(Transform position)
        {
            if (line.positionCount <= 1) return;

            GameObject empty = new GameObject("Empty");
            Transform newPosition = empty.transform;
            newPosition.position = position.position;

            newPosition.position = userPointer.transform.position;
            newPosition.SetParent(simulationParent, true);

            line.SetPosition(line.positionCount-1 , newPosition.transform.localPosition);

            Destroy(empty);
        }


        private void AddReferencePoint(Transform position)
        {
            //label.text = userPointer.transform.position.ToString();

            GameObject empty = new GameObject("Empty");
            Transform newPosition = empty.transform;
            newPosition.position = position.position;

            newPosition.position = userPointer.transform.position;
            newPosition.SetParent(simulationParent, true);

            line.positionCount = line.positionCount + 1;

            line.SetPosition(line.positionCount - 1, newPosition.localPosition);

            if (line.positionCount == 1)
            {
                line.SetPosition(0, newPosition.localPosition);
            }

            Destroy(empty);
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


