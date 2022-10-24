
using System;

namespace CG.Caching.Extensions
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
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="obj">The object reference to use for the operation.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>A task to perform the operation.</returns>
        public static async Task SetAsync(
            this IDistributedCache cache,
            string key,
            object obj,
            CancellationToken token = default
            ) 
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key))
                .ThrowIfNull(obj, nameof(obj));

            // Convert the object to JSON.
            var json = JsonSerializer.Serialize(
                obj,
                new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

            // Convert the JSON to bytes.
            var bytes = Encoding.UTF8.GetBytes(
                json,
                0,
                json.Length
                );

            // Cache the bytes.
            await cache.SetAsync(
                key,
                bytes,
                token
                ).ConfigureAwait(false);
        }

        // *******************************************************************

        /// <summary>
        /// This method sets the specified object into the cache.
        /// </summary>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="obj">The object reference to use for the operation.</param>
        /// <param name="entryOptions">The caching entry options to use for 
        /// the operation.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>A task to perform the operation.</returns>
        public static async Task SetAsync(
            this IDistributedCache cache,
            string key,
            object obj,
            DistributedCacheEntryOptions entryOptions,
            CancellationToken token = default
            ) 
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key))
                .ThrowIfNull(entryOptions, nameof(entryOptions))
                .ThrowIfNull(obj, nameof(obj));

            // Convert the object to JSON.
            var json = JsonSerializer.Serialize(
                obj,
                new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

            // Convert the JSON to bytes.
            var bytes = Encoding.UTF8.GetBytes(
                json, 
                0, 
                json.Length
                );

            // Cache the bytes.
            await cache.SetAsync(
                key,
                bytes,
                entryOptions,
                token
                ).ConfigureAwait(false);
        }

        // *******************************************************************

        /// <summary>
        /// This method gets the specified object from the cache.
        /// </summary>
        /// <typeparam name="T">The type of object to use for the operation.</typeparam>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
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
            var json = Encoding.UTF8.GetString(
                bytes
                );

            // Re hydrate the object.
            var obj = JsonSerializer.Deserialize<T>(
                json,
                new JsonSerializerOptions() 
                { 
                    AllowTrailingCommas = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

            // Return the results.
            return obj;
        }

        #endregion
    }
}
