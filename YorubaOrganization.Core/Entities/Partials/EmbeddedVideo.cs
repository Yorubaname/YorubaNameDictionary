namespace YorubaOrganization.Core.Entities.Partials;

public class EmbeddedVideo : BaseEntity
{
    public EmbeddedVideo(string videoId, string caption)
    {
        VideoId = videoId;
        Caption = caption;
    }

    public string VideoId { get; set; }
    public string Caption { get; set; }

}
