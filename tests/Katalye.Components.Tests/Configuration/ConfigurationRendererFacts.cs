using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Katalye.Components.Configuration;
using Katalye.Components.Configuration.Providers;
using Katalye.Components.Configuration.ValueParsers;
using NSubstitute;
using Xunit;

namespace Katalye.Components.Tests.Configuration
{
    public class ConfigurationRendererFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        [Fact]
        public void When_accessing_proxy_property_then_return_config_password()
        {
            var pathFake = AutoFixture.Create<string>();
            var providerMock = Substitute.For<IConfigurationProvider>();
            var configurationProviders = new List<IConfigurationProvider>
            {
                providerMock
            };

            var interceptor = new ConfigurationRenderer(configurationProviders, type => new StringValueParser());

            var valueFake = AutoFixture.Create<string>();
            providerMock.TryGet(Arg.Any<string>()).Returns((true, valueFake));

            // Act
            var result = interceptor.RenderSetting(pathFake);

            // Assert
            result.Value.Should().Be(valueFake);
        }

        [Fact]
        public void When_accessing_proxy_property_then_return_correct_data_type()
        {
            var pathFake = AutoFixture.Create<string>();
            var providerMock = Substitute.For<IConfigurationProvider>();
            var configurationProviders = new List<IConfigurationProvider>
            {
                providerMock
            };

            var interceptor = new ConfigurationRenderer(configurationProviders, type => new UriValueParser());

            var valueFake = AutoFixture.Create<Uri>();
            providerMock.TryGet(Arg.Any<string>()).Returns((true, valueFake.ToString()));

            // Act
            var result = interceptor.RenderSetting(pathFake, typeof(string));

            // Assert
            result.Value.Should().BeOfType<Uri>()
                  .Which.Should().Be(valueFake);
        }

        [Fact]
        public void When_accessing_proxy_property_and_value_is_null_then_return_default()
        {
            var pathFake = AutoFixture.Create<string>();
            var providerMock = Substitute.For<IConfigurationProvider>();
            var configurationProviders = new List<IConfigurationProvider>
            {
                providerMock
            };

            var interceptor = new ConfigurationRenderer(configurationProviders, type => new BooleanValueParser());

            providerMock.TryGet(Arg.Any<string>()).Returns((true, null));

            // Act
            var result = interceptor.RenderSetting(pathFake, typeof(bool));

            // Assert
            result.Value.Should().BeOfType<bool>()
                  .Which.Should().BeFalse();
        }
    }
}