using Castle.DynamicProxy;
using FluentAssertions;
using Katalye.Components.Configuration;
using NSubstitute;
using Xunit;

namespace Katalye.Components.Tests.Configuration
{
    public class ConfigurationRouterFacts
    {
        [Fact]
        public void Can_create_configuration_proxy()
        {
            var generator = new ProxyGenerator();
            var interceptorMock = Substitute.For<IRoutingInterceptor>();
            var router = new ConfigurationRouter(generator, interceptorMock);

            // Act
            var result = router.CreateConfiguration();

            // Assert
            result.Should().NotBeNull();
        }
    }
}