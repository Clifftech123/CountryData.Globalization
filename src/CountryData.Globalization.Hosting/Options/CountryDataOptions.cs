namespace CountryData.Globalization.Hosting.Options
{
    /// <summary>
    /// Configuration options for CountryData library.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The CountryData library uses automatic lazy-loaded caching for optimal performance.
    /// All caches (RegionInfo, CultureInfo, and lookup indexes) are built on first access
    /// and remain in memory for the application lifetime.
    /// </para>
    /// <para>
    /// This class exists for dependency injection pattern consistency and is reserved
    /// for future configuration options.
    /// </para>
    /// </remarks>
    public class CountryDataOptions
    {
        // Currently no configuration options are needed.
        // The library is fully self-optimizing with:
        // - Lazy-loaded caches (built on first access)
        // - O(1) dictionary lookups for countries
        // - Pre-built indexes for phone codes, currencies, and languages
        // - Thread-safe initialization via Lazy<T>
        //
        // Future options could include:
        // - Custom data source paths
        // - Cache warming strategies
        // - Custom culture/region fallback logic
    }
}
