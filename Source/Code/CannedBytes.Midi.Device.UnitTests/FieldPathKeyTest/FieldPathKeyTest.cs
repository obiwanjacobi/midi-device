using System.Linq;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.FieldPathKeyTest
{
    
    public class FieldPathKeyTest
    {
        [Fact]
        public void DefCtor_DepthAndIsZero_AllZero()
        {
            var key = new FieldPathKey();

            Assert.Equal(0, key.Depth);
            Assert.Equal(true, key.IsZero);
            Assert.Equal(0, key.Values.Count());
            Assert.Equal("", key.ToString());
        }

        [Fact]
        public void IndexCtor_DepthAndIsZero_AllOne()
        {
            var key = new FieldPathKey(1);

            Assert.Equal(1, key.Depth);
            Assert.Equal(false, key.IsZero);
            Assert.Equal(1, key.Values.Count());
            Assert.Equal("1", key.ToString());
        }

        [Fact]
        public void Add_DepthAndIsZero_CorrectValues()
        {
            var key = new FieldPathKey();
            key.Add(0);
            key.Add(1);
            key.Add(0);

            Assert.Equal(3, key.Depth);
            Assert.Equal(false, key.IsZero);
            Assert.Equal(3, key.Values.Count());
            Assert.Equal("0|1|0", key.ToString());
        }
    }
}