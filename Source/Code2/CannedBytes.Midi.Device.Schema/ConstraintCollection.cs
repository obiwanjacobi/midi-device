namespace CannedBytes.Midi.Device.Schema;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// The ConstraintCollection class manages <see cref="Constraint"/> items.
/// </summary>
public sealed class ConstraintCollection : Collection<Constraint>
{
    public IEnumerable<Constraint> FindAll<T>()
    {
        return from constraint in Items
               where constraint is T
               select constraint;
    }

    public IEnumerable<Constraint> FindAll(ConstraintTypes type)
    {
        return from constraint in Items
               where constraint.ConstraintType == type
               select constraint;
    }

    public Constraint Find<T>()
    {
        return FindAll<T>().FirstOrDefault();
    }

    public Constraint Find(ConstraintTypes type)
    {
        return FindAll(type).FirstOrDefault();
    }

    /// <summary>
    /// Validates the <paramref name="value"/> against all <see cref="Constraint"/>s
    /// in the collection.
    /// </summary>
    /// <param name="value">The data byte to validate.</param>
    /// <returns>Returns true if the <paramref name="value"/> passed validation
    /// otherwise false is returned.</returns>
    public bool Validate<T>(T value)
        where T : IComparable
    {
        bool success = true;
        Dictionary<string, bool> typeResults = new();

        foreach (Constraint constraint in this)
        {
            if (constraint.ValidationType == ConstraintValidationTypes.OneOf)
            {
                if (!typeResults.ContainsKey(constraint.Name))
                {
                    typeResults.Add(constraint.Name, false);
                }

                if (!typeResults[constraint.Name])
                {
                    typeResults[constraint.Name] =
                        constraint.Validate<T>(value);
                }
            }
            else
            {
                success = constraint.Validate<T>(value);

                if (!success)
                {
                    break;
                }
            }
        }

        if (success)
        {
            foreach (KeyValuePair<string, bool> item in typeResults)
            {
                if (!item.Value)
                {
                    success = false;
                }
            }
        }

        return success;
    }

    public void Merge(ConstraintCollection constraints)
    {
        ConstraintCollection newConstraints = new();

        foreach (var constraint in constraints)
        {
            var currentConstraints = FindAll(constraint.ConstraintType);

            if (currentConstraints == null || currentConstraints.Count() == 0)
            {
                newConstraints.Add(constraint);
            }
            else
            {
                if (constraint.ConstraintType == ConstraintTypes.Enumeration)
                {
                    foreach (Constraint enumConstraint in currentConstraints)
                    {
                        // add enums with a value not yet in collection.
                        if (enumConstraint.GetValue<long>() != constraint.GetValue<long>())
                        {
                            newConstraints.Add(constraint);
                        }
                    }
                }
            }

            foreach (Constraint newConstraint in newConstraints)
            {
                Add(newConstraint);
            }
        }
    }
}