using System.Collections.Generic;
using UnityEngine;

using Nanover.Visualisation;
using NanoverImd;

using UnityEngine.XR;
using Nanover.Frontend.XR;

using TMPro;


public class InteractionTrailsManager : MonoBehaviour
{
    [SerializeField] private LineManager lineManager;
    [SerializeField] private NanoverImdSimulation simulation;
    [SerializeField] private SynchronisedFrameSource frameSource;
    [SerializeField] private Transform simulationParent;
    [SerializeField] private TextMeshPro infoLabel;

    private int currentLineIndex = -1;
    private int? lastAtomIndex;
    private float? lastFrameIndex = 0;
    private Vector3? lastPosition = Vector3.zero;
    private float? lastWork = 0.0f;
    private float deltaWork = 0.0f;
    private List<float> workSnapshots = new();
    private bool hasHaptics = false;
    private UnityEngine.XR.InputDevice rightHandDevice;
    private UnityEngine.XR.HapticCapabilities hapticCapabilities;

    private Nanover.Frontend.Input.IButton yButton;
    private bool yButtonPrevPressed = false;

    private float[] colorPalette = new float[]
    {
        0.55f, 0.58f, 0.60f, 0.88f, 0.90f, 0.92f, 0.95f, 0.97f, 0.99f
    };
    private int currentColorIndex = 0;
    private float currentColorHue = 0.5f;

    void Start()
    {
        // Haptics setup
        rightHandDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
        rightHandDevice.TryGetHapticCapabilities(out hapticCapabilities);
        hasHaptics = hapticCapabilities.supportsImpulse;

        yButton = InputDeviceCharacteristics.Left.WrapUsageAsButton(CommonUsages.secondaryButton);
        yButtonPrevPressed = yButton.IsPressed;
    }

    void Update()
    {
        if (!hasHaptics)
        {
            rightHandDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
            rightHandDevice.TryGetHapticCapabilities(out hapticCapabilities);
            hasHaptics = hapticCapabilities.supportsImpulse;
            if (hasHaptics)
                rightHandDevice.SendHapticImpulse(0, 0.5f, 0.1f);
            else
                return;
        }

        if (yButton.IsPressed && !yButtonPrevPressed)
        {
            // Reset the current line and work snapshots
            if (currentLineIndex >= 0)
            {
                lineManager.ResetLine(currentLineIndex);
                currentLineIndex = Mathf.Max(currentLineIndex--, 0);
                lastFrameIndex = 0.0f;
            }
            UpdateInfo();
        }

        if (simulation == null || frameSource == null) return;
        ProcessFrameData();

        yButtonPrevPressed = yButton.IsPressed;
    }

    private void ProcessFrameData()
    {
        if (frameSource.CurrentFrame == null) return;
        var data = frameSource.CurrentFrame.Data;

        int? atomIndex = GetSelectedAtomIndex(data);
        if (atomIndex == null) return;
        lastAtomIndex = atomIndex;

        Vector3? newPosition = GetPositionFromAtom(atomIndex.Value);
        if (newPosition != null) lastPosition = newPosition;

        float? currentWork = GetCurrentWork(data);
        if (currentWork != null) lastWork = currentWork;

        if (newPosition.HasValue && newPosition.Value.magnitude > 0)
        {
            RegisterCurrentWork(lastWork, ref workSnapshots);

            float? frameIndex = GetFrameTimestamp(data);
            if (frameIndex == null) return;

            // Start a new line if needed (e.g., on new interaction)
            if (currentLineIndex == -1 || frameIndex - lastFrameIndex > 0.9f)
            {
                Debug.Log($" started a new line at frame ${lastFrameIndex}");

                currentLineIndex = lineManager.CreateNewLine(LineManager.SOLID_LINE);
                currentColorHue = (currentColorHue + 0.1f) % 1.0f;
                lineManager.SetLineColor(currentLineIndex,
                                         Color.HSVToRGB(currentColorHue, 0.85f, 0.75f));
            }

            lastFrameIndex = frameIndex;

            lineManager.AddPointToLine(currentLineIndex, newPosition.Value);
            UpdateInfo();
        }
    }

    private void UpdateInfo()
    {
        if (infoLabel == null) return;
        var line = lineManager.GetLineRenderer(currentLineIndex);
        if (line == null) return;
        float length = lineManager.GetLineLength(currentLineIndex);
        int numPoints = line.positionCount;
        float lineSmoothnessA = LineManager.CalculateAngularSmoothness(line) / Mathf.PI;
        float lineSmoothnessB = LineManager.CalculateSmoothness(line);
        float f = Mathf.Abs(deltaWork / 50);
        rightHandDevice.SendHapticImpulse(0, f, 0.01f);

        string info = $"<u>trajectory trail line</u>\n" +
                        $"lenght is {length:F2} nm\n" +
                        $"having {numPoints} points\n" +
                        $"from {line.GetPosition(0):F2}\n" +
                        $"to {line.GetPosition(line.positionCount - 1):F2}\n" +
                        $"angular triplets {(lineSmoothnessA * 100):F1}%\n" +
                        $"path jagger is {lineSmoothnessB:F2}\n\n" +
                        $"<u>system information</u>\nrelative work:{deltaWork} \n" +
                        $"last interaction atom is #{lastAtomIndex} \n";
        infoLabel.text = info;
    }

    private int? GetSelectedAtomIndex(IDictionary<string, object> data)
    {
        if (data.TryGetValue("forces.user.index", out var capturedSelectedAtoms))
        {
            if (capturedSelectedAtoms is uint[] selectedAtoms && selectedAtoms.Length > 0)
                return (int)selectedAtoms[0];
        }
        return null;
    }

    private float? GetFrameTimestamp(IDictionary<string, object> data)
    {
        if (data.TryGetValue("server.timestamp", out var frameIndex))
        {
            return (float)(double)frameIndex;
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
        }
        return null;
    }

    private void RegisterCurrentWork(float? work, ref List<float> work_array)
    {
        if (work == null) return;
        if (work_array.Count > 1)
            deltaWork = work_array[^1] - (float)work;
        work_array.Add((float)(double)work);
    }
}
