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

namespace NanoverImd.Interaction
{
    public class ReferencePointsManager : MonoBehaviour
    {
        [SerializeField]
        private NanoverImdSimulation simulation;

        [SerializeField]
        private SynchronisedFrameSource frameSource;


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
        }

        int pointCount = 0;
        private List<Vector3> referencePoints = new List<Vector3>();
        private List<float> workSnapshots = new List<float>();

        public float snapshotFrequency = 2f;
        private float drawingElapsedTime = 0.0f;

        private void Update()
        {
            if (secondaryButton.IsPressed)
            {
                RestartLine();
                return;
            }

            if (triggerButton.IsPressed)
            {
                if (!triggerButtonPrevPressed)
                {
                    triggerButtonPrevPressed = true;
                    drawingElapsedTime = snapshotFrequency;
                    RestartLine();
                    ProcessFrameData();
                    UnityEngine.Debug.Log("first enter :" + referencePoints.Count + " > " + drawingElapsedTime);
                }

                drawingElapsedTime += Time.deltaTime;
                if (drawingElapsedTime >= snapshotFrequency)
                {
                    drawingElapsedTime = 0.0f;
                    ProcessFrameData();
                }
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

        private int? lastAtomIndex = -1;
        private Vector3? lastPosition = Vector3.zero;
        private float? lastWork = 0.0f;

        private void ProcessFrameData()
        {
            if (frameSource.CurrentFrame == null) return;

            int? atomIndex = GetSelectedAtomIndex();

            if (atomIndex == null) return;

            if (atomIndex < 60 && atomIndex > 64) return;

            lastAtomIndex = atomIndex;

            Vector3? newPosition = GetPositionFromAtom(atomIndex.Value);
            if (newPosition != null) { lastPosition = newPosition; }

            float? currentWork = GetCurrentWork();
            if (currentWork != null) { lastWork = currentWork; }

            if (newPosition.HasValue)
            {
                //RegisterReferencePoint(newPosition.Value, atomIndex.Value, currentWork);
                RegisterCurrentWork(lastWork, ref workSnapshots);
                //update line renderer

                AddReferencePoint(newPosition.Value, atomIndex.Value);
                label.text = "selected atom: #" + lastAtomIndex.ToString() + "\n" + 
                             "position: " + newPosition.ToString() + "\n" + 
                             "systems work: " + lastWork.ToString();
            }
        }

        private int? GetSelectedAtomIndex()
        {
            if (frameSource.CurrentFrame.Data.TryGetValue("forces.user.index", out var capturedSelectedAtoms))
            {
                if (capturedSelectedAtoms is uint[] selectedAtoms && selectedAtoms.Length > 0)
                {
                    return (int)selectedAtoms[0];
                }
            }
            return null;
        }

        private void RegisterCurrentWork(float? work, ref List<float> work_array)
        {
            if (work == null) return;
            work_array.Add((float)(double)work);
        }

        private float? GetCurrentWork()
        {
            if (frameSource.CurrentFrame.Data.TryGetValue("forces.user.work_done", out var capturedWork))
            {
                return (float)(double)capturedWork;
                //if (capturedWork is float[] work && work.Length > 0)
                //{
                //    return work[0];
                //}
            }
            return null;
        }

        private Vector3? GetPositionFromAtom(int atomIndex)
        {
            if (frameSource.CurrentFrame.Data.TryGetValue("particle.positions", out var capturedParticlePositons))
            {
                if (capturedParticlePositons is Vector3[] particlePositons && particlePositons.Length > 0)
                {
                    return particlePositons[atomIndex];
                }
            }
            return null;
        }

        private void AddReferencePoint(Vector3 newPosition, int atomIndex)
        {

            GameObject g = Instantiate(referencePointPrefab, simulationParent);
            g.transform.localPosition = newPosition;

            referencePoints.Add(newPosition);

            UpdateLineRender();
            return;

            pointCount++;
            line.positionCount = pointCount + 1;

            line.SetPosition(pointCount, g.transform.localPosition);
            if (pointCount == 1)
            {
                line.SetPosition(0, g.transform.localPosition);
            }
        }

        
        private void UpdateLineRender()
        {
            Vector3[] pnts = referencePoints.ToArray();
            line.positionCount = pnts.Length;
            line.SetPositions(pnts);
            line.colorGradient = GetShortGradientFromArray(workSnapshots.ToArray());
        }


        /// <summary>
        /// Get a 7 point gradient from an N floats  array
        /// </summary>
        /// <param name="workSnaps">will clamp the values between 0 and 10,000</param>
        /// <returns></returns>
        private Gradient GetShortGradientFromArray(float[] workSnaps)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[7];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[7];

            int segmentSize = workSnaps.Length / 7;
            for (int i = 0; i < 7; i++)
            {
                float segmentSum = 0;
                int segmentStart = i * segmentSize;
                int segmentEnd = (i == 6) ? workSnaps.Length : segmentStart + segmentSize;

                for (int j = segmentStart; j < segmentEnd; j++)
                {
                    segmentSum += Mathf.Clamp(workSnaps[j], 0f, 10000f);
                }

                float segmentAverage = segmentSum / (segmentEnd - segmentStart);

                float hue = segmentAverage.Remap(0.0f, 10000f, 0.0f, 1.0f);
                UnityEngine.Debug.Log("step" + i +  " hue: " + hue + " > " + segmentAverage);
                //float hue = Mathf.Lerp(0.0f, 1.0f, segmentAverage);

                colorKeys[i] = new GradientColorKey(Color.HSVToRGB(hue, 1.0f, 1.0f), (float)i / 7);
                alphaKeys[i] = new GradientAlphaKey(1.0f, (float)i / 7);
            }

            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        private void RegisterReferencePoint(Vector3 newPosition, int atomIndex, float? work)
        {
            GameObject g = Instantiate(referencePointPrefab, simulationParent);
            g.transform.localPosition = newPosition;

            referencePoints.Add(newPosition);

            pointCount++;
            line.positionCount = pointCount + 1;

            line.SetPosition(pointCount, g.transform.localPosition);
            if (pointCount == 1)
            {
                line.SetPosition(0, g.transform.localPosition);
            }
        }

        private void addReferencePoint(Vector3 position)
        {
            label.text = position.ToString();

            GameObject g = Instantiate(referencePointPrefab, simulationParent);
            g.transform.localPosition = position;

            pointCount++;
            line.positionCount = pointCount + 1;

            line.SetPosition(pointCount, g.transform.localPosition);
            if (pointCount == 1)
            {
                line.SetPosition(0, g.transform.localPosition);
            }
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