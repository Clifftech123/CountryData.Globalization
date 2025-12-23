namespace CountryData.Globalization.Hosting.Options
{
    /// <summary>
    /// Configuration options for CountryData library.
    /// </summary>
    public class CountryDataOptions
    {

        /// <summary>
        /// Default culture/language code for display names (e.g., "en", "es", "fr").
        /// If not set, uses English names.
        /// </summary>
        public string? DefaultCulture { get; set; }

        /// <summary>
        /// Enable performance caching for RegionInfo and CultureInfo lookups.
        /// Recommended for production. Default is true.
        /// </summary>
        public bool EnableCaching { get; set; } = true;

        /// <summary>
        /// Cache duration in minutes for RegionInfo and CultureInfo lookups.
        /// Only used when EnableCaching is true. Default is 60 minutes.
        /// </summary>
        public int CacheDurationMinutes { get; set; } = 60;



        /// <summary>
        /// Preload all culture and region data on startup for faster access.
        /// Uses more memory but improves performance. Default is false.
        /// </summary>
        public bool PreloadCultureData { get; set; } = false;


        public string? DefaultCountryCode { get; set; }

    }
}
