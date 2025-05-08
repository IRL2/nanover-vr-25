namespace Nanover.Visualisation.Node
{
    /// <summary>
    /// A generic structure for a visualisation node.
    /// </summary>
    public abstract class GenericOutputNode
    {
        /// <summary>
        /// Is the current input valid and can it create a valid output?
        /// </summary>
        protected abstract bool IsInputValid { get; }
        
        /// <summary>
        /// Are any of the inputs dirty?
        /// </summary>
        protected abstract bool IsInputDirty { get; }

        /// <summary>
        /// Clear the dirty state of all inputs.
        /// </summary>
        protected abstract void ClearDirty();

        /// <summary>
        /// Update the output based upon the input.
        /// </summary>
        protected abstract void UpdateOutput();

        /// <summary>
        /// Clear the output to a default when the input is invalid.
        /// </summary>
        protected abstract void ClearOutput();

        /// <summary>
        /// Refresh the node.
        /// </summary>
        public void Refresh()
        {
            if (IsInputDirty)
            {
                if (IsInputValid)
                    UpdateOutput();
                else
                    ClearOutput();
                ClearDirty();
            }
        }
    }
}