using System;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Calculator
{
    [Serializable]
    public class FloatLerpNode : LerpNode<float, FloatArrayProperty>
    {
        [SerializeField]
        private FloatProperty speed = new FloatProperty()
        {
            Value = 1f
        };

        private float Delta =>  Application.isPlaying ? speed * Time.deltaTime : 999f;

        protected override float MoveTowards(float current, float target)
        {
            return Mathf.MoveTowards(current, target, Delta);
        }
    }
}