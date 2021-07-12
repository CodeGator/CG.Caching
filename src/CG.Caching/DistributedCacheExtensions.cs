using CG.Validations;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IDistributedCache"/>
    /// type.
    /// </summary>
    public static partial class DistributedCacheExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method sets the specified object into the cache.
        /// </summary>
        /// <typeparam name="T">The type of object to use for the operation.</typeparam>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="obj">The object reference to use for the operation.</param>
        /// <param name="options">The caching options to use for the operation.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>A task to perform the operation.</returns>
        public static async Task SetAsync<T>(
            this IDistributedCache cache,
            string key,
            T obj,
            DistributedCacheEntryOptions options,
            CancellationToken token = default
            ) where T : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key))
                .ThrowIfNull(obj, nameof(obj));

            // Convert the object to JSON.
            var json = JsonSerializer.Serialize(obj);

            // Convert the JSON to bytes.
            var bytes = Encoding.UTF8.GetBytes(json, 0, json.Length);

            // Cache the bytes.
            await cache.SetAsync(
                key,
                bytes, 
                options, 
                token
                ).ConfigureAwait(false);
        }

        // *******************************************************************

        /// <summary>
        /// This method retrieves the specified cached object.
        /// </summary>
        /// <typeparam name="T">The type of object to use for the operation.</typeparam>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the specified
        /// cached object.</returns>
        public static async Task<T> GetAsync<T>(
            this IDistributedCache cache,
            string key,
            CancellationToken token = default
            ) where T : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key));

            // Get the cached bytes.
            var bytes = await cache.GetAsync(
                key, 
                token
                ).ConfigureAwait(false);

            // Did we fail?
            if (null == bytes)
            {
                return null; // Wasn't in the cache.
            }

            // Convert the bytes to JSON.
            var json = Encoding.UTF8.GetString(bytes);

            // Re hydrate the object.
            var obj = JsonSerializer.Deserialize<T>(json);

            // Return the results.
            return obj;
        }

        #endregion
    }
}
