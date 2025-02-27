﻿using Xunit;

namespace CannedBytes.Midi.Device.Schema.UnitTests;

/// <summary>
/// This is a test class for Jacobi.Midi.Device.Schema.Field and is intended
/// to contain all Jacobi.Midi.Device.Schema.Field Unit Tests
/// </summary>

public class FieldTest
{
    private static DeviceSchema TestSchema = new("FieldTestSchema");

    [Fact]
    public void Constructor_WithName_ParsedCorrectly()
    {
        Field target = new(TestSchema, Constants.SchemaFieldName);

        SchemaObjectHelper.AssertName(target, Constants.SchemaName, Constants.FieldName);
    }
}