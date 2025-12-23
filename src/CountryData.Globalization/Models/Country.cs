namespace CountryData.Globalization.Models
{
    public class Country
    {
        public required string CountryName { get; set; }
        public required string PhoneCode { get; set; }
        public required string CountryShortCode { get; set; }
        public string CountryFlag { get; set; } = string.Empty;
        public List<Region> Regions { get; set; } = new();
    }
}

