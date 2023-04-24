using CommunityToolkit.Mvvm.Messaging.Messages;
using NotesApp.Models;
using System.Diagnostics;

namespace NotesApp.Messages
{
    public class NoteUpdatedMessage : ValueChangedMessage<Note>
    {
        public NoteUpdatedMessage(Note note) : base(note)
        {
            Debug.Assert(note != null);
        }
    }
}
