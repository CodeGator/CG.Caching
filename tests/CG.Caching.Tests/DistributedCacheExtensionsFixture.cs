
namespace Microsoft.Extensions.Caching.Distributed
{
    /// <summary>
    /// This class is a test fixture for the <see cref="DistributedCacheExtensions"/>
    /// class.
    /// </summary>
    [TestClass]
    public class DistributedCacheExtensionsFixture
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method ensures the <see cref="DistributedCacheExtensions.GetOrSetAsync{T}(string, Func{T}, CancellationToken)"/>
        /// method calls the delegate when the key doesn't exist in the cache,
        /// sets the value in the cache, then returns the results.
        /// </summary>
        /// <returns>A task to perform the operation.</returns>
        [TestMethod]
        public async Task DistributedCache_GetOrSetAsync()
        {
            // Arrange ...
            var value1 = new byte[]
                   {
                       0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
                       16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31
                   };

            var value2 = new byte[]
                   {
                       0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
                   };

            var cache = new Mock<IDistributedCache>();

            cache.Setup(x => x.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                )).Verifiable();

            cache.Setup(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
                )).Verifiable();

            // Act ...
            var result = await cache.Object.GetOrSetAsync<Tuple<byte[], byte[]>>(
               "TESTKEY",
               () => new Tuple<byte[], byte[]>(
                   value1,
                   value2
                   )
               ).ConfigureAwait(false);

            // Assert ...
            Assert.IsTrue(
                result.Item1.Length == value1.Length,
                "The first item was invalid!"
                );
            Assert.IsTrue(
                result.Item2.Length == value2.Length,
                "The second item was invalid!"
                );

            cache.Verify();
        }

        #endregion
    }
}
