// Copyright (c) 2016 Mark Wonnacott. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Nanover.Frontend.Utility
{
    /// <summary>
    /// Utility class for creating and managing a pool of instances of some 
    /// object.
    /// </summary>
    /// <remarks>
    /// The purpose of pooling is to avoid creating new instances from scratch 
    /// when the number of instances may be fluctuating frequently and instance
    /// creation has some non-neglible cost. 
    /// </remarks>
    public sealed class IndexedPool<TInstance>
    {
        /// <summary>
        /// The number of active instances.
        /// </summary>
        public int ActiveInstanceCount { get; private set; }

        /// <summary>
        /// Return the instance as the given index. There are always at least
        /// `ActiveInstanceCount` instances.
        /// </summary>
        public TInstance this[int index] => instances[index];

        /// <summary>
        /// Raised when an instance is about to be used.
        /// </summary>
        public event Action<TInstance> InstanceActivated;

        /// <summary>
        /// Raised when a previously used instance becomes unused.
        /// </summary>
        public event Action<TInstance> InstanceDeactivated;

        /// <summary>
        /// Callbacks for creating a new instance from scratch.
        /// </summary>
        private readonly Func<TInstance> createInstanceCallback;

        /// <summary>
        /// Callback for activating a previously deactivated instance.
        /// </summary>
        private readonly Action<TInstance> activateInstanceCallback;

        /// <summary>
        /// Callback for deactivating a previously active instance.
        /// </summary>
        private readonly Action<TInstance> deactivateInstanceCallback;

        /// <summary>
        /// The store of existing instances, including inactive instances that 
        /// may be reused later if necessary.
        /// </summary>
        private List<TInstance> instances = new List<TInstance>();

        /// <summary>
        /// Create a pool with a given callback for creating new instances,
        /// and optionally callbacks for activating and deactivating instances
        /// when them switch in and out of use.
        /// </summary>
        public IndexedPool(Func<TInstance> createInstanceCallback,
                           Action<TInstance> activateInstanceCallback = null,
                           Action<TInstance> deactivateInstanceCallback = null)
        {
            this.createInstanceCallback = createInstanceCallback;
            this.activateInstanceCallback = activateInstanceCallback;
            this.deactivateInstanceCallback = deactivateInstanceCallback;
        }

        /// <summary>
        /// Ensure an exact number of instances are active, creating new 
        /// instances if necessary, otherwise reusing existing instances and 
        /// deactivating surplus instances.
        /// </summary>
        public void SetActiveInstanceCount(int count)
        {
            for (int i = instances.Count; i < count; ++i)
                instances.Add(createInstanceCallback());

            for (int i = ActiveInstanceCount; i < count; ++i)
            {
                activateInstanceCallback?.Invoke(instances[i]);
                InstanceActivated?.Invoke(instances[i]);
            }

            for (int i = count; i < instances.Count; ++i)
            {
                deactivateInstanceCallback?.Invoke(instances[i]);
                InstanceDeactivated?.Invoke(instances[i]);
            }

            ActiveInstanceCount = count;
        }

        /// <summary>
        /// Make sure that there are exactly as many instances as configs given
        /// then run a mapping function pairing every config to its own 
        /// instance.
        /// </summary>
        public void MapConfig<TConfig>(IEnumerable<TConfig> configs, 
                                       Action<TConfig, TInstance> mapConfigToInstance)
        {
            int count = 0;

            foreach (var config in configs)
            {
                if (instances.Count <= count)
                    instances.Add(createInstanceCallback());

                mapConfigToInstance(config, instances[count]);
                count += 1;
            }

            SetActiveInstanceCount(count);
        }

        /// <summary>
        /// Set the count and then run a mapping function over every active
        /// instance.
        /// </summary>
        public void MapIndex(int count, Action<int, TInstance> mapIndexToInstance)
        {
            SetActiveInstanceCount(count);

            for (int i = 0; i < ActiveInstanceCount; ++i)
            {
                mapIndexToInstance(i, instances[i]);
            }
        }
    }
}
