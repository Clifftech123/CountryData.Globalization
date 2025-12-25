using CountryData.Globalization.Models;
using System.Reflection;
using System.Text.Json;

namespace CountryData.Globalization.Data
{
    /// <summary>
    /// Provides functionality to load country data from embedded JSON resources.
    /// </summary>
    /// <remarks>
    /// This class is responsible for reading country information from an embedded JSON file,
    /// deserializing it into <see cref="Country"/> objects, and enriching the data with
    /// Unicode emoji flags. The country data includes ISO 3166-1 codes, names, phone codes,
    /// and administrative regions.
    /// </remarks>
    public class CountryDataLoader
    {
        /// <summary>
        /// The fully qualified name of the embedded JSON resource containing country data.
        /// </summary>
        private const string DataFileName = "CountryData.Globalization.Data.data.json";

        /// <summary>
        /// Loads all country data from the embedded JSON resource.
        /// </summary>
        /// <returns>
        /// A read-only list of <see cref="Country"/> objects containing comprehensive country information
        /// including ISO codes, names, phone codes, regions, and Unicode emoji flags.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the embedded country data resource cannot be found in the assembly.
        /// </exception>
        /// <exception cref="JsonException">
        /// Thrown when the JSON data cannot be deserialized into country objects.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method performs the following operations:
        /// <list type="number">
        /// <item>Reads the embedded JSON resource from the assembly</item>
        /// <item>Deserializes the JSON into a list of <see cref="Country"/> objects</item>
        /// <item>Enriches each country with its Unicode emoji flag</item>
        /// <item>Returns the data as a read-only collection</item>
        /// </list>
        /// </para>
        /// <para>
        /// The deserialization is case-insensitive to ensure flexibility with different JSON formats.
        /// If deserialization fails or returns null, an empty list is returned instead of throwing an exception.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var loader = new CountryDataLoader();
        /// var countries = loader.LoadCountries();
        ///
        /// foreach (var country in countries)
        /// {
        ///     Console.WriteLine($"{country.CountryName} ({country.CountryShortCode}) - {country.CountryFlag}");
        /// }
        /// </code>
        /// </example>
        public IReadOnlyList<Country> LoadCountries()
        {
            var json = GetEmbeddedJsonData(DataFileName);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var countries = JsonSerializer.Deserialize<List<Country>>(json, options) ?? new List<Country>();

            foreach (var country in countries)
            {
                country.CountryFlag = GetCountryFlag(country.CountryShortCode);
            }

            return countries.AsReadOnly();
        }

        /// <summary>
        /// Reads the content of an embedded resource as a string.
        /// </summary>
        /// <param name="resourceName">
        /// The fully qualified name of the embedded resource to read.
        /// This should include the namespace and file name (e.g., "Namespace.FileName.json").
        /// </param>
        /// <returns>
        /// The complete content of the embedded resource as a string.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the specified embedded resource cannot be found in the executing assembly.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method uses reflection to access embedded resources at runtime. The resource must be
        /// configured as an embedded resource in the project file to be accessible.
        /// </para>
        /// <para>
        /// The method properly disposes of the stream and reader resources using 'using' statements
        /// to prevent memory leaks.
        /// </para>
        /// </remarks>
        private static string GetEmbeddedJsonData(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Converts an ISO 3166-1 alpha-2 country code into its corresponding Unicode emoji flag.
        /// </summary>
        /// <param name="shortCode">
        /// The two-letter ISO 3166-1 alpha-2 country code (e.g., "US", "GB", "JP").
        /// Case-insensitive - will be converted to uppercase automatically.
        /// </param>
        /// <returns>
        /// A string containing the Unicode emoji flag for the specified country.
        /// Returns an empty string if the input is null, empty, or whitespace.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method converts country codes to emoji flags using Unicode Regional Indicator Symbols.
        /// Each letter in the country code is converted to its corresponding regional indicator symbol
        /// by adding 0x1F1A5 to the character's Unicode value.
        /// </para>
        /// <para>
        /// The conversion formula: Regional Indicator = Character + 0x1F1A5
        /// <list type="bullet">
        /// <item>'A' (0x41) becomes 🇦 (0x1F1E6)</item>
        /// <item>'B' (0x42) becomes 🇧 (0x1F1E7)</item>
        /// <item>etc.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var usFlag = GetCountryFlag("US");  // Returns "🇺🇸"
        /// var gbFlag = GetCountryFlag("gb");  // Returns "🇬🇧" (case-insensitive)
        /// var empty = GetCountryFlag("");     // Returns ""
        /// var jpFlag = GetCountryFlag("JP");  // Returns "🇯🇵"
        /// </code>
        /// </example>
        private static string GetCountryFlag(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return string.Empty;
            }

            return string.Concat(shortCode.ToUpperInvariant().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
        }
    }
}
