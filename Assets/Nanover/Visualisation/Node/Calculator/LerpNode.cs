using System;
using Nanover.Visualisation.Node;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Calculator
{
    public abstract class LerpNode<TValue, TProperty> : GenericOutputNode where TProperty : ArrayProperty<TValue>, new()
    {
        [SerializeField]
        private TProperty input = new TProperty();
        
        private TProperty output = new TProperty();

        private TValue[] cached = new TValue[0];

        protected override bool IsInputValid => input.HasNonNullValue();
        protected override bool IsInputDirty => input.IsDirty;
        protected override void ClearDirty()
        {
            input.IsDirty = false;
        }

        protected override void UpdateOutput()
        {
            var input = this.input.Value;
            
            if (cached.Length != input.Length)
            {
                output.Resize(input.Length);
                Array.Resize(ref cached, input.Length);
                Array.Copy(input, cached, input.Length);
            }
            
            for (var i = 0; i < input.Length; i++)
            {
                cached[i] = MoveTowards(cached[i], input[i]);
            }

            output.Value = cached;
        }

        protected abstract TValue MoveTowards(TValue current, TValue target);

        protected override void ClearOutput()
        {
            output.UndefineValue();
        }
    }
}