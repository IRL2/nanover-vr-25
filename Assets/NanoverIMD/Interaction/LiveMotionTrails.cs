using System;
using System.Collections.Generic;
using Nanover.Visualisation;
using NanoverImd;
using UnityEngine;

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


    //[SerializeField]
    //float lineWidth = 0.07f; // Width of the line segments

    [SerializeField]
    private LineRenderer line;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //line = gameObject.AddComponent<LineRenderer>();
        //line.startWidth = lineWidth;
        //line.endWidth = lineWidth;
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

    public void Update()
    {
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
            }
        }
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
        work_array.Add((float)(double)work);
    }
}
