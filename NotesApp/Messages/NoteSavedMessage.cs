using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NotesApp.Messages
{
    public class NoteSavedMessage : ValueChangedMessage<Models.Note>
    {
        public NoteSavedMessage(Models.Note note) : base(note)
        {
        }
    }
}
