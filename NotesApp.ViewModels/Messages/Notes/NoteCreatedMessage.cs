using CommunityToolkit.Mvvm.Messaging.Messages;
using NotesApp.Models;
using System.Diagnostics;

namespace NotesApp.Messages
{
    public class NoteCreatedMessage : ValueChangedMessage<Note>
    {
        public NoteCreatedMessage(Note note) : base(note)
        {
            Debug.Assert(note != null);
        }
    }
}
