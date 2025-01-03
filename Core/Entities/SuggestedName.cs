﻿using YorubaOrganization.Core.Entities;

namespace Core.Entities
{
    public class SuggestedName : BaseEntity
    {
        public string? Name { get; set; }
        public string? Details { get; set; }
        public List<GeoLocation> GeoLocation { get; set; }
        public string? Email { get; set; }

        public SuggestedName()
        {
            GeoLocation = new List<GeoLocation>();
        }
    }
}
