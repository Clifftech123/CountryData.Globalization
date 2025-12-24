# CountryData.Globalization 🌍

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A high-performance .NET library providing comprehensive country data, regions, cultures, currencies, and globalization information. Built for international applications with full country/region support.

##  Features

-  **High Performance** - O(1) dictionary lookups with lazy-loaded caches
- **250+ Countries** - Complete ISO 3166-1 country data
-  **Administrative Regions** - States, provinces, territories
-  **Currency Information** - Symbols, codes, names in multiple languages
-  **Flag Emojis** - Unicode emoji flags for all countries
-  **Full Globalization** - CultureInfo and RegionInfo integration
-  **Phone Codes** - International dialing codes
-  **Multi-Language** - Support for cultures and localized names
-  **DI Ready** - Built-in dependency injection support
-  **Thread-Safe** - All operations are thread-safe
-  **Zero Dependencies** - Lightweight, no external packages

---

##  Installation

### .NET CLI
```bash
dotnet add package CountryData.Globalization.Hosting
```

### Package Manager Console
```powershell
Install-Package CountryData.Globalization.Hosting
```

---

##  Quick Start

### Register the Service

```csharp
// Program.cs or Startup.cs
using CountryData.Globalization.Hosting.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register CountryData services
builder.Services.AddCountryData();

var app = builder.Build();
```

### Use in Your Code

```csharp
using CountryData.Globalization.Services;

public class CountryController : ControllerBase
{
    private readonly ICountryDataProvider _countryProvider;

    // Inject the provider
    public CountryController(ICountryDataProvider countryProvider)
    {
        _countryProvider = countryProvider;
    }

    [HttpGet("country/{code}")]
    public IActionResult GetCountry(string code)
    {
        var country = _countryProvider.GetCountryByCode(code);
        if (country == null) return NotFound();

        return Ok(new
        {
            Name = country.CountryName,
            Flag = _countryProvider.GetCountryFlag(code),      // 🇺🇸
            Currency = _countryProvider.GetCurrencySymbol(code), // $
            PhoneCode = country.PhoneCode,                     // +1
            IsMetric = _countryProvider.IsMetric(code)
        });
    }

    [HttpGet("states/{countryCode}")]
    public IActionResult GetStates(string countryCode)
    {
        var regions = _countryProvider.GetRegionsByCountryCode(countryCode);
        return Ok(regions.Select(r => new 
        { 
            Name = r.Name, 
            Code = r.ShortCode 
        }));
    }
}
```

### Complete Example

```csharp
public class CountryService
{
    private readonly ICountryDataProvider _provider;

    public CountryService(ICountryDataProvider provider)
    {
        _provider = provider;
    }

    public CountryInfo GetCountryInformation(string countryCode)
    {
        var country = _provider.GetCountryByCode(countryCode);
        if (country == null) 
            throw new ArgumentException($"Country '{countryCode}' not found");

        return new CountryInfo
        {
            Name = country.CountryName,
            Flag = _provider.GetCountryFlag(countryCode),
            Currency = _provider.GetCurrencySymbol(countryCode),
            CurrencyName = _provider.GetCurrencyEnglishName(countryCode),
            PhoneCode = country.PhoneCode,
            IsMetric = _provider.IsMetric(countryCode),
            ThreeLetterCode = _provider.GetThreeLetterISORegionName(countryCode),
            Regions = _provider.GetRegionsByCountryCode(countryCode).ToList()
        };
    }
}
```

---

##  API Reference

### Country Data Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `GetAllCountries()` | Gets all 250+ countries | `IEnumerable<Country>` |
| `GetCountryByCode(code)` | Get country by ISO 3166-1 alpha-2 code | `Country?` |
| `GetCountryNames()` | Gets all country names | `IEnumerable<string>` |
| `GetCountryFlag(code)` | Gets Unicode emoji flag | `string` |
| `GetCurrentRegionCountry()` | Gets country for current user's region | `Country?` |

**Example:**
```csharp
var country = _provider.GetCountryByCode("US");
var flag = _provider.GetCountryFlag("JP");  // 🇯🇵
var allCountries = _provider.GetAllCountries();
```

### Phone Code Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `GetPhoneCodeByCountryShortCode(code)` | Get phone code for country | `string?` |
| `GetCountriesByPhoneCode(phoneCode)` | Get all countries with phone code | `IEnumerable<Country>` |

**Example:**
```csharp
var phoneCode = _provider.GetPhoneCodeByCountryShortCode("GB");  // "+44"
var countries = _provider.GetCountriesByPhoneCode("+1");         // US, Canada, etc.
```

### Region Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `GetRegionsByCountryCode(code)` | Get states/provinces/territories | `IEnumerable<Region>` |
| `GetCountriesByRegion(regionName)` | Find countries containing region | `IEnumerable<Country>` |

**Example:**
```csharp
var states = _provider.GetRegionsByCountryCode("US");
var country = _provider.GetCountriesByRegion("California").FirstOrDefault();
```

### Currency Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `GetCurrencySymbol(code)` | Get currency symbol | `string?` |
| `GetCurrencyEnglishName(code)` | Get currency English name | `string?` |
| `GetCurrencyNativeName(code)` | Get currency native name | `string?` |
| `GetCountriesByCurrency(currencyCode)` | Get all countries using currency | `IEnumerable<Country>` |

**Example:**
```csharp
var symbol = _provider.GetCurrencySymbol("US");           // "$"
var name = _provider.GetCurrencyEnglishName("GB");        // "British Pound"
var euroCountries = _provider.GetCountriesByCurrency("EUR");
```

### Culture & Globalization Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `GetCultureInfo(code)` | Get primary CultureInfo | `CultureInfo?` |
| `GetAllCulturesForCountry(code)` | Get all cultures for country | `IEnumerable<CultureInfo>` |
| `GetSpecificCultureByCountryAndLanguage(country, lang)` | Get specific culture | `CultureInfo?` |
| `GetCountriesByCulture(cultureName)` | Get countries by culture | `IEnumerable<Country>` |
| `GetCountriesByLanguage(languageCode)` | Get countries by language | `IEnumerable<Country>` |

**Example:**
```csharp
var culture = _provider.GetCultureInfo("US");             // en-US culture
var cultures = _provider.GetAllCulturesForCountry("CA");  // en-CA, fr-CA
var frenchCA = _provider.GetSpecificCultureByCountryAndLanguage("CA", "fr");
```

### RegionInfo Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `GetRegionInfo(code)` | Get .NET RegionInfo | `RegionInfo?` |
| `GetDisplayName(code, culture?)` | Get localized country name | `string?` |
| `IsMetric(code)` | Check if country uses metric | `bool` |
| `GetThreeLetterISORegionName(code)` | Get ISO 3166-1 alpha-3 code | `string?` |
| `GetThreeLetterWindowsRegionName(code)` | Get Windows region code | `string?` |

**Example:**
```csharp
var regionInfo = _provider.GetRegionInfo("US");
var isMetric = _provider.IsMetric("US");                  // false
var iso3 = _provider.GetThreeLetterISORegionName("US");   // "USA"
```

---

##  Common Usage Examples

### Example 1: Country Dropdown for Forms

```csharp
public class CountryDropdownService
{
    private readonly ICountryDataProvider _provider;

    public CountryDropdownService(ICountryDataProvider provider)
    {
        _provider = provider;
    }

    public List<CountryOption> GetCountryOptions()
    {
        return _provider.GetAllCountries()
            .OrderBy(c => c.CountryName)
            .Select(c => new CountryOption
            {
                Code = c.CountryShortCode,
                Name = c.CountryName,
                Flag = _provider.GetCountryFlag(c.CountryShortCode)
            })
            .ToList();
    }
}
```

### Example 2: Phone Number Formatting

```csharp
public class PhoneNumberService
{
    private readonly ICountryDataProvider _provider;

    public PhoneNumberService(ICountryDataProvider provider)
    {
        _provider = provider;
    }

    public string FormatPhoneNumber(string countryCode, string localNumber)
    {
        var phoneCode = _provider.GetPhoneCodeByCountryShortCode(countryCode);
        return $"{phoneCode} {localNumber}";
    }

    public List<CountryPhoneCode> GetAllPhoneCodes()
    {
        return _provider.GetAllCountries()
            .Select(c => new CountryPhoneCode
            {
                Country = c.CountryName,
                Code = c.PhoneCode,
                Flag = _provider.GetCountryFlag(c.CountryShortCode)
            })
            .OrderBy(c => c.Country)
            .ToList();
    }
}

// Usage: FormatPhoneNumber("US", "555-1234") → "+1 555-1234"
```

### Example 3: Address Form with Dynamic Regions

```csharp
public class AddressFormService
{
    private readonly ICountryDataProvider _provider;

    public AddressFormService(ICountryDataProvider provider)
    {
        _provider = provider;
    }

    public List<RegionOption> GetRegionsForCountry(string countryCode)
    {
        return _provider.GetRegionsByCountryCode(countryCode)
            .Select(r => new RegionOption
            {
                Code = r.ShortCode,
                Name = r.Name
            })
            .OrderBy(r => r.Name)
            .ToList();
    }

    public AddressMetadata GetCountryMetadata(string countryCode)
    {
        var country = _provider.GetCountryByCode(countryCode);
        var regions = _provider.GetRegionsByCountryCode(countryCode).ToList();

        return new AddressMetadata
        {
            CountryName = country.CountryName,
            HasRegions = regions.Any(),
            RegionLabel = regions.Any() ? "State/Province" : null,
            PhoneCode = country.PhoneCode,
            IsMetric = _provider.IsMetric(countryCode)
        };
    }
}
```

### Example 4: Currency Display

```csharp
public class CurrencyService
{
    private readonly ICountryDataProvider _provider;

    public CurrencyService(ICountryDataProvider provider)
    {
        _provider = provider;
    }

    public CurrencyInfo GetCurrencyInfo(string countryCode)
    {
        var regionInfo = _provider.GetRegionInfo(countryCode);
        
        return new CurrencyInfo
        {
            Symbol = _provider.GetCurrencySymbol(countryCode),
            EnglishName = _provider.GetCurrencyEnglishName(countryCode),
            NativeName = _provider.GetCurrencyNativeName(countryCode),
            ISOCode = regionInfo?.ISOCurrencySymbol
        };
    }

    public List<string> GetCountriesUsingCurrency(string currencyCode)
    {
        return _provider.GetCountriesByCurrency(currencyCode)
            .Select(c => $"{_provider.GetCountryFlag(c.CountryShortCode)} {c.CountryName}")
            .ToList();
    }
}
```

### Example 5: Multi-Language Support

```csharp
public class LocalizationService
{
    private readonly ICountryDataProvider _provider;

    public LocalizationService(ICountryDataProvider provider)
    {
        _provider = provider;
    }

    public string GetLocalizedCountryName(string countryCode, string languageCode)
    {
        var culture = new CultureInfo(languageCode);
        return _provider.GetDisplayName(countryCode, culture) 
            ?? _provider.GetCountryByCode(countryCode)?.CountryName 
            ?? "Unknown";
    }

    public List<CountryLanguage> GetCountriesByLanguage(string languageCode)
    {
        return _provider.GetCountriesByLanguage(languageCode)
            .Select(c => new CountryLanguage
            {
                Code = c.CountryShortCode,
                Name = c.CountryName,
                Flag = _provider.GetCountryFlag(c.CountryShortCode),
                Cultures = _provider.GetAllCulturesForCountry(c.CountryShortCode)
                    .Select(culture => culture.DisplayName)
                    .ToList()
            })
            .ToList();
    }
}

// Usage: 
// GetLocalizedCountryName("JP", "fr-FR") → "Japon"
// GetLocalizedCountryName("JP", "es-ES") → "Japón"
```

### Example 6: User Preferences Detection

```csharp
public class UserPreferencesService
{
    private readonly ICountryDataProvider _provider;

    public UserPreferencesService(ICountryDataProvider provider)
    {
        _provider = provider;
    }

    public UserPreferences DetectUserPreferences()
    {
        var currentCountry = _provider.GetCurrentRegionCountry();
        if (currentCountry == null) 
            return GetDefaultPreferences();

        var culture = _provider.GetCultureInfo(currentCountry.CountryShortCode);
        
        return new UserPreferences
        {
            CountryCode = currentCountry.CountryShortCode,
            CountryName = currentCountry.CountryName,
            CultureCode = culture?.Name ?? "en-US",
            CurrencySymbol = _provider.GetCurrencySymbol(currentCountry.CountryShortCode),
            IsMetricSystem = _provider.IsMetric(currentCountry.CountryShortCode),
            PhoneCode = currentCountry.PhoneCode,
            Flag = _provider.GetCountryFlag(currentCountry.CountryShortCode)
        };
    }
}
```

---

##  Architecture & Performance

### Performance Characteristics

| Operation | Time Complexity | Notes |
|-----------|----------------|-------|
| `GetCountryByCode` | **O(1)** | Dictionary lookup |
| `GetCountriesByPhoneCode` | **O(1)** | Pre-built index |
| `GetCountriesByCurrency` | **O(1)** | Pre-built index |
| `GetCountriesByLanguage` | **O(1)** | Pre-built index |
| `GetAllCountries` | **O(1)** | Returns cached collection |
| `GetRegionsByCountryCode` | **O(1)** | Direct property access |
| `GetCultureInfo` | **O(1)** | Pre-built cache |
| `GetRegionInfo` | **O(1)** | Pre-built cache |
| `GetCountriesByRegion` | **O(n×m)** | Sequential scan - cache if frequently used |

### Caching Strategy

All caches use **lazy initialization** with thread-safe `Lazy<T>`:

1. **Country Dictionary** - Built at construction (~50ms)
2. **RegionInfo Cache** - Built on first currency/region access (~50-100ms)
3. **CultureInfo Cache** - Built on first culture access (~100-150ms)
4. **Phone Code Index** - Built on first phone query (<10ms)
5. **Currency Index** - Built on first currency query (~50-100ms)
6. **Language Index** - Built on first language query (~100-150ms)

**Memory Footprint:** ~2-5 MB for all caches combined

### Thread Safety

 **100% Thread-Safe**
- Immutable data collections
- Thread-safe lazy initialization via `Lazy<T>`
- No mutable state
- Safe for concurrent access from multiple threads
- **Registered as Singleton** by default

---



##  Best Practices

###  Recommended Usage

```csharp
// ✅ Register as singleton (automatic with AddCountryData)
builder.Services.AddCountryData();

// ✅ Inject in your services
public class MyService
{
    private readonly ICountryDataProvider _provider;
    
    public MyService(ICountryDataProvider provider)
    {
        _provider = provider;
    }
}

// ✅ Use O(1) optimized methods
var country = _provider.GetCountryByCode("US");           // Fast!
var countries = _provider.GetCountriesByPhoneCode("+1");  // Fast!
```

### ❌ Anti-Patterns to Avoid

```csharp
// ❌ DON'T manually instantiate the provider

var provider = new CountryDataProvider(loader);  // Wrong! Use DI instead

// ❌ DON'T call GetCountriesByRegion in loops
foreach (var region in regions)
{
    var country = provider.GetCountriesByRegion(region).FirstOrDefault();  // O(n×m)!
}

// ✅ Instead, build your own index:
var regionIndex = _provider.GetAllCountries()
    .SelectMany(c => c.Regions.Select(r => new { Region = r.Name, Country = c }))
    .ToDictionary(x => x.Region, x => x.Country, StringComparer.OrdinalIgnoreCase);
```

---

##  Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

##  License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

##  Acknowledgments

- Country data based on **ISO 3166-1** standards
- Culture and region information from .NET `CultureInfo` and `RegionInfo`
- Flag emojis using **Unicode regional indicator symbols**

---



