using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

namespace Nanover.Frontend.Controllers
{
    /// <summary>
    /// Defines for a given SteamVR controller type the prefabs to use for each hand.
    /// These provide the pivots for where to draw tools, as well as where to position
    /// UI over the physical buttons of the controller.
    /// </summary>
    [CreateAssetMenu(menuName = "SteamVR Controller Definition", fileName = "controller")]
    public class SteamVrControllerDefinition : ScriptableObject
    {
        [Serializable]
        private struct ControllerDefinition
        {
#pragma warning disable 0649
            [SerializeField]
            [Tooltip("Prefab to place at the position of the controller.")]
            internal VrControllerPrefab prefab;

            [SerializeField]
            [Tooltip("Should the default SteamVR model be hidden.")]
            internal bool hideSteamVrModel;
#pragma warning restore 0649
        }

#pragma warning disable 0649
        [SerializeField]
        [Tooltip("Internal SteamVR controller ID.")]
        private string controllerIdRegex;

        [SerializeField]
        [Tooltip("How to handle the left controller.")]
        private ControllerDefinition leftController;

        [SerializeField]
        [Tooltip("How to handle the right controller.")]
        private ControllerDefinition rightController;
#pragma warning restore 0649

        /// <summary>
        /// Get the controller definition for a given SteamVR id.
        /// </summary>
        public static SteamVrControllerDefinition GetControllerDefinition(string id)
        {
            return Resources.LoadAll<SteamVrControllerDefinition>("Controllers")
                            .FirstOrDefault(type => Regex.IsMatch(id, type.controllerIdRegex));
        }

        /// <summary>
        /// Get the prefab for the given input source.
        /// </summary>
        public VrControllerPrefab GetPrefab(InputDevice device)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Left))
                return leftController.prefab;
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Right))
                return rightController.prefab;
            return null;
        }
    }
}
