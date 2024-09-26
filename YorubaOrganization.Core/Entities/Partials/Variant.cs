namespace YorubaOrganization.Core.Entities.Partials
{
    public class Variant(string title, GeoLocation? geoLocation = null)
    {
        public string Title { get; set; } = title;
        public GeoLocation? Geolocation { get; set; } = geoLocation;
    }
}
