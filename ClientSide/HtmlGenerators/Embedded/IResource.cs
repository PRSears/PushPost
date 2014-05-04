namespace PushPost.ClientSide.HtmlGenerators.Embedded
{
    public interface IResource
    {
        string Name { get; set; }
        string Value { get; set; }

        string CreateHTML();
    }
}
