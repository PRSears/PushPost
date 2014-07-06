namespace PushPost.Models.HtmlGeneration.Embedded
{
    public interface IResource
    {
        string Name { get; set; }
        string Value { get; set; }

        string CreateHTML();
    }
}
