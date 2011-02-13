using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Composable.System.Linq;

namespace Core.Tests.Linq
{
    [TestFixture]
    public class LinqExtensionsTests
    {
        [Test]
        public void FlattenShouldIterateAllNestedCollectionInstances()
        {
            var nestedInts = new List<List<int>>
                             {
                                 new List<int> {1,},
                                 new List<int> {2, 3},
                                 new List<int> {4, 5, 6, 7}
                             };

            var flattened = LinqExtensions.Flatten<List<int>, int>(nestedInts);
            Assert.That(flattened, Is.EquivalentTo(1.Through(7)));
        }

        [Test]
        public void ChoppingFollowedBySelectManyShouldEqualOriginalSequence()
        {
            var oneThroughAHundred = 1.Through(10003).ChopIntoSizesOf(10).SelectMany(me => me);
            Assert.That(oneThroughAHundred, Is.EqualTo(1.Through(10003)));
        }

        [Test]
        public void ChoppingListIntoListSizeChunksShouldReturnOnlyOneChunk()
        {
            var oneEntry = 1.Through(10).ChopIntoSizesOf(10);
            Assert.That(oneEntry.Count(), Is.EqualTo(1));
        }
    }
}