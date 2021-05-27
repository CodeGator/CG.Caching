using System;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Caching
{
    /// <summary>
    /// This interface represents an object that provides caching services.
    /// </summary>
    public interface ICache : IDisposable
    {
        /// <summary>
        /// This method puts an object of type <typeparamref name="T"/>
        /// into the cache, using the provided key.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="key">The key of the cache entry to match.</param>
        /// <param name="obj">The object to be placed into the cache.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation.</returns>
        Task SetAsync<T>(
            string key,
            T obj,
            CancellationToken cancellationToken = default
            ) where T : class;

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
        Task<T> GetAsync<T>(
            string key,
            CancellationToken cancellationToken = default
            ) where T : class;
    }
}
