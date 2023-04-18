using CommunityToolkit.Mvvm.Messaging.Messages;
using NotesApp.Models;

namespace NotesApp.Messages
{
    public class NoteUpdatedMessage : ValueChangedMessage<Note>
    {
        public NoteUpdatedMessage(Note note) : base(note)
        {
        }
    }
}
