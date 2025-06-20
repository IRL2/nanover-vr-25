using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [SerializeField] private GameObject dashLinePrefab;
    [SerializeField] private GameObject solidLinePrefab;
    [SerializeField] private Transform simulationParent;

    // types of lines
    public const int SOLID_LINE = 0;
    public const int DASH_LINE = 1;

    private readonly List<LineRenderer> lines = new();
    private readonly List<List<Vector3>> linePoints = new();

    public int CreateNewLine(int? type = SOLID_LINE)
    {
        var lineObj = Instantiate(type== DASH_LINE ? dashLinePrefab : solidLinePrefab, gameObject.transform);
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        lines.Add(lineRenderer);
        linePoints.Add(new List<Vector3>());
        return lines.Count - 1;
    }

    public void AddPointToLine(int lineIndex, Vector3 point)
    {
        if (lineIndex < 0 || lineIndex >= lines.Count) return;
        linePoints[lineIndex].Add(point);
        lines[lineIndex].positionCount = linePoints[lineIndex].Count;
        lines[lineIndex].SetPositions(linePoints[lineIndex].ToArray());
    }

    public void DragLastPoint(int lineIndex, Vector3 point)
    {
        if (lineIndex < 0 || lineIndex >= lines.Count) return;
        if (linePoints[lineIndex].Count == 0) return;
        linePoints[lineIndex][^1] = point;
        lines[lineIndex].SetPosition(linePoints[lineIndex].Count - 1, point);
    }

    public void ResetLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= lines.Count) return;
        linePoints[lineIndex].Clear();
        lines[lineIndex].positionCount = 0;
    }

    public void RemoveAllLines()
    {
        foreach (var line in lines)
            Destroy(line.gameObject);
        lines.Clear();
        linePoints.Clear();
    }

    public float GetLineLength(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= lines.Count) return 0f;
        float length = 0f;
        var points = linePoints[lineIndex];
        for (int i = 0; i < points.Count - 1; i++)
            length += Vector3.Distance(points[i], points[i + 1]);
        return length;
    }

    public static float CalculateSmoothness(LineRenderer lineRenderer)
    {
        int pointCount = lineRenderer.positionCount;
        if (pointCount < 3) return 0f;
        Vector3[] positions = new Vector3[pointCount];
        lineRenderer.GetPositions(positions);
        float sum = 0f;
        for (int i = 0; i < pointCount - 2; i++)
        {
            Vector3 secondDiff = positions[i + 2] - 2 * positions[i + 1] + positions[i];
            sum += secondDiff.sqrMagnitude;
        }
        return sum;
    }

    public static float CalculateAngularSmoothness(LineRenderer lineRenderer)
    {
        int pointCount = lineRenderer.positionCount;
        if (pointCount < 3) return 0f;
        Vector3[] positions = new Vector3[pointCount];
        lineRenderer.GetPositions(positions);
        float totalDeviation = 0f;
        int angleCount = 0;
        for (int i = 1; i < pointCount - 1; i++)
        {
            Vector3 prev = positions[i] - positions[i - 1];
            Vector3 next = positions[i + 1] - positions[i];
            if (prev.sqrMagnitude == 0f || next.sqrMagnitude == 0f) continue;
            float dot = Vector3.Dot(prev.normalized, next.normalized);
            dot = Mathf.Clamp(dot, -1f, 1f);
            float angle = Mathf.Acos(dot);
            float deviation = Mathf.Abs(Mathf.PI - angle);
            totalDeviation += deviation;
            angleCount++;
        }
        return (angleCount > 0) ? totalDeviation / angleCount : 0f;
    }

    public LineRenderer GetLineRenderer(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= lines.Count) return null;
        return lines[lineIndex];
    }

    public void SetLineColor(int lineIndex, UnityEngine.Color color)
    {
        if (lineIndex < 0 || lineIndex >= lines.Count) return;
        lines[lineIndex].startColor = color;
        lines[lineIndex].endColor = color;
    }
}