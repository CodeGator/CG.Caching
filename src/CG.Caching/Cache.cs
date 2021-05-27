using CG.Validations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Caching
{
    /// <summary>
    /// This class is a default implementation of the <see cref="ICache"/>
    /// interface.
    /// </summary>
    public class Cache : DisposableBase, ICache
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a memory cache.
        /// </summary>
        protected IMemoryCache MemoryCache { get; set; }

        /// <summary>
        /// This property contains a reference to a distributed cache.
        /// </summary>
        protected IDistributedCache DistributedCache { get; set; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="Cache"/>
        /// class.
        /// </summary>
        /// <param name="cache">The cache to use with the service.</param>
        public Cache(
            IMemoryCache cache
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache));

            // Save the reference.
            MemoryCache = cache;
        }

        // *******************************************************************

        /// <summary>
        /// This constructor creates a new instance of the <see cref="Cache"/>
        /// class.
        /// </summary>
        /// <param name="cache">The cache to use with the service.</param>
        public Cache(
            IDistributedCache cache
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache));

            // Save the reference.
            DistributedCache = cache;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method puts an object of type <typeparamref name="T"/>
        /// into the cache, using the provided key.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="key">The key of the cache entry to match.</param>
        /// <param name="obj">The object to be placed into the cache.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation.</returns>
        public virtual async Task SetAsync<T>(
            string key,
            T obj,
            CancellationToken cancellationToken = default
            ) where T : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(obj, nameof(obj))
                .ThrowIfNullOrEmpty(key, nameof(key));

            // Seserialize the object into JSON.
            var json = JsonSerializer.Serialize(obj);

            // Convert the string into UTF bytes.
            var bytes = Encoding.UTF8.GetBytes(json);

            // Use the proper cache implementation.
            if (null != MemoryCache)
            {
                // Create the cache entry.
                using (var entry = MemoryCache.CreateEntry(key))
                {
                    // Set the size for the entry.
                    entry.SetSize(bytes.Length);

                    // Put the bytes into the entry.
                    entry.Value = bytes;
                }
            }
            else
            {
                // Put the entry into the cache.
                await DistributedCache.SetAsync(
                    key,
                    bytes,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method retrieves an object of type <typeparamref name="T"/>
        /// from the cache, provided a match is found.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="key">The name of the cache entry to match.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation, that returns an object 
        /// of type <typeparamref name="T"/> if a match is found, or NULL 
        /// otherwise.</returns>
        public virtual async Task<T> GetAsync<T>(
            string key,
            CancellationToken cancellationToken = default
            ) where T : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNullOrEmpty(key, nameof(key));

            byte[] buffer;

            // Use the proper cache implementation.
            if (null != MemoryCache)
            {
                // Look for the entry in the cache.
                buffer = MemoryCache.Get(key) as byte[];
            }
            else
            {
                // Look for the entry in the cache.
                buffer = await DistributedCache.GetAsync(
                    key,
                    cancellationToken
                    ).ConfigureAwait(false);
            }

            // Convert the bytes to JSON.
            var json = Encoding.UTF8.GetString(buffer);

            // Convert the json to an object of type T.
            var obj = JsonSerializer.Deserialize<T>(json);

            // Return the result.
            return obj;
        }

        #endregion

        // *******************************************************************
        // Protected methods.
        // *******************************************************************

        #region Protected methods

        /// <summary>
        /// This method is called to dispose of any managed resources.
        /// </summary>
        /// <param name="disposing">True to cleanup managed resources; false otherwise.</param>
        protected override void Dispose(bool disposing)
        {
            // Should we cleanup managed resources?
            if (disposing)
            {
                MemoryCache.Dispose();
                MemoryCache = null;
            }

            // Give the base class a chance.
            base.Dispose(disposing);
        }

        #endregion

    }
}
