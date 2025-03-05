using System;
using System.Collections.Generic;
using NSubstitute.Extensions;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Hands.Samples.VisualizerSample;


public class HandPinchManager : MonoBehaviour
{
    XRHandSubsystem m_HandSubsystem;
    HandProcessor handProcessor;

    [SerializeField]
    private float minPinchDistance = 0.02f; // Minimum distance for pinch (in meters)
    [SerializeField]
    private float maxPinchDistance = 0.1f;  // Maximum distance for pinch (in meters)


    void Start()
    {
        handProcessor = this.gameObject.GetComponent<HandProcessor>();

        if (handProcessor != null)
        {
            m_HandSubsystem = handProcessor.m_Subsystem;

            if (m_HandSubsystem != null)
            {
                m_HandSubsystem.updatedHands += OnUpdatedHands;
                ConfigurePinchGesture();
                Debug.Log("Hand subsystem initialized and running in HandPinchManager.");
            }
            else
            {
                Debug.LogError("Failed to get hand subsystem from HandProcessor.");
            }
        }
        else
        {
            Debug.LogError("HandProcessor not found in the scene.");
        }
    }

    void OnUpdatedHands(XRHandSubsystem subsystem,
        XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
        XRHandSubsystem.UpdateType updateType)
    {
        float pinchValue = 0f;
        switch (updateType)
        {
            case XRHandSubsystem.UpdateType.Dynamic:
                m_HandSubsystem.rightHandCommonGestures.TryGetPinchValue(out pinchValue);
                Debug.Log("Pinch value: " + pinchValue);
                break;
            case XRHandSubsystem.UpdateType.BeforeRender:
                m_HandSubsystem.rightHandCommonGestures.TryGetPinchValue(out pinchValue);
                Debug.Log("Pinch value: " + pinchValue);
                break;
        }
    }

    private void Update()
    {
        if (m_HandSubsystem != null && m_HandSubsystem.running)
        {
            if (!pinchEventRegistered)
            {
                m_HandSubsystem.rightHandCommonGestures.pinchValueUpdated += OnPinchValueUpdated;
                m_HandSubsystem.rightHandCommonGestures.pinchPoseUpdated += OnPinchPoseUpdated;
                ConfigurePinchGesture();
                pinchEventRegistered = true;
            }
            else
            {
                XRHandJoint index = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.IndexTip);
                XRHandJoint thumb = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.ThumbTip);

                float distance = Vector3.Distance(index.TryGetPose(out Pose indexPose) ? indexPose.position : Vector3.zero,
                    thumb.TryGetPose(out Pose thumbPose) ? thumbPose.position : Vector3.zero);

                Debug.Log("Distance between index and thumb: " + distance);
            }
            //float pinchValue = 0f;
            //if (m_HandSubsystem.rightHandCommonGestures.TryGetPinchValue(out pinchValue))
            //{
            //    Debug.Log("Pinch value in Update: " + pinchValue);
            //}
            //else
            //{
            //    //Debug.LogWarning("Failed to get pinch value in Update.");
            //}
        }
        else
        {
            //Debug.LogWarning("Hand subsystem is not initialized or not running.");
            m_HandSubsystem = handProcessor.m_Subsystem;
        }
    }

    bool pinchEventRegistered = false;
    private void OnPinchValueUpdated(XRCommonHandGestures.PinchValueUpdatedEventArgs args)
    {
        float pinchValue = 0.0f;
        args.TryGetPinchValue(out pinchValue);
        Debug.Log("Pinch value in event: " + pinchValue);
    }

    private void OnPinchPoseUpdated(XRCommonHandGestures.PinchPoseUpdatedEventArgs args)
    {
        Pose pinchPose;
        args.TryGetPinchPose(out pinchPose);
        Debug.Log(args.handedness + " pinch pose: " + pinchPose.position + " " + pinchPose.rotation);
        //Debug.Log("Pinch pose updated.");
    }

    void ConfigurePinchGesture()
    {
        var fingerShapeConfig = new XRFingerShapeConfiguration
        {
            minimumPinchDistance = minPinchDistance,
            maximumPinchDistance = maxPinchDistance
        };
        //HandPinchManager hpm = new HandPinchManager();
        //HandPinchManager hpm = m_HandSubsystem.gameObject.AddComponent<HandPinchManager>();

        //hpm.m_HandSubsystem = m_HandSubsystem;
        //hpm.minPinchDistance = minPinchDistance;
        //hpm.maxPinchDistance = maxPinchDistance;

        //hpm.m_HandSubsystem.rightHandCommonGestures.Configure<>(fingerShapeConfig);
        //m_HandSubsystem.rightHandCommonGestures.Configure<>

        //m_HandSubsystem.rightHandCommonGestures.Configure<hpm>(fingerShapeConfig);
        //m_HandSubsystem.leftHandCommonGestures.ConfigurePinchGesture(fingerShapeConfig);
    }
}


