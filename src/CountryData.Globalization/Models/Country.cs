namespace CountryData.Globalization.Models {
    /// <summary>
    /// Represents a country with its associated data including ISO codes, names, phone codes, regions, and Unicode emoji flag.
    ///
    /// </summary>
    public class Country {
        /// <summary>
        /// Gets or sets the full country name (e.g., "United States", "Canada", "Japan").
        /// </summary>
        public string CountryName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the international phone code for the country (e.g., "+1", "+44", "+81").
        /// </summary>
        public string PhoneCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-2 country code (e.g., "US", "CA", "JP").
        /// </summary>
        public string CountryShortCode { get; set; } = string.Empty;

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

/// <summary>
/// Represents descriptive information about a language associated with a country.
/// </summary>
public class LanguageInfo
{
    /// <summary>
    /// Gets or sets the two-letter ISO language code (e.g., "en", "fr").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the English name of the language.
    /// </summary>
    public string EnglishName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the language as it is written in its own script.
    /// </summary>
    public string NativeName { get; set; } = string.Empty;
}

