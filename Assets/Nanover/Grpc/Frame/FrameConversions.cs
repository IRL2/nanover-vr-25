using System;
using Nanover.Core.Math;
using Nanover.Core.Science;
using Nanover.Frame;
using Nanover.Protocol;
using UnityEngine;

namespace Nanover.Grpc.Frame
{
    /// <summary>
    /// Conversion methods for changing between Frame-specific GRPC values and C#
    /// objects.
    /// </summary>
    public static class FrameConversions
    {
        /// <summary>
        /// Convert a protobuf <see cref="ValueArray" /> to an array of
        /// <see cref="BondPair" />.
        /// </summary>
        public static BondPair[] ToBondPairArray(this ValueArray valueArray)
        {
            if (valueArray.ValuesCase != ValueArray.ValuesOneofCase.IndexValues)
                throw new ArgumentException("ValueArray is of wrong type");

            var bondValues = valueArray.IndexValues.Values;

            if (bondValues.Count % 2 > 0)
                throw new ArgumentException("Odd number of indices for bond array");

            var bondCount = bondValues.Count / 2;

            var bondArray = new BondPair[bondCount];
            for (var i = 0; i < bondCount; i++)
            {
                bondArray[i].A = (int) bondValues[2 * i];
                bondArray[i].B = (int) bondValues[2 * i + 1];
            }

            return bondArray;
        }

        /// <summary>
        /// Convert a protobuf <see cref="ValueArray" /> to an array of
        /// <see cref="Element" />.
        /// </summary>
        public static Element[] ToElementArray(this ValueArray valueArray)
        {
            if (valueArray.ValuesCase != ValueArray.ValuesOneofCase.IndexValues)
                throw new ArgumentException("ValueArray is of wrong type");

            var indexArray = valueArray.IndexValues.Values;

            var elementCount = indexArray.Count;
            var elementArray = new Element[elementCount];

            for (var i = 0; i < elementCount; i++)
                elementArray[i] = (Element) indexArray[i];

            return elementArray;
        }

        /// <summary>
        /// Convert a protobuf <see cref="ValueArray" /> to a <see cref="LinearTransformation"/>.
        /// </summary>
        public static LinearTransformation ToLinearTransformation(this ValueArray valueArray)
        {
            if (valueArray.ValuesCase != ValueArray.ValuesOneofCase.FloatValues)
                throw new ArgumentException("ValueArray is of wrong type");

            var floatArray = valueArray.FloatValues.Values;

            if (floatArray.Count != 9)
                throw new ArgumentException(
                    "Incorrect number of floats to specify a linear transformation");

            return new LinearTransformation(
                new Vector3(floatArray[0], floatArray[1], floatArray[2]),
                new Vector3(floatArray[3], floatArray[4], floatArray[5]),
                new Vector3(floatArray[6], floatArray[7], floatArray[8]));
        }
    }
}