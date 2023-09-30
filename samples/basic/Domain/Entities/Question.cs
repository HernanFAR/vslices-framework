using VSlices.Domain;

namespace Domain.Entities;

public class Question : Entity<string>
{
    public string Title { get; private set; }
    public string Content { get; private set; }

    public Question(string id, string title, string content) : base(id)
    {
        Title = title;
        Content = content;
    }

    public void UpdateState(string title, string content)
    {
        Title = title;
        Content = content;
    }

    internal class Database
    {
        public const string Name = "Questions";
    }

    public class TitleField
    {
        public const int TitleMaxLength = 128;
    }

    public class ContentField
    {
        public const int ContentMaxLength = 1024;
    }
}
