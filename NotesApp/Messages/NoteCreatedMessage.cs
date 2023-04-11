using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NotesApp.Messages
{
    public class NoteCreatedMessage : ValueChangedMessage<Models.Note>
    {
        public NoteCreatedMessage(Models.Note note) : base(note)
        {
        }
    }
}
