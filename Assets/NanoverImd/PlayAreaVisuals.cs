using Nanover.Core.Math;
using Nanover.Frontend.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NanoverImd
{
    public class PlayAreaVisuals : MonoBehaviour
    {
        [SerializeField]
        private NanoverImdApplication application;
        [SerializeField]
        private LineRenderer rendererTemplate;

        private IndexedPool<LineRenderer> rendererPool;

        private void Start()
        {
            rendererPool = new IndexedPool<LineRenderer>(
                () => Instantiate(rendererTemplate, transform),
                transform => transform.gameObject.SetActive(true),
                transform => transform.gameObject.SetActive(false)
            );
        }

        private void Update()
        {
            rendererPool.MapConfig(application.Simulation.Multiplayer.PlayAreas.Values, (playarea, renderer) =>
            {
                renderer.positionCount = 4;
                renderer.SetPosition(0, TransformPlayAreaPoint(playarea.A));
                renderer.SetPosition(1, TransformPlayAreaPoint(playarea.B));
                renderer.SetPosition(2, TransformPlayAreaPoint(playarea.C));
                renderer.SetPosition(3, TransformPlayAreaPoint(playarea.D));
            });

            Vector3 TransformPlayAreaPoint(Vector3 point)
            {
                return application.CalibratedSpace.TransformPoseCalibratedToWorld(
                    new Transformation(point, Quaternion.identity, Vector3.one))
                    .Position;
            }
        }
    }
}
