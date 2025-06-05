using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nanover.Visualisation;
using NanoverImd;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class LiveMotionTrails : MonoBehaviour
{
    [SerializeField]
    private NanoverImdSimulation simulation;

    [SerializeField]
    private SynchronisedFrameSource frameSource;

    [SerializeField]
    /// <summary> item in the hierarchy that will be used to parent for the added reference points</summary>
    private Transform simulationParent;


    [SerializeField]
    private TextMeshPro infoLabel;

    [SerializeField]
    private GameObject referencePointPrefab;

    private float lenght = 0.0f; // Length of the line segment
    public float Lenght { get => lenght; set => lenght = value; }


    private int numPoints = 0; // Number of points in the line
    public int NumPoints { get => numPoints; set => numPoints = value; }


    int pointCount = 0;
    private List<Vector3> referencePoints = new List<Vector3>();
    private List<float> workSnapshots = new List<float>();


    private int? lastAtomIndex;
    private Vector3? lastPosition = Vector3.zero;
    private float? lastWork = 0.0f;
    private float deltaWork = 0.0f; // Difference in work done between two points

    private float lineSmoothnessA = 0.5f; // Angular smoothness index (0-1)
    private float lineSmoothnessB = 0.5f; // Path jagger index (0-1)

    //[SerializeField]
    //float lineWidth = 0.07f; // Width of the line segments

    [SerializeField]
    private LineRenderer line;
    private InputDevice rightHandDevice;
    private HapticCapabilities hapticCapabilities;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //line = gameObject.AddComponent<LineRenderer>();
        //line.startWidth = lineWidth;
        //line.endWidth = lineWidth;

        //Disable();
    }

    public void Enable()
    {
        line.enabled = true;
    }
    public void Disable()
    {
        ResetLine();
        line.enabled = false;
    }


    public void SetPoints(Vector3[] points)
    {
        if (points == null || points.Length < 2)
        {
            Debug.LogWarning("Not enough points to draw a line.");
            return;
        }
        line.positionCount = points.Length;
        line.SetPositions(points);
    }


    public Vector3? GetPoint(int index)
    {
        if (index < 0 || index >= line.positionCount)
        {
            Debug.LogWarning("Index out of bounds.");
            return null;
        }
        return line.GetPosition(index);
    }

    public void ResetLine()
    {
        referencePoints.Clear();
        workSnapshots.Clear();
        line.positionCount = 0;
        NumPoints = 0;
        lenght = 0.0f;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag: "ReferencePoints");
        foreach (GameObject g in gameObjects)
        {
            Destroy(g);
        }

        infoLabel.text = string.Empty;
    }

    public float CalculateLineLenght()
    {
        if (line.positionCount < 2)
        {
            Debug.LogWarning("Not enough points to calculate length.");
            return 0.0f;
        }
        float totalLength = 0.0f;
        for (int i = 0; i < line.positionCount - 1; i++)
        {
            totalLength += Vector3.Distance(line.GetPosition(i), line.GetPosition(i + 1));
        }
        return totalLength;
    }

    bool hasHaptics = false;
    public void Update()
    {
        if (!hasHaptics)
        {
            rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            rightHandDevice.TryGetHapticCapabilities(out hapticCapabilities);
            if (!hapticCapabilities.supportsImpulse)
            {
                Debug.LogWarning("Haptic feedback is not supported on this device.");
                 return;
            } else
            {
                Debug.Log("Haptic feedback is supported on this device.");
                hasHaptics = true;
                rightHandDevice.SendHapticImpulse(0, 0.5f, 0.1f); // Test haptic feedback
                //rightHandDevice.SendHapticImpulse(1, 0.5f, 0.1f); // Test haptic feedback
            }

        }


        if (line.enabled)
        {
            if (simulation == null || frameSource == null)
            {
                Debug.LogWarning("Simulation or FrameSource is not set.");
                return;
            }
            ProcessFrameData();
            // Update the line renderer with the current points from the simulation
            //Vector3[] points = 
            //if (points != null && points.Length > 0)
            //{
            //    SetPoints(points);
            //    Lenght = CalculateLineLenght();
            //    NumPoints = points.Length;
            //}
        }
    }


    private void AddReferencePoint(Vector3 newPosition, int atomIndex)
    {
        //Transform pos = transform;
        //pos.SetParent(simulationParent, false);
        //pos.transform.localPosition = newPosition;

        GameObject g = Instantiate(referencePointPrefab, simulationParent);
        g.transform.localPosition = newPosition;

        referencePoints.Add(newPosition);

        UpdateLineRender();
        
        Destroy(g);

        return;

        pointCount++;
        line.positionCount = pointCount;

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
        //line.colorGradient = GetShortGradientFromArray(workSnapshots.ToArray());
    }


    private void ProcessFrameData()
    {
        if (frameSource.CurrentFrame == null) return;
        IDictionary<string, object> data = frameSource.CurrentFrame.Data;

        int? atomIndex = GetSelectedAtomIndex(data);

        if (atomIndex == null) return;

        //if (atomIndex < 60 && atomIndex > 64) return;

        lastAtomIndex = atomIndex;

        Vector3? newPosition = GetPositionFromAtom(atomIndex.Value);
        if (newPosition != null) { lastPosition = newPosition; }

        float? currentWork = GetCurrentWork(data);
        if (currentWork != null) { lastWork = currentWork; } // prevent form updating the last work if it is null

        if (newPosition.HasValue)
        {
            if (newPosition?.magnitude > 0)
            {
                //RegisterReferencePoint(newPosition.Value, atomIndex.Value, currentWork);
                RegisterCurrentWork(lastWork, ref workSnapshots);

                AddReferencePoint(newPosition.Value, atomIndex.Value);

                UpdateInfo();
            }
        }
    }

    private void UpdateInfo()
    {
        if (infoLabel == null) return;

        lenght = CalculateLineLenght();
        NumPoints = line.positionCount;
        lineSmoothnessA = CalculateAngularSmoothness(line) / Mathf.PI;
        lineSmoothnessB = CalculateSmoothness(line);


        //float f = MMM((float)lastWork, 400, 1000, 0.0f, 0.5f);
        float f = Mathf.Abs(deltaWork/ 50);
        rightHandDevice.SendHapticImpulse(0, f, 0.01f); // Test haptic feedback


        string info = $"<u>trajectory trail line</u>\n" +
                      $"lenght is {lenght.ToString("F2")} nm\n" +
                      $"having {NumPoints} points\n" +
                      $"from {line.GetPosition(0).ToString("F2")}\n" +
                      $"to {line.GetPosition(line.positionCount - 1).ToString("F2")}\n" +
                      $"angular triplets {(lineSmoothnessA * 100).ToString("F1")}%\n" +
                      $"path jagger is {lineSmoothnessB.ToString("F2")}\n\n" +
                      $"<u>system information</u>\nrelative work:{deltaWork} \n" +
                      $"last interaction atom is #{lastAtomIndex} \n";

        infoLabel.text = info;
    }

    private float MMM(float value, float from1, float to1, float from2, float to2)
    {
        return ((value - from1) / (to1 - from1) * (to2 - from2) + from2);
    }

    private int? GetSelectedAtomIndex(IDictionary<string, object> data)
    {
        if (data.TryGetValue("forces.user.index", out var capturedSelectedAtoms))
        {
            if (capturedSelectedAtoms is uint[] selectedAtoms && selectedAtoms.Length > 0)
            {
                return (int)selectedAtoms[0];
            }
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

    private float? GetCurrentWork(IDictionary<string, object> data)
    {
        if (data.TryGetValue("forces.user.work_done", out var capturedWork))
        {
            return (float)(double)capturedWork;
            //if (capturedWork is float[] work && work.Length > 0)
            //{
            //    return work[0];
            //}
        }
        return null;
    }

    private void RegisterCurrentWork(float? work, ref List<float> work_array)
    {
        if (work == null) return;
        if (work_array.Count() > 1)
        {
            deltaWork = work_array.LastOrDefault<float>() - (float)work;
        }
        work_array.Add((float)(double)work);
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
}
