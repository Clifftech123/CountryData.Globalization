namespace CountryData.Globalization.Models
{
    /// <summary>
    /// Represents an administrative region within a country (state, province, territory, etc.).
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Gets or sets the full region name (e.g., "California", "Ontario", "Tokyo").
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the region short code (e.g., "CA", "ON", "13").
        /// </summary>
        public required string ShortCode { get; set; }
    }
}
