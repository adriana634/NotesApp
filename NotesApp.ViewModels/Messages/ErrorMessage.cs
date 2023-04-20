using System.Diagnostics;

namespace NotesApp.Messages
{
    public class ErrorMessage
    {
        public string Title { get; }
        public string Content { get; }

        public ErrorMessage(string title, string content)
        {
            Debug.Assert(title != null);
            Debug.Assert(content != null);

            this.Title = title;
            this.Content = content;
        }
    }
}
