
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
        /// This method sets the specified string into the cache.
        /// </summary>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="value">The string to use for the operation.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>A task to perform the operation.</returns>
        public static async ValueTask SetAsync(
            this IDistributedCache cache,
            string key,
            string value,
            CancellationToken token = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key));

            // Convert the value to bytes.
            var bytes = Encoding.UTF8.GetBytes(
                value,
                0,
                value.Length
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
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>A task to perform the operation.</returns>
        public static async ValueTask SetAsync(
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

            // Did we fail?
            if (string.IsNullOrEmpty(json))
            {
                // Panic!!
                throw new JsonException(
                    $"The object failed to serialize for key: '{key}'"
                    );
            }

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
        /// This method sets the specified string into the cache.
        /// </summary>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="value">The value to use for the operation.</param>
        /// <param name="entryOptions">The caching entry options to use for 
        /// the operation.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>A task to perform the operation.</returns>
        public static async ValueTask SetAsync(
            this IDistributedCache cache,
            string key,
            string value,
            DistributedCacheEntryOptions entryOptions,
            CancellationToken token = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key));

            // Convert the value to bytes.
            var bytes = Encoding.UTF8.GetBytes(
                value,
                0,
                value.Length
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
        public static async ValueTask SetAsync(
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

            // Did we fail?
            if (string.IsNullOrEmpty(json))
            {
                // Panic!!
                throw new JsonException(
                    $"The object failed to serialize for key: '{key}'"
                    );
            }

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
        public static async ValueTask<T> GetAsync<T>(
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
            if (bytes is null || !bytes.Any())
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

        // *******************************************************************

        /// <summary>
        /// This method attempts to get the object associated with the given
        /// key. If the key is not found, the <paramref name="setDelegate"/>
        /// is called to create the object, which is then set in the cache using
        /// the <paramref name="key"/> value.
        /// </summary>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="setDelegate">The delegate to call in the event the
        /// key does not belong to the cache.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>The value of the object associated with the given key.</returns>
        public static async ValueTask<byte[]> GetOrSetAsync(
            this IDistributedCache cache,
            string key,
            Func<byte[]> setDelegate,
            CancellationToken token = default
            ) 
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key))
                .ThrowIfNull(setDelegate, nameof(setDelegate));

            // Call the overload.
            var bytes = await cache.GetAsync(
                key,
                token
                ).ConfigureAwait(false);

            // Did we fail?
            if (bytes is null || !bytes.Any())
            {
                // Create the data.
                bytes = setDelegate.Invoke();

                // Set the data in the cache.
                await cache.SetAsync(
                    key, 
                    bytes
                    ).ConfigureAwait(false);  
            }

            // Return the results.
            return bytes;
        }

        // *******************************************************************

        /// <summary>
        /// This method attempts to get the string associated with the given
        /// key. If the key is not found, the <paramref name="setDelegate"/>
        /// is called to create the string, which is then set in the cache using
        /// the <paramref name="key"/> value.
        /// </summary>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="options">The cache options to use for the operation.</param>
        /// <param name="setDelegate">The delegate to call in the event the
        /// key does not belong to the cache.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>The value of the string associated with the given key.</returns>
        public static async ValueTask<string> GetOrSetAsync(
            this IDistributedCache cache,
            string key,
            DistributedCacheEntryOptions options,
            Func<string> setDelegate,
            CancellationToken token = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key))
                .ThrowIfNull(options, nameof(options))
                .ThrowIfNull(setDelegate, nameof(setDelegate));

            // Call the overload.
            var bytes = await cache.GetAsync(
                key,
                token
                ).ConfigureAwait(false);

            var value = "";

            // Did we fail?
            if (bytes is null || !bytes.Any())
            {
                // Create the value.
                value = setDelegate.Invoke();

                // Convert to bytes.
                bytes = Encoding.UTF8.GetBytes(value);

                // Set the bytes in the cache.
                await cache.SetAsync(
                    key,
                    bytes,
                    options
                    ).ConfigureAwait(false);
            }
            else
            {
                // Convert to a string.
                value = Encoding.UTF8.GetString(bytes);
            }

            // Return the results.
            return value;
        }

        // *******************************************************************

        /// <summary>
        /// This method attempts to get the bytes associated with the given
        /// key. If the key is not found, the <paramref name="setDelegate"/>
        /// is called to create the object, which is then set in the cache using
        /// the <paramref name="key"/> value.
        /// </summary>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="options">The cache options to use for the operation.</param>
        /// <param name="setDelegate">The delegate to call in the event the
        /// key does not belong to the cache.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>The value of the bytes associated with the given key.</returns>
        public static async ValueTask<byte[]> GetOrSetAsync(
            this IDistributedCache cache,
            string key,
            DistributedCacheEntryOptions options,
            Func<byte[]> setDelegate,
            CancellationToken token = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key))
                .ThrowIfNull(options, nameof(options))
                .ThrowIfNull(setDelegate, nameof(setDelegate));

            // Call the overload.
            var bytes = await cache.GetAsync(
                key,
                token
                ).ConfigureAwait(false);

            // Did we fail?
            if (bytes is null || !bytes.Any())
            {
                // Create the data.
                bytes = setDelegate.Invoke();

                // Did we fail?
                if (bytes is null || !bytes.Any())
                {
                    // Panic!!
                    throw new JsonException(
                        $"The delegate failed to return data for key: '{key}'"
                        );
                }

                // Set the data in the cache.
                await cache.SetAsync(
                    key,
                    bytes,
                    options
                    ).ConfigureAwait(false);
            }

            // Return the results.
            return bytes;
        }

        // *******************************************************************

        /// <summary>
        /// This method attempts to get the string associated with the given
        /// key. If the key is not found, the <paramref name="setDelegate"/>
        /// is called to create the string, which is then set in the cache using
        /// the <paramref name="key"/> value.
        /// </summary>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="setDelegate">The delegate to call in the event the
        /// key does not belong to the cache.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>The value of the string associated with the given key.</returns>
        public static async ValueTask<string> GetOrSetAsync(
            this IDistributedCache cache,
            string key,
            Func<string> setDelegate,
            CancellationToken token = default
            ) 
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key))
                .ThrowIfNull(setDelegate, nameof(setDelegate));

            // Call the overload.
            var bytes = await cache.GetAsync(
                key,
                token
                ).ConfigureAwait(false);

            var value = "";

            // Did we fail?
            if (bytes is null || !bytes.Any())
            {
                // Create the value.
                value = setDelegate.Invoke();

                // Convert to bytes.
                bytes = Encoding.UTF8.GetBytes(value);

                // Set the bytes in the cache.
                await cache.SetAsync(
                    key,
                    bytes
                    ).ConfigureAwait(false);
            }
            else
            {
                // Convert to a string.
                value = Encoding.UTF8.GetString(bytes);
            }            

            // Return the results.
            return value;
        }

        // *******************************************************************

        /// <summary>
        /// This method attempts to get the object associated with the given
        /// key. If the key is not found, the <paramref name="setDelegate"/>
        /// is called to create the object, which is then set in the cache using
        /// the <paramref name="key"/> value.
        /// </summary>
        /// <typeparam name="T">The type of associated object.</typeparam>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="setDelegate">The delegate to call in the event the
        /// key does not belong to the cache.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>The value of the object associated with the given key.</returns>
        public static async ValueTask<T> GetOrSetAsync<T>(
            this IDistributedCache cache,
            string key,
            Func<T> setDelegate,
            CancellationToken token = default
            ) where T : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key))
                .ThrowIfNull(setDelegate, nameof(setDelegate));

            // Call the overload.
            var obj = await cache.GetAsync<T>(
                key,
                token
                ).ConfigureAwait(false);

            // Did we fail?
            if (obj is null)
            {
                // Create the object.
                obj = setDelegate.Invoke();

                // Did we fail?
                if (obj is null)
                {
                    // Panic!!
                    throw new JsonException(
                        $"The delegate returned NULL for key: '{key}'"
                        );
                }

                // Set the object in the cache.
                await cache.SetAsync(
                    key,
                    obj
                    ).ConfigureAwait(false);
            }

            // Return the results.
            return obj;
        }

        // *******************************************************************

        /// <summary>
        /// This method attempts to get the object associated with the given
        /// key. If the key is not found, the <paramref name="setDelegate"/>
        /// is called to create the object, which is then set in the cache using
        /// the <paramref name="key"/> value.
        /// </summary>
        /// <typeparam name="T">The type of associated object.</typeparam>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="options">The cache options to use for the operation.</param>
        /// <param name="setDelegate">The delegate to call in the event the
        /// key does not belong to the cache.</param>
        /// <param name="token">A cancellation token that is monitored throughout
        /// the lifetime of the method.</param>
        /// <returns>The value of the object associated with the given key.</returns>
        public static async ValueTask<T> GetOrSetAsync<T>(
            this IDistributedCache cache,
            string key,
            DistributedCacheEntryOptions options,
            Func<T> setDelegate,
            CancellationToken token = default
            ) where T : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(key, nameof(key))
                .ThrowIfNull(options, nameof(options))
                .ThrowIfNull(setDelegate, nameof(setDelegate));

            // Call the overload.
            var obj = await cache.GetAsync<T>(
                key,
                token
                ).ConfigureAwait(false);

            // Did we fail?
            if (obj is null)
            {
                // Create the object.
                obj = setDelegate.Invoke();

                // Did we fail?
                if (obj is null)
                {
                    // Panic!!
                    throw new JsonException(
                        $"The delegate returned NULL for key: '{key}'"
                        );
                }

                // Set the object in the cache.
                await cache.SetAsync(
                    key,
                    obj,
                    options
                    ).ConfigureAwait(false);
            }

            // Return the results.
            return obj;
        }

        #endregion
    }
}
