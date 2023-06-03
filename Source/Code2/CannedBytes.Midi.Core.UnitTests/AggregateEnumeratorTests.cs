using Xunit;
using CannedBytes.Collections;
using FluentAssertions;

namespace CannedBytes.Midi.Core.UnitTests
{
    public class AggregateEnumeratorTests
    {
        [Fact]
        public void ForEach_SimpleList_IteratesAllItems()
        {
            var list = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var aggEnum = new AggregateEnumerator<int>();
            aggEnum.Add(list);

            int count = 0;
            foreach (var item in aggEnum)
            {
                item.Should().BeOneOf(list);
                count++;
            }

            count.Should().Be(list.Length);
        }
    }
}
