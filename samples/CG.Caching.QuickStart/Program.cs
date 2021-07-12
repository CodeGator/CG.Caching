using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CG.Caching.QuickStart
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appSettings.json");
            var configuration = builder.Build();

            serviceCollection.AddDistributedCaching(
                configuration.GetSection("Services:Caching")
                );

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var cache = serviceProvider.GetRequiredService<IDistributedCache>();

            var example1 = new Example()
            {
                A = "1",
                B = "2"
            };
            
            await cache.SetAsync(
                "foo", 
                example1,
                new DistributedCacheEntryOptions() { }
                ).ConfigureAwait(false);

            var example2 = await cache.GetAsync<Example>(
                "foo"
                ).ConfigureAwait(false);

            // TODO : verify that example2 is equal to example1.
        }
    }

    class Example
    {
        public string A { get; set; }
        public string B { get; set; }
    }
}
