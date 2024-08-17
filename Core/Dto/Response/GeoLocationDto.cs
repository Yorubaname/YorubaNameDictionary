namespace Core.Dto.Response
{
    public record GeoLocationDto(string Id, string Place, string Region)
    {
        public override string ToString()
        {
            return Place;
        }
    }
}
