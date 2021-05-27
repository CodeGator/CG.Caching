using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CG.Caching.QuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appSettings.json");
            var configuration = builder.Build();

            serviceCollection.AddCaching(
                configuration.GetSection("Services:Caching")
                );

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var cache = serviceProvider.GetRequiredService<ICache>();

            var example1 = new Example()
            {
                A = "1",
                B = "2"
            };
            cache.SetAsync("foo", example1).Wait();
            var example2 = cache.GetAsync<Example>("foo").Result;

            // TODO : verify that example2 is equal to example1.
        }
    }

    class Example
    {
        public string A { get; set; }
        public string B { get; set; }
    }

}
