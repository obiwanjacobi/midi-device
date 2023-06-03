using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace CannedBytes.Midi.SpeechController.DomainModel
{
    /// <summary>
    /// A base class for the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Helper method for derived classes to set a <paramref name="newValue"/> for the property's <paramref name="backingField"/>.
        /// </summary>
        /// <typeparam name="T">Inferred - usually you do not have to specify.</typeparam>
        /// <param name="backingField">The backing field of your property By Ref.</param>
        /// <param name="newValue">The new value that has to be assigned to the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>Returns true when the value of the property was actually changed and the event raised.</returns>
        protected bool SetPropertyValue<T>(ref T backingField, T newValue, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(backingField, newValue))
            {
                backingField = newValue;

                OnPropertyChanged(propertyName);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            AssertPropertyName(propertyName);

            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raised when a property has changed value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Debug (only) helper that verifies if the specified <paramref name="propertyName"/> actually exists on the object.
        /// </summary>
        /// <param name="propertyName">Must not be null or empty.</param>
        /// <exception cref="InvalidOperationException">
        /// Throw when the <paramref name="propertyName"/> was not found on the instance.
        /// </exception>
        [Conditional("DEBUG")]
        private void AssertPropertyName(string propertyName)
        {
            var propInfo = GetType().GetProperty(propertyName);

            if (propInfo == null)
            {
                throw new InvalidOperationException(
                    "There is no property found with the name '" + propertyName + "' on the type: " + GetType().FullName);
            }
        }
    }
}