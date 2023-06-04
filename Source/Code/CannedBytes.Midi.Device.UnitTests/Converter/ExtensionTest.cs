using System.Linq;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.UnitTests.Stubs;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.Converter
{
    
    //[DeploymentItem("Converter/ConverterExtensionTest.mds")]
    //[DeploymentItem("Converter/ConverterExtensionStream.bin")]
    public class ExtensionTest
    {
        [Fact]
        public void LoadExtensionType_ExtensionToSigned_NoErrors()
        {
            var schema = DeviceHelper.OpenDeviceSchema("ConverterExtensionTest.mds");
            Assert.NotNull(schema);

            var dt = schema.AllDataTypes.Find("SignedRange");
            Assert.NotNull(dt);

            Assert.True(dt.HasBaseTypes);
            Assert.False(dt.IsUnion);
            Assert.False(dt.IsAbstract);
            Assert.True(dt.IsExtension);
            Assert.Equal(2, dt.BaseTypes.Count);

            Assert.Equal("midiSigned", dt.BaseTypes[0].Name.Name);
            Assert.Equal("midiBit0-4", dt.BaseTypes[1].Name.Name);
        }

        [Fact]
        public void CreateConverter_ExtensionToSigned_StackedConverters()
        {
            var schema = DeviceHelper.OpenDeviceSchema("ConverterExtensionTest.mds");
            Assert.NotNull(schema);

            var dt = schema.AllDataTypes.Find("SignedRange");
            Assert.NotNull(dt);

            var container = DeviceHelper.CreateContainer();
            var converterMgr = container.GetExportedValue<ConverterManager>();

            var converter = converterMgr.GetConverter(dt);
            Assert.NotNull(converter);

            var extension = converter as IConverterExtension;
            Assert.NotNull(extension);
            Assert.NotNull(extension.InnerConverter);
            Assert.Null(extension.InnerConverter.InnerConverter);
        }

        [Fact]
        public void CallConverter_ExtensionToSigned_CorrectValue()
        {
            var writer = new DictionaryBasedLogicalStub();
            DeviceHelper.ReadLogical("ConverterExtensionTest.mds", "ConverterExtensionStream.bin", "ExtensionTest", writer);

            Assert.Equal(1, writer.FieldValues.Count);
            Assert.Equal(0x00, writer.FieldValues.Values.First());
        }
    }
}