using UnityEngine;

namespace Nanover.Frontend.Controllers
{
    public class NanoverRenderModel : MonoBehaviour
    {
        [SerializeField]
        private VrController controller;

        private void OnTransformChildrenChanged()
        {
            UpdateColor();
        }

        private void UpdateColor()
        {
            if (this == null)
                return;
            foreach (var renderer in this.GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = color;
            }
        }

        private Color color = Color.white;

        /// <summary>
        /// Set the color of the current controller. The alpha channel of this color is used
        /// to fade out a controller.
        /// </summary>
        public void SetColor(Color color)
        {
            this.color = color;
            UpdateColor();
        }
    }
}