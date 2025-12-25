namespace CountryData.Globalization.Models
{
    /// <summary>
    /// Represents a country with its associated data including ISO codes, names, phone codes, regions, and Unicode emoji flag.
    /// 
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Gets or sets the full country name (e.g., "United States", "Canada", "Japan").
        /// </summary>
        public required string CountryName { get; set; }

        /// <summary>
        /// Gets or sets the international phone code for the country (e.g., "+1", "+44", "+81").
        /// </summary>
        public required string PhoneCode { get; set; }

        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-2 country code (e.g., "US", "CA", "JP").
        /// </summary>
        public required string CountryShortCode { get; set; }

        /// <summary>
        /// Gets or sets the Unicode emoji flag representing the country (e.g., "🇺🇸", "🇨🇦", "🇯🇵").
        /// </summary>
        public string CountryFlag { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of administrative regions (states, provinces, territories, etc.) within the country.
        /// </summary>
        public List<Region> Regions { get; set; } = new();
    }
}

