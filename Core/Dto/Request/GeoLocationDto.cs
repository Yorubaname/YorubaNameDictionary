namespace Core.Dto.Request
{
    public record GeoLocationDto(string Place, string Region)
    {
        public override string ToString() 
        {
            return Place;
        }
    }

}
