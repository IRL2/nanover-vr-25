using Nanover.Frontend.Controllers;
using Nanover.Frontend.XR;
using NanoverImd;
using UnityEngine;
using UnityEngine.XR;

public class InteractionStrengthController : MonoBehaviour
{
    [SerializeField]
    private NanoverImdSimulation simulation;

    [SerializeField]
    private VrController controller;

    [SerializeField]
    private float maximumInteractionStrength;

    [SerializeField]
    private float minimumInteractionStrength;

    [SerializeField]
    private float scaling;

    private void Update()
    {
        var joystick = InputDeviceCharacteristics.Right.GetFirstDevice().GetJoystickValue(CommonUsages.primary2DAxis) ?? Vector2.zero;

        var increase = joystick.x > .5f;
        var decrease = joystick.x < -.5f;

        var change = 0f;
        if (increase)
            change++;
        if (decrease)
            change--;
        if (change != 0)
        {
            change = Mathf.Pow(scaling, change * Time.deltaTime);
            Scale = Mathf.Clamp(Scale * change,
                                minimumInteractionStrength,
                                maximumInteractionStrength);

            controller.PushNotification($"{(int) Scale}x");
        }
    }

    private float Scale
    {
        get => simulation.ManipulableParticles.ForceScale;
        set => simulation.ManipulableParticles.ForceScale = value;
    }
}