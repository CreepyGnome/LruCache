using AutoFixture;
using AutoFixture.Xunit2;
using LruCache;
using Shouldly;
using Xunit;

namespace Tests.LruCache
{
    public class LeastRecentlyUsedCacheTests
    {
        [Fact]
        public void CtorShouldUseDefaultMinCapacityIfSpecifiedCapacityIsLessThanDefault()
        {
            // Arrange
            var expected = 10;

            // Act
            var sut = new LeastRecentlyUsedCache<int, string>(5);
            var actual = sut.Capacity;

            // Assert
            actual.ShouldBe(expected);
        }

        [Theory]
        [AutoData]
        public void CtorShouldSetCapacitySpecifiedIfValid(int capacity)
        {
            // Arrange
            
            // Act
            var sut = new LeastRecentlyUsedCache<int, string>(capacity);
            var actual = sut.Capacity;

            // Assert
            actual.ShouldBe(capacity);
        }

        [Theory]
        [AutoData]
        public void AddEntryShouldReturnEntryUsingGetEntry(int key, string actual, LeastRecentlyUsedCache<int, string> sut)
        {
            // Arrange

            // Act
            sut.AddEntry(key, actual);
            var expected = sut.GetEntry(key);

            // Assert
            actual.ShouldBe(expected);
        }

        [Theory]
        [AutoData]
        public void GetMostRecentlyUsedEntryReturnsValidEntry(Fixture fixture, LeastRecentlyUsedCache<int, string> sut)
        {
            // Arrange
            for (int i = 0; i < sut.Capacity; i++)
            {
                var key = fixture.Create<int>();
                sut.AddEntry(key, fixture.Create<string>());
            }
            
            var currentLru = sut.GetLeastRecentlyUsedEntry();

            // Act
            sut.AddEntry(currentLru.Key, currentLru.Entry);
            var actualMru = sut.GetMostRecentlyUsedEntry();
            var actualLru = sut.GetLeastRecentlyUsedEntry();

            // Assert
            actualMru.ShouldBe(currentLru);
            actualLru.ShouldNotBe(currentLru);
        }
        
        [Theory]
        [AutoData]
        public void AddingNewItemGivenCacheFullShouldRemoveLru(Fixture fixture, LeastRecentlyUsedCache<int, string> sut)
        {
            // Arrange
            for (int i = 0; i < sut.Capacity; i++)
            {
                var key = fixture.Create<int>();
                sut.AddEntry(key, fixture.Create<string>());
            }

            var currentLru = sut.GetLeastRecentlyUsedEntry();
            var newKey = fixture.Create<int>();
            var newValue = fixture.Create<string>();

            // Act
            sut.AddEntry(newKey, newValue);
            var actual = sut.ContainsKey(currentLru.Key);

            // Assert
            actual.ShouldBeFalse();
        }

        [Theory]
        [AutoData]
        public void RemoveEntryShouldRemove(int key, string entry, LeastRecentlyUsedCache<int, string> sut)
        {
            // Arrange
            sut.AddEntry(key, entry);

            // Act
            sut.RemoveEntry(key);
            var actual = sut.GetEntry(key);

            // Assert
            actual.ShouldBeNull();
        }


        [Theory]
        [AutoData]
        public void ClearShouldRemoveAllData(int[] keys, string[] entries, LeastRecentlyUsedCache<int, string> sut)
        {
            // Arrange
            for (int i = 0; i < keys.Length; i++)
                sut.AddEntry(keys[i], entries[i]);

            // Act
            sut.Clear();
            var actual = sut.Count;

            // Assert
            actual.ShouldBe(0);
        }
    }
}
