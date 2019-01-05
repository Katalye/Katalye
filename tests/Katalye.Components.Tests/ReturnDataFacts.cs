using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Katalye.Components.Tests
{
    public class ReturnDataFacts
    {
        [Fact]
        public void Can_handle_booleans()
        {
            var fixture = new
            {
                Data = true
            };
            var json = JsonConvert.SerializeObject(fixture);

            // Act
            var result = JsonConvert.DeserializeObject<ReturnDataConverterFixture>(json);

            // Assert
            result.Data.ToObject<bool>().Should().BeTrue();
        }

        [Fact]
        public void Can_handle_objects()
        {
            var fixture = new
            {
                Data = new Dictionary<string, object>
                {
                    {
                        "key", new
                        {
                            Test = true
                        }
                    }
                }
            };
            var json = JsonConvert.SerializeObject(fixture);

            // Act
            var result = JsonConvert.DeserializeObject<ReturnDataConverterFixture>(json);

            // Assert
            result.Data.ToString().Should().Be("{\r\n  \"key\": {\r\n    \"Test\": true\r\n  }\r\n}");
        }

        public class ReturnDataConverterFixture
        {
            public JToken Data { get; set; }
        }
    }
}