﻿using Xunit;
using FluentAssertions;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    /// This is a test class for Jacobi.Midi.Device.Schema.DataType and is intended
    /// to contain all Jacobi.Midi.Device.Schema.DataType Unit Tests
    /// </summary>
    
    public class DataTypeTest
    {
        [Fact]
        public void Constructor_WithName_ParsesCorrectly()
        {
            var target = new DataType(Constants.SchemaTypeName);

            SchemaObjectTest.AssertName(target, Constants.SchemaName, Constants.TypeName);
        }

        [Fact]
        public void BaseTypesAdd_NewDataType_Sticks()
        {
            var target = new DataType(Constants.SchemaTypeName);
            var baseType = new DataType(Constants.SchemaBaseTypeName);

            target.BaseTypes.Add(baseType);

            target.HasBaseTypes.Should().BeTrue();
            target.BaseType.Should().NotBeNull();
            target.BaseType.Should().BeSameAs(baseType);
            target.BaseTypes.Should().HaveCount(1);
        }

        [Fact]
        public void DataTypeSchema_SchemaProperty_IsSame()
        {
            var schema = new DeviceSchema(Constants.SchemaName);
            var target = new DataType(Constants.SchemaTypeName);

            schema.AllDataTypes.Add(target);

            target.Schema.Should().NotBeNull();
            target.Schema.Should().BeSameAs(schema);
        }

        [Fact]
        public void IsType_OwnType_IsFound()
        {
            var target = new DataType(Constants.SchemaTypeName);
            var baseType = new DataType(Constants.SchemaBaseTypeName);

            target.BaseTypes.Add(baseType);

            // own type should be found with/w-out recursive.
            target.IsType(Constants.SchemaTypeName, false).Should().BeTrue();
            target.IsType(Constants.SchemaTypeName, true).Should().BeTrue();
        }

        [Fact]
        public void IsType_BaseType_IsFound()
        {
            var target = new DataType(Constants.SchemaTypeName);
            var baseType = new DataType(Constants.SchemaBaseTypeName);

            target.BaseTypes.Add(baseType);

            // immediate base type should be found with/w-out recursive.
            target.IsType(Constants.SchemaBaseTypeName, true).Should().BeTrue();
            target.IsType(Constants.SchemaBaseTypeName, false).Should().BeTrue();
        }
    }
}