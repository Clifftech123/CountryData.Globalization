using CountryData.Globalization.Data;
using CountryData.Globalization.Models;
using System.Globalization;

namespace CountryData.Globalization.Services
{
    public class CountryDataProvider : ICountryDataProvider
    {
        private readonly IReadOnlyList<Country> _countries;

        public CountryDataProvider(CountryDataLoader loader)
        {
            _countries = loader.LoadCountries();
        }

        public IEnumerable<Country> GetAllCountries()
        {
            return _countries;
        }

        public Country? GetCountryByCode(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return null;
            }

            return _countries.FirstOrDefault(c =>
                string.Equals(c.CountryShortCode, shortCode, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Country> GetCountriesByPhoneCode(string phoneCode)
        {
            if (string.IsNullOrWhiteSpace(phoneCode))
            {
                return Enumerable.Empty<Country>();
            }

            return _countries.Where(c =>
                string.Equals(c.PhoneCode, phoneCode, StringComparison.Ordinal));
        }

        public string? GetPhoneCodeByCountryShortCode(string shortCode)
        {
            return GetCountryByCode(shortCode)?.PhoneCode;
        }

        public IEnumerable<Region> GetRegionsByCountryCode(string shortCode)
        {
            var country = GetCountryByCode(shortCode);
            return country?.Regions ?? Enumerable.Empty<Region>();
        }

        public IEnumerable<string> GetCountryNames()
        {
            return _countries.Select(c => c.CountryName);
        }

        public string GetCountryFlag(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return string.Empty;
            }

            return string.Concat(shortCode.ToUpperInvariant().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
        }

        public CultureInfo? GetCultureInfo(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return null;
            }

            try
            {
                var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                return cultures.FirstOrDefault(c =>
                {
                    try
                    {
                        var region = new RegionInfo(c.Name);
                        return string.Equals(region.TwoLetterISORegionName, shortCode, StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch
            {
                return null;
            }
        }

        public RegionInfo? GetRegionInfo(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return null;
            }

            try
            {
                return new RegionInfo(shortCode);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<Country> GetCountriesByCulture(string cultureName)
        {
            if (string.IsNullOrWhiteSpace(cultureName))
            {
                return Enumerable.Empty<Country>();
            }

            try
            {
                var culture = CultureInfo.GetCultureInfo(cultureName);
                var region = new RegionInfo(culture.Name);
                var shortCode = region.TwoLetterISORegionName;

                return _countries.Where(c =>
                    string.Equals(c.CountryShortCode, shortCode, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return Enumerable.Empty<Country>();
            }
        }

        public IEnumerable<CultureInfo> GetAllCulturesForCountry(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return Enumerable.Empty<CultureInfo>();
            }

            try
            {
                var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                return cultures.Where(c =>
                {
                    try
                    {
                        var region = new RegionInfo(c.Name);
                        return string.Equals(region.TwoLetterISORegionName, shortCode, StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch
            {
                return Enumerable.Empty<CultureInfo>();
            }
        }

        public CultureInfo? GetSpecificCultureByCountryAndLanguage(string shortCode, string languageCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode) || string.IsNullOrWhiteSpace(languageCode))
            {
                return null;
            }

            try
            {
                var cultureName = $"{languageCode}-{shortCode}";
                return CultureInfo.GetCultureInfo(cultureName);
            }
            catch
            {
                return null;
            }
        }

        public string? GetDisplayName(string shortCode, CultureInfo? displayCulture = null)
        {
            var regionInfo = GetRegionInfo(shortCode);
            if (regionInfo == null)
            {
                return null;
            }

            if (displayCulture != null)
            {
                return regionInfo.DisplayName;
            }

            return regionInfo.EnglishName;
        }

        public string? GetCurrencySymbol(string shortCode)
        {
            var regionInfo = GetRegionInfo(shortCode);
            return regionInfo?.CurrencySymbol;
        }

        public string? GetCurrencyEnglishName(string shortCode)
        {
            var regionInfo = GetRegionInfo(shortCode);
            return regionInfo?.CurrencyEnglishName;
        }

        public string? GetCurrencyNativeName(string shortCode)
        {
            var regionInfo = GetRegionInfo(shortCode);
            return regionInfo?.CurrencyNativeName;
        }

        public bool IsMetric(string shortCode)
        {
            var regionInfo = GetRegionInfo(shortCode);
            return regionInfo?.IsMetric ?? false;
        }

        public string? GetThreeLetterISORegionName(string shortCode)
        {
            var regionInfo = GetRegionInfo(shortCode);
            return regionInfo?.ThreeLetterISORegionName;
        }

        public string? GetThreeLetterWindowsRegionName(string shortCode)
        {
            var regionInfo = GetRegionInfo(shortCode);
            return regionInfo?.ThreeLetterWindowsRegionName;
        }

        public IEnumerable<Country> GetCountriesByRegion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
            {
                return Enumerable.Empty<Country>();
            }

            return _countries.Where(c =>
                c.Regions.Any(r => string.Equals(r.Name, regionName, StringComparison.OrdinalIgnoreCase)));
        }

        public IEnumerable<Country> GetCountriesByLanguage(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                return Enumerable.Empty<Country>();
            }

            try
            {
                var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .Where(c => c.TwoLetterISOLanguageName.Equals(languageCode, StringComparison.OrdinalIgnoreCase));

                var countryCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var culture in cultures)
                {
                    try
                    {
                        var region = new RegionInfo(culture.Name);
                        countryCodes.Add(region.TwoLetterISORegionName);
                    }
                    catch
                    {
                        // Skip invalid cultures
                    }
                }

                return _countries.Where(c => countryCodes.Contains(c.CountryShortCode));
            }
            catch
            {
                return Enumerable.Empty<Country>();
            }
        }

        public IEnumerable<Country> GetCountriesByCurrency(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
            {
                return Enumerable.Empty<Country>();
            }

            var matchingCountries = new List<Country>();

            foreach (var country in _countries)
            {
                try
                {
                    var regionInfo = new RegionInfo(country.CountryShortCode);
                    if (string.Equals(regionInfo.ISOCurrencySymbol, currencyCode, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingCountries.Add(country);
                    }
                }
                catch
                {
                    // Skip countries that don't have valid region info
                }
            }

            return matchingCountries;
        }

        public Country? GetCurrentRegionCountry()
        {
            try
            {
                var currentRegion = RegionInfo.CurrentRegion;
                return GetCountryByCode(currentRegion.TwoLetterISORegionName);
            }
            catch
            {
                return null;
            }
        }
    }
}
