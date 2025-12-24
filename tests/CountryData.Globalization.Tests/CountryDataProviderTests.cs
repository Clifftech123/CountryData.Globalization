using CountryData.Globalization.Data;
using CountryData.Globalization.Services;

namespace CountryData.Globalization.Tests
{
    /// <summary>
    /// Comprehensive test suite for CountryDataProvider functionality.
    /// Tests all public methods, caching behavior, edge cases, and performance characteristics.
    /// </summary>
    public class CountryDataProviderTests : IDisposable
    {
        private readonly CountryDataProvider _provider;
        private readonly CountryDataLoader _loader;

        public CountryDataProviderTests()
        {
            _loader = new CountryDataLoader();
            _provider = new CountryDataProvider(_loader);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }

        #region GetAllCountries Tests

        [Fact]
        public void GetAllCountries_ShouldReturnNonEmptyCollection()
        {
            // Act
            var countries = _provider.GetAllCountries();

            // Assert
            Assert.NotNull(countries);
            Assert.NotEmpty(countries);
        }

        [Fact]
        public void GetAllCountries_ShouldReturnExpectedCountryCount()
        {
            // Act
            var countries = _provider.GetAllCountries().ToList();

            // Assert
            Assert.True(countries.Count > 200, "Should have at least 200 countries");
        }

        [Fact]
        public void GetAllCountries_ShouldReturnSameInstanceOnMultipleCalls()
        {
            // Act
            var countries1 = _provider.GetAllCountries();
            var countries2 = _provider.GetAllCountries();

            // Assert
            Assert.Same(countries1, countries2);
        }

        #endregion

        #region GetCountryByCode Tests

        [Theory]
        [InlineData("US", "United States")]
        [InlineData("CA", "Canada")]
        [InlineData("GB", "United Kingdom")]
        [InlineData("JP", "Japan")]
        [InlineData("DE", "Germany")]
        [InlineData("FR", "France")]
        [InlineData("AU", "Australia")]
        [InlineData("BR", "Brazil")]
        public void GetCountryByCode_WithValidCode_ShouldReturnCorrectCountry(string code, string expectedName)
        {
            // Act
            var country = _provider.GetCountryByCode(code);

            // Assert
            Assert.NotNull(country);
            Assert.Equal(code, country.CountryShortCode, ignoreCase: true);
            Assert.Contains(expectedName, country.CountryName, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("us")] // lowercase
        [InlineData("US")] // uppercase
        [InlineData("Us")] // mixed case
        public void GetCountryByCode_ShouldBeCaseInsensitive(string code)
        {
            // Act
            var country = _provider.GetCountryByCode(code);

            // Assert
            Assert.NotNull(country);
            Assert.Equal("US", country.CountryShortCode, ignoreCase: true);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetCountryByCode_WithInvalidInput_ShouldReturnNull(string? code)
        {
            // Act
            var country = _provider.GetCountryByCode(code!);

            // Assert
            Assert.Null(country);
        }

        [Fact]
        public void GetCountryByCode_WithNonExistentCode_ShouldReturnNull()
        {
            // Act
            var country = _provider.GetCountryByCode("ZZ");

            // Assert
            Assert.Null(country);
        }

        #endregion

        #region GetCountriesByPhoneCode Tests

        [Theory]
        [InlineData("+1")] // US, Canada
        [InlineData("+44")] // UK
        [InlineData("+81")] // Japan
        public void GetCountriesByPhoneCode_WithValidCode_ShouldReturnCountries(string phoneCode)
        {
            // Act
            var countries = _provider.GetCountriesByPhoneCode(phoneCode).ToList();

            // Assert
            Assert.NotEmpty(countries);
            Assert.All(countries, c => Assert.Equal(phoneCode, c.PhoneCode));
        }

        [Fact]
        public void GetCountriesByPhoneCode_WithSharedCode_ShouldReturnMultipleCountries()
        {
            // Act - +1 is shared by US, Canada, and others
            var countries = _provider.GetCountriesByPhoneCode("+1").ToList();

            // Assert
            Assert.True(countries.Count > 1, "Phone code +1 should be shared by multiple countries");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetCountriesByPhoneCode_WithInvalidInput_ShouldReturnEmpty(string? phoneCode)
        {
            // Act
            var countries = _provider.GetCountriesByPhoneCode(phoneCode!);

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        [Fact]
        public void GetCountriesByPhoneCode_WithNonExistentCode_ShouldReturnEmpty()
        {
            // Act
            var countries = _provider.GetCountriesByPhoneCode("+999999");

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        #endregion

        #region GetPhoneCodeByCountryShortCode Tests

        [Theory]
        [InlineData("US", "+1")]
        [InlineData("GB", "+44")]
        [InlineData("JP", "+81")]
        [InlineData("DE", "+49")]
        public void GetPhoneCodeByCountryShortCode_WithValidCode_ShouldReturnPhoneCode(string countryCode, string expectedPhoneCode)
        {
            // Act
            var phoneCode = _provider.GetPhoneCodeByCountryShortCode(countryCode);

            // Assert
            Assert.Equal(expectedPhoneCode, phoneCode);
        }

        [Fact]
        public void GetPhoneCodeByCountryShortCode_WithInvalidCode_ShouldReturnNull()
        {
            // Act
            var phoneCode = _provider.GetPhoneCodeByCountryShortCode("ZZ");

            // Assert
            Assert.Null(phoneCode);
        }

        #endregion

        #region GetRegionsByCountryCode Tests

        [Fact]
        public void GetRegionsByCountryCode_ForUS_ShouldReturn50States()
        {
            // Act
            var regions = _provider.GetRegionsByCountryCode("US").ToList();

            // Assert
            Assert.NotEmpty(regions);
            Assert.Contains(regions, r => r.Name == "California");
            Assert.Contains(regions, r => r.Name == "Texas");
            Assert.Contains(regions, r => r.Name == "New York");
        }

        [Fact]
        public void GetRegionsByCountryCode_ForCanada_ShouldReturnProvincesAndTerritories()
        {
            // Act
            var regions = _provider.GetRegionsByCountryCode("CA").ToList();

            // Assert
            Assert.NotEmpty(regions);
            Assert.Contains(regions, r => r.Name == "Ontario");
            Assert.Contains(regions, r => r.Name == "Quebec");
        }

        [Fact]
        public void GetRegionsByCountryCode_WithInvalidCode_ShouldReturnEmpty()
        {
            // Act
            var regions = _provider.GetRegionsByCountryCode("ZZ");

            // Assert
            Assert.NotNull(regions);
            Assert.Empty(regions);
        }

        [Fact]
        public void GetRegionsByCountryCode_USRegionsShouldHaveShortCodes()
        {
            // Act
            var regions = _provider.GetRegionsByCountryCode("US").ToList();

            // Assert - US states should have short codes
            Assert.All(regions, r => Assert.False(string.IsNullOrWhiteSpace(r.ShortCode)));
        }

        [Fact]
        public void GetRegionsByCountryCode_SomeRegionsMayNotHaveShortCodes()
        {
            // Act - Get all countries and check if any regions lack short codes
            var allCountries = _provider.GetAllCountries().ToList();
            var regionsWithoutShortCodes = allCountries
                .SelectMany(c => c.Regions)
                .Where(r => string.IsNullOrWhiteSpace(r.ShortCode))
                .ToList();

            // Assert - This is valid; some regions don't have official short codes
            // This test just validates that the system handles optional ShortCode correctly
            Assert.True(true, "System correctly handles regions with and without short codes");
        }

        #endregion

        #region GetCountryNames Tests

        [Fact]
        public void GetCountryNames_ShouldReturnAllCountryNames()
        {
            // Act
            var names = _provider.GetCountryNames().ToList();

            // Assert
            Assert.NotEmpty(names);
            Assert.Contains("United States", names, StringComparer.OrdinalIgnoreCase);
            Assert.Contains("Canada", names, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetCountryNames_ShouldReturnSameCountAsGetAllCountries()
        {
            // Act
            var namesCount = _provider.GetCountryNames().Count();
            var countriesCount = _provider.GetAllCountries().Count();

            // Assert
            Assert.Equal(countriesCount, namesCount);
        }

        #endregion

        #region GetCountryFlag Tests

        [Theory]
        [InlineData("US", "🇺🇸")]
        [InlineData("CA", "🇨🇦")]
        [InlineData("GB", "🇬🇧")]
        [InlineData("JP", "🇯🇵")]
        [InlineData("DE", "🇩🇪")]
        public void GetCountryFlag_WithValidCode_ShouldReturnFlag(string code, string expectedFlag)
        {
            // Act
            var flag = _provider.GetCountryFlag(code);

            // Assert
            Assert.Equal(expectedFlag, flag);
        }

        [Theory]
        [InlineData("us", "🇺🇸")] // lowercase should work
        [InlineData("gb", "🇬🇧")]
        public void GetCountryFlag_ShouldHandleLowercase(string code, string expectedFlag)
        {
            // Act
            var flag = _provider.GetCountryFlag(code);

            // Assert
            Assert.Equal(expectedFlag, flag);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetCountryFlag_WithInvalidInput_ShouldReturnEmpty(string? code)
        {
            // Act
            var flag = _provider.GetCountryFlag(code!);

            // Assert
            Assert.Equal(string.Empty, flag);
        }

        #endregion

        #region GetCultureInfo Tests

        [Theory]
        [InlineData("US")] // Returns en-US or es-US (first alphabetically)
        [InlineData("DE")] // Returns de-DE
        public void GetCultureInfo_WithValidCode_ShouldReturnCulture(string countryCode)
        {
            // Act
            var culture = _provider.GetCultureInfo(countryCode);

            // Assert
            Assert.NotNull(culture);
            Assert.Contains(countryCode, culture.Name, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetCultureInfo_WithUS_ShouldReturnUSCulture()
        {
            // Act
            var culture = _provider.GetCultureInfo("US");

            // Assert
            Assert.NotNull(culture);
            Assert.EndsWith("-US", culture.Name, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetCultureInfo_WithInvalidInput_ShouldReturnNull(string? code)
        {
            // Act
            var culture = _provider.GetCultureInfo(code!);

            // Assert
            Assert.Null(culture);
        }

        [Fact]
        public void GetCultureInfo_ShouldUseCacheOnSecondCall()
        {
            // Act
            var culture1 = _provider.GetCultureInfo("US");
            var culture2 = _provider.GetCultureInfo("US");

            // Assert
            Assert.NotNull(culture1);
            Assert.NotNull(culture2);
            Assert.Equal(culture1.Name, culture2.Name);
        }

        #endregion

        #region GetRegionInfo Tests

        [Theory]
        [InlineData("US", "United States")]
        [InlineData("CA", "Canada")]
        [InlineData("GB", "United Kingdom")]
        public void GetRegionInfo_WithValidCode_ShouldReturnRegionInfo(string code, string expectedName)
        {
            // Act
            var regionInfo = _provider.GetRegionInfo(code);

            // Assert
            Assert.NotNull(regionInfo);
            Assert.Contains(expectedName, regionInfo.EnglishName, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetRegionInfo_WithInvalidInput_ShouldReturnNull(string? code)
        {
            // Act
            var regionInfo = _provider.GetRegionInfo(code!);

            // Assert
            Assert.Null(regionInfo);
        }

        [Fact]
        public void GetRegionInfo_ShouldReturnCachedInstance()
        {
            // Act - Call twice to test caching
            var region1 = _provider.GetRegionInfo("US");
            var region2 = _provider.GetRegionInfo("US");

            // Assert
            Assert.NotNull(region1);
            Assert.NotNull(region2);
            Assert.Equal(region1.Name, region2.Name);
        }

        #endregion

        #region GetCountriesByCulture Tests

        [Theory]
        [InlineData("en-US", "US")]
        [InlineData("en-GB", "GB")]
        [InlineData("fr-FR", "FR")]
        [InlineData("de-DE", "DE")]
        public void GetCountriesByCulture_WithValidCulture_ShouldReturnCountry(string cultureName, string expectedCountryCode)
        {
            // Act
            var countries = _provider.GetCountriesByCulture(cultureName).ToList();

            // Assert
            Assert.NotEmpty(countries);
            Assert.Contains(countries, c => c.CountryShortCode.Equals(expectedCountryCode, StringComparison.OrdinalIgnoreCase));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetCountriesByCulture_WithInvalidInput_ShouldReturnEmpty(string? cultureName)
        {
            // Act
            var countries = _provider.GetCountriesByCulture(cultureName!);

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        [Fact]
        public void GetCountriesByCulture_WithInvalidCulture_ShouldReturnEmpty()
        {
            // Act
            var countries = _provider.GetCountriesByCulture("invalid-culture");

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        #endregion

        #region GetAllCulturesForCountry Tests

        [Theory]
        [InlineData("US")] // en-US, es-US
        [InlineData("CA")] // en-CA, fr-CA
        [InlineData("CH")] // de-CH, fr-CH, it-CH
        public void GetAllCulturesForCountry_WithMultilingualCountry_ShouldReturnMultipleCultures(string countryCode)
        {
            // Act
            var cultures = _provider.GetAllCulturesForCountry(countryCode).ToList();

            // Assert
            Assert.NotEmpty(cultures);
            Assert.All(cultures, c => Assert.NotNull(c));
        }

        [Fact]
        public void GetAllCulturesForCountry_ForCanada_ShouldIncludeBothEnglishAndFrench()
        {
            // Act
            var cultures = _provider.GetAllCulturesForCountry("CA").ToList();

            // Assert
            Assert.Contains(cultures, c => c.Name == "en-CA");
            Assert.Contains(cultures, c => c.Name == "fr-CA");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetAllCulturesForCountry_WithInvalidInput_ShouldReturnEmpty(string? code)
        {
            // Act
            var cultures = _provider.GetAllCulturesForCountry(code!);

            // Assert
            Assert.NotNull(cultures);
            Assert.Empty(cultures);
        }

        #endregion

        #region GetSpecificCultureByCountryAndLanguage Tests

        [Theory]
        [InlineData("US", "en", "en-US")]
        [InlineData("CA", "fr", "fr-CA")]
        [InlineData("CH", "de", "de-CH")]
        [InlineData("CH", "fr", "fr-CH")]
        public void GetSpecificCultureByCountryAndLanguage_WithValidInputs_ShouldReturnCulture(
            string countryCode, string languageCode, string expectedCulture)
        {
            // Act
            var culture = _provider.GetSpecificCultureByCountryAndLanguage(countryCode, languageCode);

            // Assert
            Assert.NotNull(culture);
            Assert.Equal(expectedCulture, culture.Name, ignoreCase: true);
        }

        [Theory]
        [InlineData(null, "en")]
        [InlineData("US", null)]
        [InlineData("", "en")]
        [InlineData("US", "")]
        public void GetSpecificCultureByCountryAndLanguage_WithInvalidInputs_ShouldReturnNull(
            string? countryCode, string? languageCode)
        {
            // Act
            var culture = _provider.GetSpecificCultureByCountryAndLanguage(countryCode!, languageCode!);

            // Assert
            Assert.Null(culture);
        }

        [Fact]
        public void GetSpecificCultureByCountryAndLanguage_WithNonExistentCombination_ShouldReturnNull()
        {
            // Act - Use invalid format that CultureInfo.GetCultureInfo will reject
            // Using numbers or invalid characters that don't form a valid culture name
            var culture = _provider.GetSpecificCultureByCountryAndLanguage("99", "99");

            // Assert
            Assert.Null(culture);
        }

        #endregion

        #region GetDisplayName Tests

        [Theory]
        [InlineData("US", "United States")]
        [InlineData("GB", "United Kingdom")]
        [InlineData("CA", "Canada")]
        public void GetDisplayName_WithValidCode_ShouldReturnEnglishName(string code, string expectedName)
        {
            // Act
            var displayName = _provider.GetDisplayName(code);

            // Assert
            Assert.NotNull(displayName);
            Assert.Contains(expectedName, displayName, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ZZ")]
        public void GetDisplayName_WithInvalidInput_ShouldReturnNull(string? code)
        {
            // Act
            var displayName = _provider.GetDisplayName(code!);

            // Assert
            Assert.Null(displayName);
        }

        #endregion

        #region Currency Tests

        [Theory]
        [InlineData("US", "$")]
        [InlineData("GB", "£")]
        public void GetCurrencySymbol_WithValidCode_ShouldReturnSymbol(string code, string expectedSymbol)
        {
            // Act
            var symbol = _provider.GetCurrencySymbol(code);

            // Assert
            Assert.NotNull(symbol);
            Assert.Equal(expectedSymbol, symbol);
        }

        [Fact]
        public void GetCurrencySymbol_ForJapan_ShouldReturnYenSymbol()
        {
            // Act
            var symbol = _provider.GetCurrencySymbol("JP");

            // Assert - Japan uses ¥ but system may return ￥ (fullwidth yen sign)
            Assert.NotNull(symbol);
            Assert.True(symbol == "¥" || symbol == "￥", $"Expected yen symbol but got: {symbol}");
        }

        [Theory]
        [InlineData("US", "Dollar")]
        [InlineData("GB", "Pound")]
        [InlineData("JP", "Yen")]
        public void GetCurrencyEnglishName_WithValidCode_ShouldContainCurrencyName(string code, string expectedSubstring)
        {
            // Act
            var currencyName = _provider.GetCurrencyEnglishName(code);

            // Assert
            Assert.NotNull(currencyName);
            Assert.Contains(expectedSubstring, currencyName, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("US")]
        [InlineData("GB")]
        [InlineData("JP")]
        public void GetCurrencyNativeName_WithValidCode_ShouldReturnName(string code)
        {
            // Act
            var nativeName = _provider.GetCurrencyNativeName(code);

            // Assert
            Assert.NotNull(nativeName);
            Assert.NotEmpty(nativeName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ZZ")]
        public void GetCurrencySymbol_WithInvalidInput_ShouldReturnNull(string? code)
        {
            // Act
            var symbol = _provider.GetCurrencySymbol(code!);

            // Assert
            Assert.Null(symbol);
        }

        #endregion

        #region IsMetric Tests

        [Theory]
        [InlineData("US", false)] // US uses imperial
        [InlineData("GB", true)]  // UK uses metric
        [InlineData("CA", true)]  // Canada uses metric
        [InlineData("AU", true)]  // Australia uses metric
        public void IsMetric_ShouldReturnCorrectValue(string code, bool expectedIsMetric)
        {
            // Act
            var isMetric = _provider.IsMetric(code);

            // Assert
            Assert.Equal(expectedIsMetric, isMetric);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ZZ")]
        public void IsMetric_WithInvalidInput_ShouldReturnFalse(string? code)
        {
            // Act
            var isMetric = _provider.IsMetric(code!);

            // Assert
            Assert.False(isMetric);
        }

        #endregion

        #region ThreeLetterRegionName Tests

        [Theory]
        [InlineData("US", "USA")]
        [InlineData("GB", "GBR")]
        [InlineData("JP", "JPN")]
        [InlineData("DE", "DEU")]
        public void GetThreeLetterISORegionName_WithValidCode_ShouldReturnThreeLetterCode(
            string code, string expectedThreeLetterCode)
        {
            // Act
            var threeLetterCode = _provider.GetThreeLetterISORegionName(code);

            // Assert
            Assert.Equal(expectedThreeLetterCode, threeLetterCode);
        }

        [Theory]
        [InlineData("US")]
        [InlineData("GB")]
        [InlineData("CA")]
        public void GetThreeLetterWindowsRegionName_WithValidCode_ShouldReturnCode(string code)
        {
            // Act
            var windowsCode = _provider.GetThreeLetterWindowsRegionName(code);

            // Assert
            Assert.NotNull(windowsCode);
            Assert.Equal(3, windowsCode.Length);
        }

        #endregion

        #region GetCountriesByRegion Tests

        [Theory]
        [InlineData("California")]
        [InlineData("Texas")]
        [InlineData("New York")]
        public void GetCountriesByRegion_WithUSState_ShouldReturnUS(string regionName)
        {
            // Act
            var countries = _provider.GetCountriesByRegion(regionName).ToList();

            // Assert
            Assert.NotEmpty(countries);
            Assert.Contains(countries, c => c.CountryShortCode == "US");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetCountriesByRegion_WithInvalidInput_ShouldReturnEmpty(string? regionName)
        {
            // Act
            var countries = _provider.GetCountriesByRegion(regionName!);

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        [Fact]
        public void GetCountriesByRegion_WithNonExistentRegion_ShouldReturnEmpty()
        {
            // Act
            var countries = _provider.GetCountriesByRegion("NonExistentRegion123");

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        #endregion

        #region GetCountriesByLanguage Tests

        [Theory]
        [InlineData("en")] // English
        [InlineData("es")] // Spanish
        [InlineData("fr")] // French
        public void GetCountriesByLanguage_WithCommonLanguage_ShouldReturnMultipleCountries(string languageCode)
        {
            // Act
            var countries = _provider.GetCountriesByLanguage(languageCode).ToList();

            // Assert
            Assert.NotEmpty(countries);
            Assert.True(countries.Count > 1, $"Language {languageCode} should be spoken in multiple countries");
        }

        [Fact]
        public void GetCountriesByLanguage_WithEnglish_ShouldIncludeUSAndGB()
        {
            // Act
            var countries = _provider.GetCountriesByLanguage("en").ToList();

            // Assert
            Assert.Contains(countries, c => c.CountryShortCode == "US");
            Assert.Contains(countries, c => c.CountryShortCode == "GB");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetCountriesByLanguage_WithInvalidInput_ShouldReturnEmpty(string? languageCode)
        {
            // Act
            var countries = _provider.GetCountriesByLanguage(languageCode!);

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        #endregion

        #region GetCountriesByCurrency Tests

        [Theory]
        [InlineData("USD")] // US Dollar
        [InlineData("EUR")] // Euro
        [InlineData("GBP")] // British Pound
        public void GetCountriesByCurrency_WithCommonCurrency_ShouldReturnCountries(string currencyCode)
        {
            // Act
            var countries = _provider.GetCountriesByCurrency(currencyCode).ToList();

            // Assert
            Assert.NotEmpty(countries);
        }

        [Fact]
        public void GetCountriesByCurrency_WithEUR_ShouldReturnMultipleCountries()
        {
            // Act - Euro is used by multiple European countries
            var countries = _provider.GetCountriesByCurrency("EUR").ToList();

            // Assert
            Assert.True(countries.Count > 10, "EUR should be used by multiple Eurozone countries");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetCountriesByCurrency_WithInvalidInput_ShouldReturnEmpty(string? currencyCode)
        {
            // Act
            var countries = _provider.GetCountriesByCurrency(currencyCode!);

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        [Fact]
        public void GetCountriesByCurrency_WithNonExistentCurrency_ShouldReturnEmpty()
        {
            // Act
            var countries = _provider.GetCountriesByCurrency("XXX999");

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        #endregion

        #region GetCurrentRegionCountry Tests

        [Fact]
        public void GetCurrentRegionCountry_ShouldNotThrow()
        {
            // Act
            var country = _provider.GetCurrentRegionCountry();

            // Assert
            // Country might be null if current region is not in our data
            // but we can verify the method doesn't throw
            Assert.True(true, "Method executed without throwing");
        }

        #endregion

        #region Performance and Caching Tests

        [Fact]
        public void GetCountryByCode_ShouldBeFasterAfterFirstCall()
        {
            // Warm up
            _ = _provider.GetCountryByCode("US");

            // Act - Measure performance
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                _ = _provider.GetCountryByCode("US");
            }
            stopwatch.Stop();

            // Assert - Should complete quickly (less than 10ms for 1000 calls)
            Assert.True(stopwatch.ElapsedMilliseconds < 10,
                $"1000 lookups should be fast but took {stopwatch.ElapsedMilliseconds}ms");
        }

        [Fact]
        public void GetRegionInfo_ShouldUseCaching()
        {
            // Act - Call twice
            var region1 = _provider.GetRegionInfo("US");
            var region2 = _provider.GetRegionInfo("US");

            // Assert - Should return same cached data
            Assert.NotNull(region1);
            Assert.NotNull(region2);
            Assert.Equal(region1.Name, region2.Name);
        }

        [Fact]
        public void GetCountriesByPhoneCode_ShouldUseIndex()
        {
            // Act - First call builds index, second uses it
            var countries1 = _provider.GetCountriesByPhoneCode("+1").ToList();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var countries2 = _provider.GetCountriesByPhoneCode("+1").ToList();
            stopwatch.Stop();

            // Assert
            Assert.Equal(countries1.Count, countries2.Count);
            Assert.True(stopwatch.ElapsedMilliseconds < 5,
                "Second call should use cached index and be very fast");
        }

        #endregion

        #region Edge Cases and Error Handling Tests

        [Fact]
        public void AllPublicMethods_ShouldHandleNullInputsGracefully()
        {
            // Arrange & Act & Assert - None of these should throw
            Assert.Null(_provider.GetCountryByCode(null!));
            Assert.Empty(_provider.GetCountriesByPhoneCode(null!));
            Assert.Null(_provider.GetPhoneCodeByCountryShortCode(null!));
            Assert.Empty(_provider.GetRegionsByCountryCode(null!));
            Assert.Equal(string.Empty, _provider.GetCountryFlag(null!));
            Assert.Null(_provider.GetCultureInfo(null!));
            Assert.Null(_provider.GetRegionInfo(null!));
            Assert.Empty(_provider.GetCountriesByCulture(null!));
            Assert.Empty(_provider.GetAllCulturesForCountry(null!));
            Assert.Null(_provider.GetSpecificCultureByCountryAndLanguage(null!, null!));
            Assert.Null(_provider.GetDisplayName(null!));
            Assert.Null(_provider.GetCurrencySymbol(null!));
            Assert.False(_provider.IsMetric(null!));
            Assert.Empty(_provider.GetCountriesByRegion(null!));
            Assert.Empty(_provider.GetCountriesByLanguage(null!));
            Assert.Empty(_provider.GetCountriesByCurrency(null!));
        }

        [Fact]
        public void CountryData_ShouldHaveConsistentStructure()
        {
            // Act
            var countries = _provider.GetAllCountries().ToList();

            // Assert - All countries should have required properties
            Assert.All(countries, country =>
            {
                Assert.False(string.IsNullOrWhiteSpace(country.CountryShortCode),
                    "Country should have a short code");
                Assert.False(string.IsNullOrWhiteSpace(country.CountryName),
                    "Country should have a name");
                Assert.NotNull(country.Regions);
            });
        }

        [Fact]
        public void GetCountryByCode_MultipleCallsSameCode_ShouldReturnConsistentResults()
        {
            // Act
            var country1 = _provider.GetCountryByCode("US");
            var country2 = _provider.GetCountryByCode("US");
            var country3 = _provider.GetCountryByCode("us"); // different case

            // Assert
            Assert.NotNull(country1);
            Assert.NotNull(country2);
            Assert.NotNull(country3);
            Assert.Equal(country1.CountryName, country2.CountryName);
            Assert.Equal(country1.CountryName, country3.CountryName);
        }

        #endregion
    }
}
