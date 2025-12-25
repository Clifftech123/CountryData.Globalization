using CountryData.Globalization.Data;
using CountryData.Globalization.Hosting.Options;
using CountryData.Globalization.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CountryData.Globalization.Hosting.Extensions
{
    /// <summary>
    /// Extension methods for registering CountryData services in the dependency injection container.
    /// </summary>
    public static class CountryDataServiceCollectionExtensions
    {
        /// <summary>
        /// Adds CountryData services to the dependency injection container with default options.
        /// </summary>
        public static IServiceCollection AddCountryData(this IServiceCollection services)
        {
            services.AddSingleton<CountryDataLoader>();
            services.AddSingleton<ICountryDataProvider, CountryDataProvider>();
            services.AddSingleton(new CountryDataOptions());

            return services;
        }

        /// <summary>
        /// Adds CountryData services to the dependency injection container with custom options.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configureOptions">Action to configure CountryDataOptions</param>
        public static IServiceCollection AddCountryData(
            this IServiceCollection services,
            Action<CountryDataOptions> configureOptions)
        {
            var options = new CountryDataOptions();
            configureOptions?.Invoke(options);

            services.AddSingleton(options);
            services.AddSingleton<CountryDataLoader>();
            services.AddSingleton<ICountryDataProvider, CountryDataProvider>();

            return services;
        }
    }
}
