using CountryData.Globalization.Models;
using System.Reflection;
using System.Text.Json;

namespace CountryData.Globalization.Data
{
    public class CountryDataLoader
    {
        private const string DataFileName = "CountryData.Globalization.Data.data.json";

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
