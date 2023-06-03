using System;
using System.Collections.Generic;
using CannedBytes.Midi.Device.Converters;

namespace CannedBytes.Midi.Device.Message
{
    internal class AddressMapSequentialFieldNavigator : AddressMapFieldConverterNavigator
    {
        private SevenBitUInt32 _lastPosition;

        public AddressMapSequentialFieldNavigator(FieldConverterPair rootPair)
            : base(rootPair)
        {
            CurrentAddress = new SevenBitUInt32();
            _lastPosition = new SevenBitUInt32();
        }

        protected SevenBitUInt32 CurrentAddress { get; private set; }

        private List<DynamicFieldConverterPair> _pairList = new List<DynamicFieldConverterPair>();

        public List<DynamicFieldConverterPair> FieldConverterPairList
        {
            get { return _pairList; }
        }

        protected override NavigationResult OnFieldGroupConverter(FieldConverterPair fieldGroupConverterPair)
        {
            var addressAttr = fieldGroupConverterPair.Field.Attributes.Find(Constants.AddressAttributeName);

            if (addressAttr != null)
            {
                var baseAddress = new AddressProperty(addressAttr.Value).Address;
                this.CurrentAddress = baseAddress + (this.Context.CurrentInstanceIndex * fieldGroupConverterPair.Converter.ByteLength);
                Console.WriteLine("New address: 0x" + CurrentAddress.ToString("X"));
            }

            return base.OnFieldGroupConverter(fieldGroupConverterPair);
        }

        // this Carry is only used to reuse the BitFlags logic
        private Carry _dummyCarry = new Carry();

        protected override NavigationResult OnFieldConverter(FieldConverterPair fieldConverterPair)
        {
            NavigationResult result = base.OnFieldConverter(fieldConverterPair);
            var groupPair = this.FieldGroupConverterStack.Peek();

            // let the parent (group) converter calculate the byte length (e.g. SplitNibble group takes 2 bytes).
            int byteLength = ((GroupConverter)groupPair.Converter).CalculateByteLength(fieldConverterPair.Converter, _dummyCarry);
            SevenBitUInt32 currentAddress = CurrentAddress;

            result = CalculateNextPosition(fieldConverterPair, byteLength);

            if (StartConverterPair != null)
            {
                var dynamicPair = new DynamicFieldConverterPair(
                    fieldConverterPair.Field, fieldConverterPair.Converter,
                    Context.CurrentInstanceIndex, currentAddress);

                AddField(dynamicPair);
            }

            return result;
        }

        private NavigationResult CalculateNextPosition(FieldConverterPair fieldConverterPair, int byteLength)
        {
            // detect already matched
            if (_lastPosition == CurrentAddress && Size > 0)
            {
                return NavigationResult.Error;
            }

            // detect address not starting on a boundary
            if (Address < CurrentAddress && StartConverterPair == null)
            {
                return NavigationResult.Error;
            }
            // detect size not ending on a boundary
            if (_lastPosition < CurrentAddress && EndConverterPair == null && Size > 0)
            {
                return NavigationResult.Error;
            }

            NavigationResult result = NavigationResult.Continue;

            if (Address == CurrentAddress)
            {
                Console.WriteLine("Start found: " + fieldConverterPair.Field);
                StartConverterPair = fieldConverterPair;
            }

            Console.Write("Adding " + byteLength + " to address 0x" + CurrentAddress.ToString("X"));
            // add byte length of current field to current count
            CurrentAddress += byteLength;

            Console.WriteLine(" -> new address 0x" + CurrentAddress.ToString("X"));

            // check for the last
            if (_lastPosition == CurrentAddress && Size > 0)
            {
                Console.WriteLine("End found: " + fieldConverterPair.Field);
                EndConverterPair = fieldConverterPair;

                result = NavigationResult.Stop;   // stop, we have all the info we need (not an error).
            }

            return result; // continue?
        }

        protected virtual void AddField(DynamicFieldConverterPair fieldConverterPair)
        {
            Console.WriteLine("Adding Field to result: " + fieldConverterPair.Field + "[" + fieldConverterPair.InstanceIndex + "]");

            this.FieldConverterPairList.Add(fieldConverterPair);
        }

        public override bool Navigate(SevenBitUInt32 address, SevenBitUInt32 size)
        {
            _lastPosition = address + size;

            bool success = base.Navigate(address, size);

            if (this.EndConverterPair == null)
            {
                if (this.FieldConverterPairList.Count > 0)
                {
                    this.EndConverterPair = this.FieldConverterPairList[this.FieldConverterPairList.Count - 1];
                }
            }

            return success;
        }
    }
}