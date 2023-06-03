using System.Linq;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.UnitTests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.UnitTests.Converter
{
    [TestClass]
    [DeploymentItem("Converter/ConverterExtensionTest.mds")]
    [DeploymentItem("Converter/ConverterExtensionStream.bin")]
    public class ExtensionTest
    {
        [TestMethod]
        public void LoadExtensionType_ExtensionToSigned_NoErrors()
        {
            var schema = DeviceHelper.OpenDeviceSchema("ConverterExtensionTest.mds");
            Assert.IsNotNull(schema);

            var dt = schema.AllDataTypes.Find("SignedRange");
            Assert.IsNotNull(dt);

            Assert.IsTrue(dt.HasBaseTypes);
            Assert.IsFalse(dt.IsUnion);
            Assert.IsFalse(dt.IsAbstract);
            Assert.IsTrue(dt.IsExtension);
            Assert.AreEqual(2, dt.BaseTypes.Count);

            Assert.AreEqual("midiSigned", dt.BaseTypes[0].Name.Name);
            Assert.AreEqual("midiBit0-4", dt.BaseTypes[1].Name.Name);
        }

        [TestMethod]
        public void CreateConverter_ExtensionToSigned_StackedConverters()
        {
            var schema = DeviceHelper.OpenDeviceSchema("ConverterExtensionTest.mds");
            Assert.IsNotNull(schema);

            var dt = schema.AllDataTypes.Find("SignedRange");
            Assert.IsNotNull(dt);

            var container = DeviceHelper.CreateContainer();
            var converterMgr = container.GetExportedValue<ConverterManager>();

            var converter = converterMgr.GetConverter(dt);
            Assert.IsNotNull(converter);

            var extension = converter as IConverterExtension;
            Assert.IsNotNull(extension);
            Assert.IsNotNull(extension.InnerConverter);
            Assert.IsNull(extension.InnerConverter.InnerConverter);
        }

        [TestMethod]
        public void CallConverter_ExtensionToSigned_CorrectValue()
        {
            var writer = new DictionaryBasedLogicalStub();
            DeviceHelper.ReadLogical("ConverterExtensionTest.mds", "ConverterExtensionStream.bin", "ExtensionTest", writer);

            Assert.AreEqual(1, writer.FieldValues.Count);
            Assert.AreEqual(0x00, writer.FieldValues.Values.First());
        }
    }
}