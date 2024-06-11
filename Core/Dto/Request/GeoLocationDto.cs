namespace Core.Dto.Request
{
    /// <summary>
    /// TODO: Add field validations
    /// </summary>
    public record GeoLocationDto(string Place, string Region)
    {

        public override string ToString() 
        {
            return Place;
        }
    }

}
