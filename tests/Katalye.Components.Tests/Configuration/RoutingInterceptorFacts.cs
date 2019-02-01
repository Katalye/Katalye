using System;
using System.Collections.Generic;
using AutoFixture;
using Castle.DynamicProxy;
using FluentAssertions;
using Katalye.Components.Configuration;
using Katalye.Components.Configuration.Providers;
using NSubstitute;
using Xunit;

namespace Katalye.Components.Tests.Configuration
{
    public class RoutingInterceptorFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        [Fact]
        public void When_accessing_proxy_property_then_pass_config_path()
        {
            var providerMock = Substitute.For<IConfigurationProvider>();
            var configurationProviders = new List<IConfigurationProvider>
            {
                providerMock
            };

            var interceptor = new RoutingInterceptor(configurationProviders);

            var fixture = new ConfigurationRouter(new ProxyGenerator(), interceptor);
            var config = fixture.CreateConfiguration();

            // Act
            var _ = config.SaltApiServer;

            // Assert
            providerMock.Received().TryGet("Katalye:Salt:Api", Arg.Any<string>());
        }

        [Fact]
        public void When_accessing_proxy_property_then_return_config_password()
        {
            var providerMock = Substitute.For<IConfigurationProvider>();
            var configurationProviders = new List<IConfigurationProvider>
            {
                providerMock
            };

            var interceptor = new RoutingInterceptor(configurationProviders);

            var fixture = new ConfigurationRouter(new ProxyGenerator(), interceptor);
            var config = fixture.CreateConfiguration();

            var valueFake = AutoFixture.Create<string>();
            providerMock.TryGet(Arg.Any<string>(), Arg.Any<string>()).Returns((true, valueFake));

            // Act
            var result = config.SaltApiServicePassword;

            // Assert
            result.Should().Be(valueFake);
        }

        [Fact]
        public void When_accessing_proxy_property_then_return_correct_data_type()
        {
            var providerMock = Substitute.For<IConfigurationProvider>();
            var configurationProviders = new List<IConfigurationProvider>
            {
                providerMock
            };

            var interceptor = new RoutingInterceptor(configurationProviders);

            var fixture = new ConfigurationRouter(new ProxyGenerator(), interceptor);
            var config = fixture.CreateConfiguration();

            var valueFake = AutoFixture.Create<Uri>();
            providerMock.TryGet(Arg.Any<string>(), Arg.Any<string>()).Returns((true, valueFake.ToString()));

            // Act
            var result = config.SaltApiServer;

            // Assert
            result.Should().Be(valueFake);
        }

        [Fact]
        public void When_accessing_proxy_property_and_value_is_null_then_return_default()
        {
            var providerMock = Substitute.For<IConfigurationProvider>();
            var configurationProviders = new List<IConfigurationProvider>
            {
                providerMock
            };

            var interceptor = new RoutingInterceptor(configurationProviders);

            var fixture = new ConfigurationRouter(new ProxyGenerator(), interceptor);
            var config = fixture.CreateConfiguration();
            
            providerMock.TryGet(Arg.Any<string>(), Arg.Any<string>()).Returns((true, null));

            // Act
            var result = config.DisableDistributedLocks;

            // Assert
            result.Should().BeFalse();
        }
    }
}