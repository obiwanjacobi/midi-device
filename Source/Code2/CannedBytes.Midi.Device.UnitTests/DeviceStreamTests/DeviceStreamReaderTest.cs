using System;
using Xunit;
using System.IO;
using FluentAssertions;

namespace CannedBytes.Midi.Device.UnitTests.DeviceStream
{
    
    public class DeviceStreamReaderTest
    {
        public static DeviceStreamReader CreateDeviceStreamReader(byte[] buffer)
        {
            return CreateDeviceStreamReader(new MemoryStream(buffer, false));
        }

        public static DeviceStreamReader CreateDeviceStreamReader(Stream stream)
        {
            return new DeviceStreamReader(stream, new Carry());
        }

        [Fact]
        public void ReadWithCarry()
        {
            var reader = CreateDeviceStreamReader(new byte[] { 0xFF });

            ushort result = 0;

            var read = reader.Read(BitFlags.Bit1 | BitFlags.Bit2 | BitFlags.Bit3 | BitFlags.Bit4 | BitFlags.Bit5, out result);

            // value is not shifted down!
            result.Should().Be(62);
        }
    }
}
