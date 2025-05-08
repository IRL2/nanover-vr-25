using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Nanover.Frontend.UI
{
    /// <summary>
    /// Layout group for arranging UI elements radially around a point
    /// </summary>
    public class RadialLayoutGroup : LayoutGroup
    {
        [SerializeField]
        private float angularOffset = 0;

        private void ArrangeChildrenRadially()
        {
            var children = rectChildren;
            var da = 360f / children.Count;
            for (var i = 0; i < children.Count; i++)
            {
                var angle = angularOffset + i * da;
                var d = Vector2.Scale(new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle),
                                                  Mathf.Cos(Mathf.Deg2Rad * angle)),
                                      GetComponent<RectTransform>().sizeDelta * 0.5f);
                children[i].transform.localPosition = d;
            }
        }

        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
            ArrangeChildrenRadially();
        }
    }
}