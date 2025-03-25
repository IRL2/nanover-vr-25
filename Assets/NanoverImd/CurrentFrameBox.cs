using Nanover.Frame;
using Nanover.Frame.Event;
using Nanover.Visualisation;
using UnityEngine;
using UnityEngine.Assertions;

namespace NanoverImd
{
    /// <summary>
    /// Updates a <see cref="BoxVisualiser" /> with the simulation box of a
    /// <see cref="Frame" />.
    /// </summary>
    public class CurrentFrameBox : MonoBehaviour
    {
        /// <summary>
        /// Source of the <see cref="Frame" />.
        /// </summary>
        [SerializeField]
        private SynchronisedFrameSource frameSource;

        /// <summary>
        /// The <see cref="BoxVisualiser" /> that will render the box.
        /// </summary>
        [SerializeField]
        private BoxVisualiser boxVisualiser;

        private void Start()
        {
            Assert.IsNotNull(boxVisualiser);
            Assert.IsNotNull(frameSource);

            boxVisualiser.enabled = false;

            frameSource.FrameChanged += OnFrameChanged;
        }

        /// <summary>
        /// Callback for when the frame is updated.
        /// </summary>
        private void OnFrameChanged(IFrame frame, FrameChanges changes)
        {
            if (changes.HasChanged(StandardFrameProperties.BoxTransformation.Key))
            {
                var box = (frame as Frame)?.BoxVectors;
                if (box == null)
                {
                    boxVisualiser.enabled = false;
                }
                else
                {
                    boxVisualiser.enabled = true;
                    boxVisualiser.SetBox(box.Value);
                }
            }
        }
    }
}