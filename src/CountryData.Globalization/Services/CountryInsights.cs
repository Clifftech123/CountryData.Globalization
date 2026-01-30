using System.Globalization;
namespace CountryData.Globalization.Services;


/// <summary>
/// Provides derived insights and helper methods for country data.
/// </summary>
public class CountryInsights {
    private readonly ICountryDataProvider _countryDataProvider;

    /// <summary>
    /// Sets the country context for the insights.
    /// </summary>
    public CountryInsights(ICountryDataProvider countryDataProvider) {
        _countryDataProvider = countryDataProvider;

    }

    /// <summary>
    /// Determines whether a country has multiple languages associated with it.
    /// </summary>
    /// <param name="countryShortCode">The ISO 3166-1 alpha-2 country code (e.g., "CH" for Switzerland).</param>
    /// <returns>True if the country is multilingual; otherwise, false.</returns>
    public bool IsMultilingualCountry(string countryShortCode) {
        // We look at all specific cultures and count how many match this country code
        var languageCount = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(c => {
                try { return new RegionInfo(c.Name).TwoLetterISORegionName; }
                catch { return null; }
            })
            .Count(regionCode => regionCode != null &&
                                 regionCode.Equals(countryShortCode, StringComparison.OrdinalIgnoreCase));

        return languageCount > 1;
    }

    /// <summary>
    /// Returns the primary culture name associated with a country.
    /// </summary>
    /// <param name="countryShortCode">The ISO 3166-1 alpha-2 country code.</param>
    /// <returns>The culture name (e.g., "en-US") or null if not found.</returns>
    public string? GetPrimaryCulture(string countryShortCode) {
        // Returns the first culture found for this region
        return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .FirstOrDefault(c => {
                try { return new RegionInfo(c.Name).TwoLetterISORegionName.Equals(countryShortCode, StringComparison.OrdinalIgnoreCase); }
                catch { return false; }
            })?.Name;
    }

    /// <summary>
    /// Returns a list of languages associated with the country.
    /// </summary>
    /// <param name="countryCode">The ISO 3166-1 alpha-2 country code.</param>
    public IEnumerable<LanguageInfo> GetCountryLanguages(string countryCode) {
        var languagesFound = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Where(c => {
                try { return new RegionInfo(c.Name).TwoLetterISORegionName.Equals(countryCode, StringComparison.OrdinalIgnoreCase); }
                catch { return false; }
            })
            .Select(c => new LanguageInfo {
                Code = c.TwoLetterISOLanguageName,
                NativeName = c.NativeName,
                EnglishName = c.EnglishName
            })
            .GroupBy(l => l.Code)
            .Select(g => g.First())
            .ToList();

        return languagesFound;
    }
}