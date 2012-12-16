﻿using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters
{
    public sealed class TableEntityConverterTests
    {
        [Fact]
        public void CheckPartionAndRowKeysNameMappings()
        {
            // Arrange & Act
            var converter = new TableEntityConverter<Country>();

            // Assert
            Assert.NotNull(converter.NameMappings);
            Assert.Equal(converter.NameMappings.Count, 2);
            Assert.Equal(converter.NameMappings["Continent"], "PartitionKey");
            Assert.Equal(converter.NameMappings["Name"], "RowKey");
        }

        [Fact]
        public void ConvertEntityToTableEntityTest()
        {
            // Arrange
            var converter = new TableEntityConverter<Country>();
            var country = new Country
                              {
                                  Area = 505992,
                                  Continent = "Europe",
                                  TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                  Formed = new DateTime(1812, 1, 1),
                                  Id = Guid.NewGuid(),
                                  IsExists = true,
                                  Name = "Spain",
                                  Population = 47190493,
                                  PresidentsCount = 8
                              };

            var context = new OperationContext();

            // Act
            ITableEntity tableEntity = converter.GetEntity(country);
            IDictionary<string, EntityProperty> properties = tableEntity.WriteEntity(context);

            // Assert
            Assert.Equal(tableEntity.ETag, "*");
            Assert.Equal(tableEntity.PartitionKey, country.Continent);
            Assert.Equal(tableEntity.RowKey, country.Name);

            Assert.Equal(properties["Area"].DoubleValue, country.Area);
            Assert.Equal(properties["TopSecretKey"].BinaryValue, country.TopSecretKey);
            Assert.True(properties["Formed"].DateTimeOffsetValue.HasValue);
// ReSharper disable PossibleInvalidOperationException
            Assert.Equal(properties["Formed"].DateTimeOffsetValue.Value.DateTime, country.Formed);
// ReSharper restore PossibleInvalidOperationException
            Assert.Equal(properties["Id"].GuidValue, country.Id);
            Assert.Equal(properties["IsExists"].BooleanValue, country.IsExists);
            Assert.Equal(properties["Population"].Int64Value, country.Population);
            Assert.Equal(properties["PresidentsCount"].Int32Value, country.PresidentsCount);
        }

        [Fact]
        public void ConvertTableEntityToEntityTest()
        {
            // Arrange
            var converter = new TableEntityConverter<Country>();
            var tableEntity = new DynamicTableEntity
                                  {
                                      PartitionKey = "Europe",
                                      RowKey = "Spain",
                                      Properties = new Dictionary<string, EntityProperty>
                                                       {
                                                           {"Area", new EntityProperty(505992d)},
                                                           {
                                                               "TopSecretKey",
                                                               new EntityProperty(new byte[] {0xaa, 0xbb, 0xcc})
                                                           },
                                                           {"Formed", new EntityProperty(new DateTime(1812, 1, 1))},
                                                           {"Id", new EntityProperty(Guid.NewGuid())},
                                                           {"IsExists", new EntityProperty(true)},
                                                           {"Population", new EntityProperty(47190493L)},
                                                           {"PresidentsCount", new EntityProperty(8)},
                                                       }
                                  };

            // Act
            Country country = converter.GetEntity(tableEntity);

            // Assert
            Assert.Equal(tableEntity.PartitionKey, country.Continent);
            Assert.Equal(tableEntity.RowKey, country.Name);

            Assert.Equal(tableEntity.Properties["Area"].DoubleValue, country.Area);
            Assert.Equal(tableEntity.Properties["TopSecretKey"].BinaryValue, country.TopSecretKey);
            Assert.True(tableEntity.Properties["Formed"].DateTimeOffsetValue.HasValue);
// ReSharper disable PossibleInvalidOperationException
            Assert.Equal(tableEntity.Properties["Formed"].DateTimeOffsetValue.Value.DateTime, country.Formed);
// ReSharper restore PossibleInvalidOperationException
            Assert.Equal(tableEntity.Properties["Id"].GuidValue, country.Id);
            Assert.Equal(tableEntity.Properties["IsExists"].BooleanValue, country.IsExists);
            Assert.Equal(tableEntity.Properties["Population"].Int64Value, country.Population);
            Assert.Equal(tableEntity.Properties["PresidentsCount"].Int32Value, country.PresidentsCount);
        }
    }
}