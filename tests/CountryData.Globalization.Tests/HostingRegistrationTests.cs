using CountryData.Globalization.Data;
using CountryData.Globalization.Hosting.Extensions;
using CountryData.Globalization.Hosting.Options;
using CountryData.Globalization.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CountryData.Globalization.Tests
{
    /// <summary>
    /// Tests for Dependency Injection and service registration functionality.
    /// Validates that CountryData services can be properly registered and resolved from the DI container.
    /// </summary>
    public class HostingRegistrationTests
    {
        #region Service Registration Tests

        [Fact]
        public void AddCountryData_ShouldRegisterAllRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddCountryData();
            var serviceProvider = services.BuildServiceProvider();

            // Assert - All three services should be registered
            Assert.NotNull(serviceProvider.GetService<CountryDataLoader>());
            Assert.NotNull(serviceProvider.GetService<ICountryDataProvider>());
            Assert.NotNull(serviceProvider.GetService<CountryDataOptions>());
        }

        [Fact]
        public void AddCountryData_WithOptionsAction_ShouldRegisterServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act - Even though options are empty, the overload should work
            services.AddCountryData(options => { /* Options class is currently empty */ });
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            Assert.NotNull(serviceProvider.GetService<ICountryDataProvider>());
        }

        #endregion

        #region Service Lifetime Tests

        [Fact]
        public void RegisteredServices_ShouldAllBeSingletons()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddCountryData();

            // Act
            var serviceDescriptors = services.ToList();

            // Assert - All services should be Singleton
            var loaderDescriptor = serviceDescriptors.FirstOrDefault(s => s.ServiceType == typeof(CountryDataLoader));
            var providerDescriptor = serviceDescriptors.FirstOrDefault(s => s.ServiceType == typeof(ICountryDataProvider));
            var optionsDescriptor = serviceDescriptors.FirstOrDefault(s => s.ServiceType == typeof(CountryDataOptions));

            Assert.Equal(ServiceLifetime.Singleton, loaderDescriptor?.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, providerDescriptor?.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, optionsDescriptor?.Lifetime);
        }

        [Fact]
        public void Provider_ShouldBeSingletonAcrossScopes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddCountryData();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var provider1 = serviceProvider.GetService<ICountryDataProvider>();
            
            using (var scope = serviceProvider.CreateScope())
            {
                var provider2 = scope.ServiceProvider.GetService<ICountryDataProvider>();

                // Assert - Should be same instance across scopes
                Assert.Same(provider1, provider2);
            }
        }

        #endregion

        #region Provider Functionality Tests

        [Fact]
        public void ResolvedProvider_ShouldBeFullyFunctional()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddCountryData();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var provider = serviceProvider.GetRequiredService<ICountryDataProvider>();
            var country = provider.GetCountryByCode("US");

            // Assert
            Assert.NotNull(country);
            Assert.Equal("US", country.CountryShortCode);
        }

        [Fact]
        public void ResolvedProvider_ShouldHaveAllCountries()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddCountryData();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var provider = serviceProvider.GetRequiredService<ICountryDataProvider>();
            var countries = provider.GetAllCountries().ToList();

            // Assert
            Assert.True(countries.Count > 200);
        }

        #endregion

        #region Extension Method Tests

        [Fact]
        public void AddCountryData_ShouldSupportMethodChaining()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act - Should return IServiceCollection for chaining
            var result = services
                .AddCountryData()
                .AddSingleton<TestService>();

            // Assert
            var serviceProvider = result.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService<ICountryDataProvider>());
            Assert.NotNull(serviceProvider.GetService<TestService>());
        }

        private class TestService { }

        #endregion

        #region Integration Tests

        [Fact]
        public void MultipleClients_ShouldShareSameProviderInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddCountryData();
            var serviceProvider = services.BuildServiceProvider();

            // Act - Multiple clients requesting provider
            var client1 = serviceProvider.GetRequiredService<ICountryDataProvider>();
            var client2 = serviceProvider.GetRequiredService<ICountryDataProvider>();
            var client3 = serviceProvider.GetRequiredService<ICountryDataProvider>();

            // Assert - All should get same singleton
            Assert.Same(client1, client2);
            Assert.Same(client2, client3);
        }

        [Fact]
        public void FullIntegration_ShouldWorkEndToEnd()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddCountryData();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var provider = serviceProvider.GetRequiredService<ICountryDataProvider>();
            
            // Assert - Test real functionality
            Assert.Equal("US", provider.GetCountryByCode("US")?.CountryShortCode);
            Assert.Equal("🇬🇧", provider.GetCountryFlag("GB"));
            Assert.Equal("+1", provider.GetPhoneCodeByCountryShortCode("CA"));
            Assert.Equal("$", provider.GetCurrencySymbol("US"));
        }

        [Fact]
        public void Provider_WithDI_ShouldMaintainPerformance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddCountryData();
            var serviceProvider = services.BuildServiceProvider();
            var provider = serviceProvider.GetRequiredService<ICountryDataProvider>();

            // Warm up
            _ = provider.GetCountryByCode("US");

            // Act - Measure performance
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                _ = provider.GetCountryByCode("US");
            }
            stopwatch.Stop();

            // Assert - Should be fast
            Assert.True(stopwatch.ElapsedMilliseconds < 10,
                $"Expected <10ms but took {stopwatch.ElapsedMilliseconds}ms");
        }

        #endregion
    }
}
