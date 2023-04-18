namespace NotesApp.Messages
{
    public class ErrorMessage
    {
        public string Title { get; }
        public string Content { get; }

        public ErrorMessage(string title, string content)
        {
            this.Title = title;
            this.Content = content;
        }
    }
}
