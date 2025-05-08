using UnityEngine;
using UnityEngine.Assertions;

using UnityEngine.XR;
using Nanover.Frontend.XR;

namespace Nanover.Frontend.Controllers
{
    /// <summary>
    /// Reacts to a change in the SteamVR device name and loads the appropriate prefab.
    /// </summary>
    public class SteamVrDynamicController : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private InputDeviceCharacteristics characteristics;

        [SerializeField]
        private Transform prefabRoot;

        [SerializeField]
        private VrController controller;
#pragma warning restore 0649

        private VrControllerPrefab currentPrefab;
        private string prevType;

        private void Awake()
        {
            Assert.IsNotNull(controller);
            Assert.IsNotNull(prefabRoot);
        }

        private void Update()
        {
            UpdateDevice();
        }

        private void UpdateDevice()
        {
            var device = characteristics.GetFirstDevice();

            if (device.isValid && device.name != prevType)
            {
                OnControllerTypeChanged(device.name, device);
                prevType = device.name;
            }
        }

        private void OnControllerTypeChanged(string type, InputDevice device)
        {
            if (currentPrefab != null)
                Destroy(currentPrefab);

            var definition = SteamVrControllerDefinition.GetControllerDefinition(type);
            if (definition == null)
            {
                Debug.LogWarning($"Unsupported controller type: {type}, using fallback.");
                definition = SteamVrControllerDefinition.GetControllerDefinition("null");
            }

            var prefab = definition.GetPrefab(device);

            if (prefab == null)
            {
                Debug.LogWarning($"Controller type '{type}' is missing prefab for {device.name}. Controller will be unusable.");
                currentPrefab = null;
            }
            else
            {
                currentPrefab = Instantiate(prefab, prefabRoot ?? transform);
            }

            controller.ResetController(currentPrefab);
        }
    }
}