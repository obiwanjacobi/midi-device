using System;
using CannedBytes.Midi.Device.Converters;

namespace CannedBytes.Midi.Device.Message
{
    internal abstract class AddressMapFieldConverterNavigator : FieldConverterMapNavigator
    {
        protected AddressMapFieldConverterNavigator(FieldConverterPair rootPair)
            : base(rootPair)
        { }

        private SevenBitUInt32 _address;

        protected SevenBitUInt32 Address
        {
            get { return _address; }
        }

        private SevenBitUInt32 _size;

        protected SevenBitUInt32 Size
        {
            get { return _size; }
        }

        private FieldConverterPair _startPair;

        public FieldConverterPair StartConverterPair
        {
            get { return _startPair; }
            protected set { _startPair = value; }
        }

        private FieldConverterPair _endPair;

        public FieldConverterPair EndConverterPair
        {
            get { return _endPair; }
            protected set { _endPair = value; }
        }

        public virtual bool Navigate(SevenBitUInt32 address, SevenBitUInt32 size)
        {
            Console.WriteLine("Navigate address: 0x" + address.ToString("X") + " size: 0x" + size.ToString("X"));

            _address = address;
            _size = size;

            bool success = base.Navigate();

            if (this.StartConverterPair == null)
            {
                throw new ArgumentOutOfRangeException("address", "Value did not lie on a type boundary");
            }

            // Allow for Size too large.
            //if (this.EndConverterPair == null)
            //{
            //    throw new ArgumentOutOfRangeException("size", "Value did not lie on a type boundary");
            //}

            return success;
        }
    }
}