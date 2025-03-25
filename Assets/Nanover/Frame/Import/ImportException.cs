using System;

namespace Nanover.Frame.Import
{
    /// <summary>
    /// Exception thrown when parsing fails for a given line.
    /// </summary>
    public class ImportException : Exception
    {
        public ImportException(string line, int lineNumber, Exception e = null) : base(
            $"Failed to parse line {lineNumber} [{line}]", e)
        {
        }
    }
}