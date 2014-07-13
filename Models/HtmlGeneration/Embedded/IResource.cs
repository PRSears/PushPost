namespace PushPost.Models.HtmlGeneration.Embedded
{
    public interface IResource : System.ComponentModel.INotifyPropertyChanged
    {
        string Name { get; set; }
        string Value { get; set; }
        string ResourceType { get; set; }

        string CreateHTML();
    }
}
