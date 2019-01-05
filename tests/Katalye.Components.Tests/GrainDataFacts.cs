using FluentAssertions;
using Katalye.Components.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Katalye.Components.Tests
{
    public class GrainDataFacts
    {
        private const string GrainFixture =
            "{\"kernel\":\"Linux\",\"domain\":\"\",\"uid\":0,\"pythonpath\":[\"/usr/bin\"],\"ip_interfaces\":{\"br-950befb17084\":[\"172.18.0.1\",\"fe80::42:6ff:fe06:4490\"]},\"saltversioninfo\":[2018,3,3,0],\"zfs_support\":false,\"SSDs\":[]}";

        [Fact]
        public void Supports_booleans()
        {
            var flattener = new JsonFlattener();
            var fixture = JsonConvert.DeserializeObject<JToken>(GrainFixture);

            // Act
            var result = flattener.Flatten(fixture);

            // Assert
            result.Should().ContainKey("zfs_support")
                  .WhichValue.Should().ContainSingle(x => x == false.ToString());
        }

        [Fact]
        public void Supports_numbers()
        {
            var flattener = new JsonFlattener();
            var fixture = JsonConvert.DeserializeObject<JToken>(GrainFixture);

            // Act
            var result = flattener.Flatten(fixture);

            // Assert
            result.Should().ContainKey("uid")
                  .WhichValue.Should().ContainSingle(x => x == 0.ToString());
        }

        [Fact]
        public void Supports_arrays()
        {
            var flattener = new JsonFlattener();
            var fixture = JsonConvert.DeserializeObject<JToken>(GrainFixture);

            // Act
            var result = flattener.Flatten(fixture);

            // Assert
            result.Should().ContainKey("pythonpath")
                  .WhichValue.Should().ContainSingle(x => x == "/usr/bin");
        }

        [Fact]
        public void Supports_empty_arrays()
        {
            var flattener = new JsonFlattener();
            var fixture = JsonConvert.DeserializeObject<JToken>(GrainFixture);

            // Act
            var result = flattener.Flatten(fixture);

            // Assert
            result.Should().NotContainKey("SSDs");
        }

        [Fact]
        public void Supports_nesting()
        {
            var flattener = new JsonFlattener();
            var fixture = JsonConvert.DeserializeObject<JToken>(GrainFixture);

            // Act
            var result = flattener.Flatten(fixture);

            // Assert
            result.Should().ContainKey("ip_interfaces.br-950befb17084")
                  .WhichValue.Should().HaveCount(2)
                  .And.Subject.Should().Contain(x => x == "172.18.0.1");
        }
    }
}