namespace Core.Entities.NameEntry.Collections
{
    /// <summary>
    /// Feedback from anonymous website users. 
    /// TODO: We can capture their name or email address going forward (in the universal SubmittedBy field)
    /// </summary>
    public class Feedback : Entity
    {
        public string? Content { get; set; }

        public Feedback() { }

        public Feedback(string content)
        {
            Content = content;
        }
    }
}
