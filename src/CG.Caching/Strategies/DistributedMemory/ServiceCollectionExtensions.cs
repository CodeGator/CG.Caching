using CG.Validations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CG.Caching.Strategies.Distributed
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// type.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method registers a distributed caching service with the specified 
        /// service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <param name="serviceLifetime">The service lifetime to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> 
        /// parameter, for chaining calls together.</returns>
        public static IServiceCollection AddDistributedMemoryCachingStrategies(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            ServiceLifetime serviceLifetime
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Register the options.
            serviceCollection.ConfigureOptions<MemoryDistributedCacheOptions>(
                configuration
                );

            // Register the ASP.NET cache service.
            serviceCollection.AddDistributedMemoryCache();

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
