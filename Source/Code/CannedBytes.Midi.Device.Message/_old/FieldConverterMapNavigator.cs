using System;
using System.Collections.Generic;
using CannedBytes.Midi.Device.Converters;

namespace CannedBytes.Midi.Device.Message
{
    internal abstract class FieldConverterMapNavigator
    {
        public FieldConverterMapNavigator(FieldConverterPair rootPair)
        {
            if (rootPair.Field.RecordType != null)
            {
                _context = CreateContext(rootPair);
            }

            _rootPair = rootPair;
        }

        private FieldConverterPair _rootPair;

        public FieldConverterPair RootFieldConverterPair
        {
            get { return _rootPair; }
        }

        private Stack<FieldConverterPair> _pairStack = new Stack<FieldConverterPair>();

        protected Stack<FieldConverterPair> FieldGroupConverterStack
        {
            get { return _pairStack; }
        }

        private MidiMessageDataContext _context;

        public MidiMessageDataContext Context
        {
            get { return _context; }
            protected set { _context = value; }
        }

        public bool Navigate()
        {
            bool success = (Navigate(_rootPair) != NavigationResult.Error);

            return success;
        }

        protected virtual MidiMessageDataContext CreateContext(FieldConverterPair pair)
        {
            // TODO: find a cheaper way to use field enumerators.
            return new MidiMessageDataContext(pair.Field.RecordType, (GroupConverter)pair.Converter);
        }

        protected NavigationResult Navigate(FieldConverterPair fieldConverterPair)
        {
            NavigationResult result = NavigationResult.Continue;

            if (fieldConverterPair.Field.RecordType != null)
            {
                result = OnFieldGroupConverter(fieldConverterPair);
            }
            else if (fieldConverterPair.Field.DataType != null)
            {
                result = OnFieldConverter(fieldConverterPair);
            }

            return result;
        }

        protected virtual NavigationResult OnFieldGroupConverter(FieldConverterPair fieldGroupConverterPair)
        {
            Console.WriteLine("Navigate: " + fieldGroupConverterPair.Field);

            NavigationResult result = NavigationResult.Continue;

            FieldGroupConverterStack.Push(fieldGroupConverterPair);

            using (IEnumerator<FieldConverterPair> enumPairs = Context.GetEnumerator((GroupConverter)fieldGroupConverterPair.Converter))
            {
                while (enumPairs.MoveNext())
                {
                    result = Navigate(enumPairs.Current);

                    if (result != NavigationResult.Continue)
                    {
                        Console.WriteLine("Abort navigation: " + result.ToString());
                        break;
                    }
                }
            }

            FieldGroupConverterStack.Pop();

            return result;
        }

        protected virtual NavigationResult OnFieldConverter(FieldConverterPair fieldConverterPair)
        {
            Console.WriteLine("Navigate: " + fieldConverterPair.Field);

            return NavigationResult.Continue;
        }
    }
}