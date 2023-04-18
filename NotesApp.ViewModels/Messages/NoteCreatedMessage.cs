using CommunityToolkit.Mvvm.Messaging.Messages;
using NotesApp.Models;

namespace NotesApp.Messages
{
    public class NoteCreatedMessage : ValueChangedMessage<Note>
    {
        public NoteCreatedMessage(Note note) : base(note)
        {
        }
    }
}
