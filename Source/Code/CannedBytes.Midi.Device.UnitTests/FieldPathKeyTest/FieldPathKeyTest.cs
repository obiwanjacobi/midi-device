using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.UnitTests.FieldPathKeyTest
{
    [TestClass]
    public class FieldPathKeyTest
    {
        [TestMethod]
        public void DefCtor_DepthAndIsZero_AllZero()
        {
            var key = new FieldPathKey();

            Assert.AreEqual(0, key.Depth);
            Assert.AreEqual(true, key.IsZero);
            Assert.AreEqual(0, key.Values.Count());
            Assert.AreEqual("", key.ToString());
        }

        [TestMethod]
        public void IndexCtor_DepthAndIsZero_AllOne()
        {
            var key = new FieldPathKey(1);

            Assert.AreEqual(1, key.Depth);
            Assert.AreEqual(false, key.IsZero);
            Assert.AreEqual(1, key.Values.Count());
            Assert.AreEqual("1", key.ToString());
        }

        [TestMethod]
        public void Add_DepthAndIsZero_CorrectValues()
        {
            var key = new FieldPathKey();
            key.Add(0);
            key.Add(1);
            key.Add(0);

            Assert.AreEqual(3, key.Depth);
            Assert.AreEqual(false, key.IsZero);
            Assert.AreEqual(3, key.Values.Count());
            Assert.AreEqual("0|1|0", key.ToString());
        }
    }
}