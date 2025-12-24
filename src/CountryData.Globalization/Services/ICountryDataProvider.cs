using CountryData.Globalization.Models;
using System.Globalization;

namespace CountryData.Globalization.Services
{
    /// <summary>
    /// Provides comprehensive country data and globalization information.
    /// All methods are thread-safe and optimized for performance.
    /// </summary>
    public interface ICountryDataProvider
    {
        #region Basic Country Data Operations

        /// <summary>
        /// Gets all available countries in the dataset.
        /// </summary>
        /// <returns>Collection of all countries with their regions and metadata.</returns>
        IEnumerable<Country> GetAllCountries();

        /// <summary>
        /// Gets a specific country by its ISO 3166-1 alpha-2 code.
        /// </summary>
        /// <param name="shortCode">Two-letter country code (e.g., "US", "GB", "FR").</param>
        /// <returns>Country data if found; otherwise, null.</returns>
        /// <example>
        /// <code>
        /// var usa = provider.GetCountryByCode("US");
        /// var japan = provider.GetCountryByCode("JP");
        /// </code>
        /// </example>
        Country? GetCountryByCode(string shortCode);

        /// <summary>
        /// Gets all countries that use the specified international phone code.
        /// </summary>
        /// <param name="phoneCode">International dialing code (e.g., "+1", "+44", "+81").</param>
        /// <returns>Collection of countries using this phone code.</returns>
        IEnumerable<Country> GetCountriesByPhoneCode(string phoneCode);

        /// <summary>
        /// Gets the international phone code for a specific country.
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>Phone code if found; otherwise, null.</returns>
        string? GetPhoneCodeByCountryShortCode(string shortCode);

        /// <summary>
        /// Gets all administrative regions (states/provinces) for a specific country.
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>Collection of regions with their names and codes.</returns>
        IEnumerable<Region> GetRegionsByCountryCode(string shortCode);

        /// <summary>
        /// Gets the names of all countries.
        /// </summary>
        /// <returns>Collection of country names.</returns>
        IEnumerable<string> GetCountryNames();

        /// <summary>
        /// Gets the emoji flag for a specific country.
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>Unicode emoji flag (e.g., "🇺🇸", "🇬🇧", "🇯🇵").</returns>
        string GetCountryFlag(string shortCode);

        #endregion

        #region CultureInfo Operations

        /// <summary>
        /// Gets the primary CultureInfo for a country.
        /// Returns the first specific culture found for the country.
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>CultureInfo if found; otherwise, null.</returns>
        CultureInfo? GetCultureInfo(string shortCode);

        /// <summary>
        /// Gets all available cultures (languages) for a specific country.
        /// For example, Canada has both English (en-CA) and French (fr-CA).
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>Collection of all cultures available for the country.</returns>
        IEnumerable<CultureInfo> GetAllCulturesForCountry(string shortCode);

        /// <summary>
        /// Gets a specific culture by combining country and language codes.
        /// </summary>
        /// <param name="shortCode">Two-letter country code (e.g., "CA").</param>
        /// <param name="languageCode">Two-letter language code (e.g., "en", "fr").</param>
        /// <returns>Specific CultureInfo (e.g., "en-CA", "fr-CA") if found; otherwise, null.</returns>
        CultureInfo? GetSpecificCultureByCountryAndLanguage(string shortCode, string languageCode);

        /// <summary>
        /// Gets all countries that use a specific culture.
        /// </summary>
        /// <param name="cultureName">Culture name (e.g., "en-US", "fr-FR").</param>
        /// <returns>Collection of countries using this culture.</returns>
        IEnumerable<Country> GetCountriesByCulture(string cultureName);

        /// <summary>
        /// Gets the localized display name for a country.
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <param name="displayCulture">Culture to use for display name; if null, uses English.</param>
        /// <returns>Localized country name if found; otherwise, null.</returns>
        string? GetDisplayName(string shortCode, CultureInfo? displayCulture = null);

        #endregion

        #region RegionInfo Operations

        /// <summary>
        /// Gets the RegionInfo for a specific country.
        /// Provides currency, measurement system, and other regional data.
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>RegionInfo if found; otherwise, null.</returns>
        RegionInfo? GetRegionInfo(string shortCode);

        /// <summary>
        /// Gets the currency symbol used in a country (e.g., "$", "€", "¥").
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>Currency symbol if found; otherwise, null.</returns>
        string? GetCurrencySymbol(string shortCode);

        /// <summary>
        /// Gets the English name of the currency (e.g., "US Dollar", "Euro", "Japanese Yen").
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>Currency English name if found; otherwise, null.</returns>
        string? GetCurrencyEnglishName(string shortCode);

        /// <summary>
        /// Gets the native name of the currency in the country's language.
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>Currency native name if found; otherwise, null.</returns>
        string? GetCurrencyNativeName(string shortCode);

        /// <summary>
        /// Determines if a country uses the metric system for measurements.
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>True if the country uses metric; false otherwise.</returns>
        bool IsMetric(string shortCode);

        /// <summary>
        /// Gets the three-letter ISO 3166-1 alpha-3 code (e.g., "USA", "GBR", "JPN").
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>Three-letter ISO code if found; otherwise, null.</returns>
        string? GetThreeLetterISORegionName(string shortCode);

        /// <summary>
        /// Gets the three-letter Windows region name.
        /// </summary>
        /// <param name="shortCode">Two-letter country code.</param>
        /// <returns>Three-letter Windows code if found; otherwise, null.</returns>
        string? GetThreeLetterWindowsRegionName(string shortCode);

        #endregion

        #region Advanced Queries

        /// <summary>
        /// Gets all countries that contain a specific region/state/province name.
        /// </summary>
        /// <param name="regionName">Region name to search for.</param>
        /// <returns>Collection of countries containing this region.</returns>
        IEnumerable<Country> GetCountriesByRegion(string regionName);

        /// <summary>
        /// Gets all countries where a specific language is spoken.
        /// </summary>
        /// <param name="languageCode">Two-letter language code (e.g., "en", "es", "fr").</param>
        /// <returns>Collection of countries where this language is used.</returns>
        IEnumerable<Country> GetCountriesByLanguage(string languageCode);

        /// <summary>
        /// Gets all countries that use a specific currency.
        /// </summary>
        /// <param name="currencyCode">ISO 4217 currency code (e.g., "USD", "EUR", "JPY").</param>
        /// <returns>Collection of countries using this currency.</returns>
        IEnumerable<Country> GetCountriesByCurrency(string currencyCode);

        /// <summary>
        /// Gets the country data for the current user's region.
        /// Based on RegionInfo.CurrentRegion.
        /// </summary>
        /// <returns>Country data for current region if found; otherwise, null.</returns>
        Country? GetCurrentRegionCountry();

        #endregion
    }
}
