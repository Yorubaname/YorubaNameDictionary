namespace Core.Entities
{
    public class GeoLocation : Entity
    {
        public string? Place { get; set; }

        // TODO: Get Kola's brief about why this is separated into Place and Region.
        // TODO: Is the Region which is not currently used anywhere of any use?
        public string? Region { get; set; }


        public GeoLocation() { }

        public GeoLocation(string place, string region) 
        {
            Place = place;
            Region = region;
        }
    }
}