# CountryData.Globalization

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A high-performance .NET library providing comprehensive country data, regions, cultures, currencies, and globalization information. Built for international applications with full country/region support.

## Features

- **High Performance** - O(1) dictionary lookups with lazy-loaded caches
- **250+ Countries** - Complete ISO 3166-1 country data
- **Administrative Regions** - States, provinces, territories
- **Currency Information** - Symbols, codes, names in multiple languages
- **Full Globalization** - CultureInfo and RegionInfo integration
- **Phone Codes** - International dialing codes
- **Multi-Language** - Support for cultures and localized names
- **DI Ready** - Built-in dependency injection support
- **Thread-Safe** - All operations are thread-safe
- **Zero Dependencies** - Lightweight, no external packages

---

## Installation

### Option 1: With Dependency Injection Support (Recommended)

Install both the core package and hosting package:

```bash
# Core package
dotnet add package CountryData.Globalization

# Hosting package (for DI support)
dotnet add package CountryData.Globalization.Hosting
```

**Package Manager Console:**
```powershell
Install-Package CountryData.Globalization
Install-Package CountryData.Globalization.Hosting
```

### Option 2: Core Package Only

If you don't need dependency injection support:

```bash
dotnet add package CountryData.Globalization
```

**Package Manager Console:**
```powershell
Install-Package CountryData.Globalization
```

---

## Quick Start

### Using With Dependency Injection (Recommended)

**Step 1: Register the service**

```csharp
// Program.cs
using CountryData.Globalization.Hosting.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register CountryData services
builder.Services.AddCountryData();

var app = builder.Build();
```

**Step 2: Inject and use**

```csharp
using CountryData.Globalization.Services;

public class CountryController : ControllerBase
{
    private readonly ICountryDataProvider _countryProvider;

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
            Flag = _countryProvider.GetCountryFlag(code),
            Currency = _countryProvider.GetCurrencySymbol(code),
            PhoneCode = country.PhoneCode,
            IsMetric = _countryProvider.IsMetric(code)
        });
    }
}
```

### Using Without Dependency Injection

If you're not using DI, create the provider directly:

```csharp
using CountryData.Globalization.Services;

// Simple - just create it (recommended)
var provider = new CountryDataProvider();

// Use the provider
var country = provider.GetCountryByCode("US");
Console.WriteLine($"{country.CountryName} - {provider.GetCountryFlag("US")}");

// Get regions
var states = provider.GetRegionsByCountryCode("US");
foreach (var state in states)
{
    Console.WriteLine($"{state.Name} ({state.ShortCode})");
}

// Get currency info
var symbol = provider.GetCurrencySymbol("US");
Console.WriteLine($"Currency: {symbol}");
```

**Important:** Reuse the same provider instance. Don't create new instances repeatedly as it's expensive.

```csharp
// GOOD - Singleton pattern
public class CountryDataSingleton
{
    private static readonly Lazy<ICountryDataProvider> _instance = 
        new Lazy<ICountryDataProvider>(() => new CountryDataProvider());

    public static ICountryDataProvider Instance => _instance.Value;
}

// Usage
var country = CountryDataSingleton.Instance.GetCountryByCode("US");
```

---

## API Reference

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
var country = provider.GetCountryByCode("US");
var flag = provider.GetCountryFlag("JP");
var allCountries = provider.GetAllCountries();
```

### Phone Code Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `GetPhoneCodeByCountryShortCode(code)` | Get phone code for country | `string?` |
| `GetCountriesByPhoneCode(phoneCode)` | Get all countries with phone code | `IEnumerable<Country>` |

**Example:**
```csharp
var phoneCode = provider.GetPhoneCodeByCountryShortCode("GB");  // "+44"
var countries = provider.GetCountriesByPhoneCode("+1");         // US, Canada, etc.
```

### Region Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `GetRegionsByCountryCode(code)` | Get states/provinces/territories | `IEnumerable<Region>` |
| `GetCountriesByRegion(regionName)` | Find countries containing region | `IEnumerable<Country>` |

**Example:**
```csharp
var states = provider.GetRegionsByCountryCode("US");
var country = provider.GetCountriesByRegion("California").FirstOrDefault();
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
var symbol = provider.GetCurrencySymbol("US");           // "$"
var name = provider.GetCurrencyEnglishName("GB");        // "British Pound"
var euroCountries = provider.GetCountriesByCurrency("EUR");
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
var culture = provider.GetCultureInfo("US");             // en-US culture
var cultures = provider.GetAllCulturesForCountry("CA");  // en-CA, fr-CA
var frenchCA = provider.GetSpecificCultureByCountryAndLanguage("CA", "fr");
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
var regionInfo = provider.GetRegionInfo("US");
var isMetric = provider.IsMetric("US");                  // false
var iso3 = provider.GetThreeLetterISORegionName("US");   // "USA"
```

---

## Common Usage Examples

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
}

// Usage: FormatPhoneNumber("US", "555-1234") returns "+1 555-1234"
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
        return new CurrencyInfo
        {
            Symbol = _provider.GetCurrencySymbol(countryCode),
            EnglishName = _provider.GetCurrencyEnglishName(countryCode),
            NativeName = _provider.GetCurrencyNativeName(countryCode)
        };
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
}

// Usage: 
// GetLocalizedCountryName("JP", "fr-FR") returns "Japon"
// GetLocalizedCountryName("JP", "es-ES") returns "Japón"
```

---

## Architecture & Performance

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
- Registered as Singleton by default

---

## Data Models

### Country Model

```csharp
public class Country
{
    public string CountryName { get; set; }        // "United States"
    public string CountryShortCode { get; set; }   // "US"
    public string PhoneCode { get; set; }          // "+1"
    public List<Region> Regions { get; set; }      // States/Provinces
}
```

### Region Model

```csharp
public class Region
{
    public string Name { get; set; }         // "California"
    public string? ShortCode { get; set; }   // "CA" (optional)
}
```

---

## Best Practices

### Recommended Usage

```csharp
// GOOD - With DI (Recommended)
builder.Services.AddCountryData();

public class MyService
{
    private readonly ICountryDataProvider _provider;
    
    public MyService(ICountryDataProvider provider)
    {
        _provider = provider;
    }
}

// GOOD - Without DI (Simple instantiation)
var provider = new CountryDataProvider();

// GOOD - Without DI (Singleton pattern)
public class CountryDataSingleton
{
    private static readonly Lazy<ICountryDataProvider> _instance = 
        new Lazy<ICountryDataProvider>(() => new CountryDataProvider());

    public static ICountryDataProvider Instance => _instance.Value;
}
```

### Anti-Patterns to Avoid

```csharp
// BAD - Creating new instances repeatedly
foreach (var item in items)
{
    var provider = new CountryDataProvider();  // Expensive!
    var country = provider.GetCountryByCode(item.Code);
}

// GOOD - Reuse single instance
var provider = new CountryDataProvider();
foreach (var item in items)
{
    var country = provider.GetCountryByCode(item.Code);
}

// BAD - Calling GetCountriesByRegion in loops
foreach (var region in regions)
{
    var country = provider.GetCountriesByRegion(region).FirstOrDefault();  // O(n×m)!
}

// GOOD - Build your own index for repeated lookups
var regionIndex = provider.GetAllCountries()
    .SelectMany(c => c.Regions.Select(r => new { Region = r.Name, Country = c }))
    .ToDictionary(x => x.Region, x => x.Country, StringComparer.OrdinalIgnoreCase);
```




## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- Country data based on **ISO 3166-1** standards
- Culture and region information from .NET `CultureInfo` and `RegionInfo`
- Flag emojis using **Unicode regional indicator symbols**

---








