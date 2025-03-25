using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Nanover.Core.Collections;
using UnityEngine;

namespace Nanover.Visualisation.Utility
{
    /// <summary>
    /// A collection of <see cref="ComputeBuffer" />'s, which can be assigned to by
    /// directly passing arrays.
    /// </summary>
    public class ComputeBufferCollection : IDisposable, IReadOnlyDictionary<string, ComputeBuffer>
    {
        /// <summary>
        /// Internal list of managed <see cref="ComputeBuffer" />'s.
        /// </summary>
        private readonly ObservableDictionary<string, ComputeBuffer> computeBuffers =
            new ObservableDictionary<string, ComputeBuffer>();

        /// <summary>
        /// Stores the list of buffer names of managed <see cref="ComputeBuffer" />'s which
        /// are dirty.
        /// </summary>
        private readonly DictionaryDirtyState<string, ComputeBuffer> dirtyState;

        public ComputeBufferCollection()
        {
            dirtyState = new DictionaryDirtyState<string, ComputeBuffer>(computeBuffers);
        }

        /// <summary>
        /// Dispose of all <see cref="ComputeBuffer" />'s.
        /// </summary>
        public void Dispose()
        {
            foreach (var buffer in computeBuffers.Values)
                buffer.Dispose();
            computeBuffers.Clear();
        }

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}" />
        public IEnumerator<KeyValuePair<string, ComputeBuffer>> GetEnumerator()
        {
            return computeBuffers.GetEnumerator();
        }

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}" />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}" />
        public int Count => computeBuffers.Count;

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}" />
        public bool ContainsKey(string key)
        {
            return computeBuffers.ContainsKey(key);
        }

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}" />
        public bool TryGetValue(string bufferName, out ComputeBuffer value)
        {
            return computeBuffers.TryGetValue(bufferName, out value);
        }

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}" />
        public ComputeBuffer this[string bufferName] => computeBuffers[bufferName];

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}" />
        public IEnumerable<string> Keys => computeBuffers.Keys;

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}" />
        public IEnumerable<ComputeBuffer> Values => computeBuffers.Values;

        /// <summary>
        /// Passes an array of data as <paramref name="content" /> to a renderer, where it
        /// is uploaded as a
        /// <see cref="ComputeBuffer" /> with the provided <paramref name="bufferName" />
        /// as an identifier.
        /// </summary>
        public void SetBuffer<T>(string bufferName, T[] content) where T : struct
        {
            if (content.Length == 0)
                return;
            
            EnsureComputeBufferExists(bufferName, content.Length, Marshal.SizeOf(typeof(T)));

            computeBuffers[bufferName].SetData(content);
            dirtyState.MarkDirty(bufferName);
        }

        /// <summary>
        /// Ensure that a compute buffer exists with the given properties.
        /// </summary>
        /// <param name="bufferName">
        /// Identifying name of the <see cref="ComputeBuffer" />
        /// </param>
        /// <param name="length">
        /// Number of elements in the <see cref="ComputeBuffer" />
        /// </param>
        /// <param name="stride">
        /// Size of each individual element of the
        /// <see cref="ComputeBuffer" />
        /// </param>
        private void EnsureComputeBufferExists(string bufferName, int length, int stride)
        {
            computeBuffers.TryGetValue(bufferName, out var buffer);
            if (buffer != null && length == buffer.count && stride == buffer.stride)
                return;
            // Either no existing buffer, or existing is wrong length/stride
            buffer?.Dispose();
            computeBuffers[bufferName] = new ComputeBuffer(length, stride);
        }

        /// <summary>
        /// Enumerate over all buffers which have been updated since the last call to clear
        /// dirty buffers.
        /// </summary>
        public IEnumerable<(string Id, ComputeBuffer Buffer)> GetDirtyBuffers()
        {
            return dirtyState.DirtyKeyValuePairs.Select(kvp => (kvp.Key, kvp.Value));
        }

        /// <summary>
        /// Clear all dirty flags.
        /// </summary>
        public void ClearDirtyBuffers()
        {
            dirtyState.ClearAllDirty();
        }

        /// <summary>
        /// Apply all buffers (dirty or otherwise) to <paramref name="shader" />.
        /// </summary>
        public void ApplyAllBuffersToShader(IGpuProgram shader)
        {
            foreach (var pair in computeBuffers)
                shader.SetBuffer(pair.Key, pair.Value);
        }

        /// <summary>
        /// Apply all dirty buffers to <paramref name="shader" />.
        /// </summary>
        public void ApplyDirtyBuffersToShader(IGpuProgram shader)
        {
            foreach (var (id, buffer) in GetDirtyBuffers())
                shader.SetBuffer(id, buffer);
        }

        public void Clear()
        {
            computeBuffers.Clear();
        }

        public void RemoveBuffer(string key)
        {
            if (computeBuffers.TryGetValue(key, out var buffer))
            {
                buffer.Dispose();
                computeBuffers.Remove(key);
            }
        }
    }
}